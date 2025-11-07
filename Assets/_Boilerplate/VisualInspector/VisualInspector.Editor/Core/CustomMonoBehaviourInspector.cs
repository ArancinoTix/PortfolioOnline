using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using VisualInspector.Editor.Drawers;

namespace VisualInspector.Editor.Core
{
    /// <summary>
    ///     Custom mono behaviour inspector that allows for <see cref="VisualElement"/> attribute based drawers
    ///     eg: <see cref="ButtonAttribute"/> and <see cref="ButtonPropertyDrawer"/>
    /// </summary>
    [CustomEditor(typeof(MonoBehaviour), true, isFallback = true)]
    public class CustomMonoBehaviourInspector : UnityEditor.Editor
    {
        private const string SettingsJsonName = "settings.json";
        private static readonly string LibraryPluginPath = Path.Combine(Application.dataPath, "..", "Library", "VisualInspector");
        
        private static Dictionary<Type, VisualDrawer> _visualDrawers;
        private static JObject _settings;
        // Write a regex to match X from order-[X]
        private static Regex _orderRegex = new Regex(@"order-\[(-?\S+)\]", RegexOptions.Compiled);

        private List<VisualDrawer> _activeDrawers = new();

        /// <summary>
        ///     Stores information about current inspector class
        /// </summary>
        private InspectorData _inspectorData;
        
        /// <summary>
        ///     Initialize static resources to cache all visual types if not visualized for one time performance penalty,
        ///     Also initialize all methods and properties to get 
        /// </summary>
        public virtual void OnEnable()
        {
            if (_visualDrawers == null)
            {
                _visualDrawers = new Dictionary<Type, VisualDrawer>();
                var visualDrawerTypes = Assembly.GetExecutingAssembly().GetTypes()
                    .Where(t => t.IsSubclassOf(typeof(VisualDrawer)));

                foreach (var visualDrawerType in visualDrawerTypes)
                {
                    var visualDrawer = (VisualDrawer) Activator.CreateInstance(visualDrawerType);
                    var targetAttribute = visualDrawerType.GetCustomAttribute<VisualDrawerTargetAttribute>(); 
                    _visualDrawers.Add(targetAttribute.TargetType, visualDrawer);
                }
            }

            _settings ??= GetSettingsObject();

            foreach (var visualDrawer in _activeDrawers)
            {
                visualDrawer.OnEnable();
            }
        }

        public virtual void OnDisable()
        {
            foreach (var visualDrawer in _activeDrawers)
            {
                visualDrawer.OnDisable();
            }
        }


        /// <summary>
        ///     Draw a custom inspector GUI 
        /// </summary>
        /// <returns></returns>
        public override VisualElement CreateInspectorGUI()
        {
            if (target.GetType().Assembly.FullName.Contains("Unity"))
                return base.CreateInspectorGUI();
            
            var root = new VisualElement();
            _inspectorData = new InspectorData(this, target, root);
            
            // Add script field
            var scriptField = new ObjectField("Script");
            scriptField.objectType = typeof(MonoScript);
            scriptField.AddToClassList("order-[-10000]");
            if (target is MonoBehaviour)
            {
                scriptField.value = MonoScript.FromMonoBehaviour((MonoBehaviour) target);
            }
            else
            {
                scriptField.value = MonoScript.FromScriptableObject((ScriptableObject) target);
            }
            scriptField.SetEnabled(false);
            root.Add(scriptField);

            AlphaWarning(root);
            var inspector = DoDefaultVisualElementsInspector();//new IMGUIContainer(() => base.OnInspectorGUI());
            foreach (var element in inspector)
            {
                root.Add(element);
            }
            
            // Draw all custom inspector on fields
            foreach (var field in _inspectorData.Members)
            {
                if(field.Attributes == null) 
                    continue;
                foreach (var attribute in field.Attributes)
                {
                    if (!TryGetVisualDrawer(attribute.GetType(), out var visualDrawer)) 
                        continue;
                    
                    var visualElement = CreateVisualDrawer(visualDrawer, field.MemberInfo, attribute, root);
                    root.Add(visualElement);
                }
            }
            OrderVisualElements(root);
            return root;
        }

