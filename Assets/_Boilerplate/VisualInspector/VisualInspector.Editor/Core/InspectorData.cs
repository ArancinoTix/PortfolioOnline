using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace VisualInspector.Editor.Core
{
    /// <summary>
    ///     Container for all information about MonoBehaviour. It's used to cache some metadata about MonoBehaviour.
    /// </summary>
    public class InspectorData
    {
        public InspectorData(UnityEditor.Editor editor, Object target, VisualElement root)
        {
            var bindingFlags = BindingFlags.Public |
                               BindingFlags.NonPublic |
                               BindingFlags.Instance |
                               BindingFlags.Static |
                               BindingFlags.FlattenHierarchy |
                               BindingFlags.DeclaredOnly;
            var targetType = target.GetType();
            var baseTypes = new List<Type>();
            while (targetType != null)
            {
                baseTypes.Add(targetType);
                targetType = targetType.BaseType;
            }

            var methods = baseTypes.SelectMany(x => x
                    .GetMethods(bindingFlags))
                .Select(x => new MemberAttribute<MethodInfo>(x)).ToArray();
            var properties = baseTypes.SelectMany(x => x
                    .GetProperties(bindingFlags))
                .Select(x => new MemberAttribute<PropertyInfo>(x)).ToArray();
            var fields = baseTypes.SelectMany(x => x
                    .GetFields(bindingFlags))
                .Select(x => new MemberAttribute<FieldInfo>(x)).ToArray();

            Editor = editor;
            Target = target;
            Properties = properties;
            Fields = fields;
            Methods = methods;
            Root = root;

            if(target == null)
                return;
            SerializedObject = new SerializedObject(target);
        }

        /// <summary>
        ///     Cached properties of MonoBehaviour.
        /// </summary>
        public MemberAttribute<PropertyInfo>[] Properties { get; }

        /// <summary>
        ///     Cached fields of MonoBehaviour.
        /// </summary>
        public MemberAttribute<FieldInfo>[] Fields { get; }

        /// <summary>
        ///     Cached methods of MonoBehaviour.
        /// </summary>
        public MemberAttribute<MethodInfo>[] Methods { get; }

        /// <summary>
        ///     Root of visual element tree.
        /// </summary>
        public IEnumerable<IMemberAttribute> Members
        {
            get
            {
                foreach (var property in Fields)
                {
                    yield return property;
                }

                foreach (var field in Properties)
                {
                    yield return field;
                }

                foreach (var method in Methods)
                {
                    yield return method;
                }
            }
        }

        /// <summary>
        ///     Root of visual element tree.
        /// </summary>
        public VisualElement Root { get; set; }

        /// <summary>
        ///     Editor class owning this data.
        /// </summary>
        public UnityEditor.Editor Editor { get; }

        /// <summary>
        ///     Underlying MonoBehaviour.
        /// </summary>
        public Object Target { get; }

        /// <summary>
        ///     Serialized object
        /// </summary>
        public SerializedObject SerializedObject { get; set; }
    }
}