        /// <summary>
        ///     Create a visual element from a visual drawer
        /// </summary>
        /// <param name="visualDrawer"></param>
        /// <param name="memberInfo"></param>
        /// <param name="attribute"></param>
        /// <param name="root"></param>
        /// <returns></returns>
        private VisualElement CreateVisualDrawer(VisualDrawer visualDrawer, MemberInfo memberInfo, Attribute attribute,
            VisualElement root)
        {
            visualDrawer.Target = target;
            visualDrawer.MemberInfo = memberInfo;
            visualDrawer.Attribute = attribute as VisualAttribute;
            visualDrawer.TargetVisualElement = root.Q(memberInfo.Name);
            var visualElement = visualDrawer.CreateInspectorGUI(_inspectorData);
            _activeDrawers.Add(visualDrawer);
            visualDrawer.OnEnable();
            return visualElement;
        }

        private bool TryGetVisualDrawer(Type type, out VisualDrawer visualDrawer)
        {
            foreach (var pair in _visualDrawers.Where(pair => pair.Key.IsAssignableFrom(type)))
            {
                visualDrawer = Activator.CreateInstance(pair.Value.GetType()) as VisualDrawer;
                return true;
            }

            visualDrawer = null;
            return false;
        }

        /// <summary>
        ///     Draw default inspector GUI using Visual Elements
        /// </summary>
        /// <returns></returns>
        private IEnumerable<VisualElement> DoDefaultVisualElementsInspector()
        {
            foreach (var dataField in _inspectorData.Fields)
            {
                if(dataField.Attributes.Any(a => a is HideInInspector))
                    continue;
                if(!(dataField.MemberInfo.IsPublic || dataField.Attributes.Any(a => a is SerializeField)))
                    continue;
                
                if(_inspectorData.SerializedObject == null)
                    continue;
                
                var serializedProperty = _inspectorData.SerializedObject.FindProperty(dataField.MemberInfo.Name);
                
                if(serializedProperty == null) 
                    continue;
                
                var field = new PropertyField(serializedProperty);
                field.BindProperty(serializedProperty);
                var container = new VisualElement();
                container.name = dataField.MemberInfo.Name;
                container.Add(field);
                yield return container;
            }
        }

        /// <summary>
        ///     Warn about alpha status of this plugin
        /// </summary>
        private void AlphaWarning(VisualElement container)
        {
            var dismissWarning = false;
            if (_settings.TryGetValue("dismissWarning", out var dismissWarningToken))
            {
                dismissWarning = dismissWarningToken.Value<bool>();
            }

            if (dismissWarning) return;
            var infoBox = new HelpBox("This script overwrites all MonoBehaviour components, with custom logic." +
                                      "Please report on Slack if something is broken", HelpBoxMessageType.Warning);
            container.Add(infoBox);
                
            var button = new Button()
            {
                text = "Dismiss"
            };
            button.clicked += () =>
            {
                _settings["dismissWarning"] = true;
                SaveSettings(_settings);
                container.Remove(infoBox);
            };
            infoBox.Add(button);
        }
        
        /// <summary>
        ///     Reorder visual elements based on order-[X] class name
        ///     TODO: This can be more efficient by creating dummy elements and reordering them and only after that add them to the root
        /// </summary>
        /// <param name="root"></param>
        private void OrderVisualElements(VisualElement root)
        {
            int KeySelector(VisualElement element)
            {
                var orderClass = element.GetClasses().FirstOrDefault(s => s.StartsWith("order-"));
                if (orderClass == null) return 0;
                // Write a regex to extract X from order-[X]
                var order = _orderRegex.Match(orderClass).Groups[1].Value;
                if (string.IsNullOrEmpty(order)) return 0;
                return int.Parse(order);
            }

            var order = root.Children().OrderBy(KeySelector).ToArray();
            root.Clear();
            foreach (var element in order)
            {
                root.Add(element);
            }
        }
        
        /// <summary>
        ///     Save all settings from JObject
        /// </summary>
        /// <param name="o"></param>
        private void SaveSettings(JObject o)
        {
            var path = Path.Combine(LibraryPluginPath, SettingsJsonName);
            Directory.CreateDirectory(LibraryPluginPath);
            File.WriteAllText(path, o.ToString(Formatting.Indented));
        }
        
        /// <summary>
        ///     Read settings as JObject
        /// </summary>
        /// <returns></returns>
        private JObject GetSettingsObject()
        {
            var path = Path.Combine(LibraryPluginPath, SettingsJsonName);
            if (!File.Exists(path)) return new JObject();
            var fileText = File.ReadAllText(path);
            var json = JObject.Parse(fileText);
            return json;
        }
    }
}
