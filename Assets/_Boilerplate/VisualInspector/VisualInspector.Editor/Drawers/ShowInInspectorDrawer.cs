using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using VisualInspector.Editor.Core;

namespace VisualInspector.Editor.Drawers
{
    [VisualDrawerTarget(typeof(ShowInInspectorAttribute))]
    public class ShowInInspectorDrawer : VisualDrawer
    {
        private object _value;
        private VisualElement _drawer;
        
        public override VisualElement CreateInspectorGUI(InspectorData inspectorData)
        {
            if (MemberInfo is PropertyInfo propertyInfo)
            {
                switch (propertyInfo.PropertyType)
                {
                    case var _ when propertyInfo.PropertyType == typeof(bool):
                        var toggle = new Toggle();
                        toggle.label = propertyInfo.Name;
                        toggle.RegisterValueChangedCallback(evt =>
                        {
                            ChangeValue(evt, propertyInfo);
                        });
                        _drawer = toggle;
                        break;
                    case var _ when propertyInfo.PropertyType == typeof(int):
                        var intField = new IntegerField();
                        intField.label = propertyInfo.Name;
                        intField.RegisterValueChangedCallback(evt =>
                        {
                            ChangeValue(evt, propertyInfo);
                        });
                        _drawer = intField;
                        break;
                    case var _ when propertyInfo.PropertyType == typeof(float):
                        var floatField = new FloatField();
                        floatField.label = propertyInfo.Name;
                        floatField.RegisterValueChangedCallback(evt =>
                        {
                            ChangeValue(evt, propertyInfo);
                        });
                        _drawer = floatField;
                        break;
                    case var _ when propertyInfo.PropertyType == typeof(string):
                        var textField = new TextField();
                        textField.label = propertyInfo.Name;
                        textField.RegisterValueChangedCallback(evt =>
                        {
                            ChangeValue(evt, propertyInfo);
                        });
                        _drawer = textField;
                        break;
                    default:
                        _drawer = new Label($"Unsupported type - {propertyInfo.PropertyType.Name}");
                        break;
                }

                _drawer.SetEnabled(propertyInfo.CanWrite);
            }
            else if (MemberInfo is FieldInfo fieldInfo)
            {
                _drawer = new Label(fieldInfo.Name);
                switch (fieldInfo)
                {
                    case var _ when fieldInfo.FieldType == typeof(bool):
                        var toggle = new Toggle();
                        toggle.label = fieldInfo.Name;
                        toggle.RegisterValueChangedCallback(evt =>
                        {
                            ChangeValue(evt, fieldInfo);
                        });
                        _drawer = toggle;
                        break;
                    case var _ when fieldInfo.FieldType == typeof(int):
                        var intField = new IntegerField();
                        intField.label = fieldInfo.Name;
                        intField.RegisterValueChangedCallback(evt =>
                        {
                            ChangeValue(evt, fieldInfo);
                        });
                        _drawer = intField;
                        break;
                    case var _ when fieldInfo.FieldType == typeof(float):
                        var floatField = new FloatField();
                        floatField.label = fieldInfo.Name;
                        floatField.RegisterValueChangedCallback(evt =>
                        {
                            ChangeValue(evt, fieldInfo);
                        });
                        _drawer = floatField;
                        break;
                    case var _ when fieldInfo.FieldType == typeof(string):
                        var textField = new TextField();
                        textField.label = fieldInfo.Name;
                        textField.RegisterValueChangedCallback(evt =>
                        {
                            ChangeValue(evt, fieldInfo);
                        });
                        _drawer = textField;
                        break;
                    default:
                        _drawer = new Label($"Unsupported type - {fieldInfo.FieldType.Name}");
                        break;
                }
            }
            
            // check if readonly
            if (MemberInfo.GetCustomAttribute<ReadOnlyAttribute>() != null)
            {
                _drawer.SetEnabled(false);
            }
        
            return _drawer;
        }

        public override void OnEnable()
        {
            EditorApplication.update += OnEditorUpdate;
        }
        
        public override void OnDisable()
        {
            EditorApplication.update -= OnEditorUpdate;
        }

        private void OnEditorUpdate()
        {
            if(MemberInfo is PropertyInfo propertyInfo)
                _value = propertyInfo.GetValue(Target);
            else if (MemberInfo is FieldInfo fieldInfo)
                _value = fieldInfo.GetValue(Target);
            
            switch (_drawer)
            {
                case Toggle toggle:
                    toggle.SetValueWithoutNotify((bool) _value);
                    break;
                case IntegerField intField:
                    intField.SetValueWithoutNotify((int) _value);
                    break;
                case FloatField floatField:
                    floatField.SetValueWithoutNotify((float) _value);
                    break;
                case TextField textField:
                    textField.SetValueWithoutNotify((string) _value);
                    break;
            }
        }

        private void ChangeValue<T>(ChangeEvent<T> evt, PropertyInfo propertyInfo)
        {
            _value = evt.newValue;
            if (propertyInfo.CanWrite)
            {
                propertyInfo.SetValue(Target, _value);
            }
        }
        
        private void ChangeValue<T>(ChangeEvent<T> evt, FieldInfo fieldInfo)
        {
            _value = evt.newValue;
            fieldInfo.SetValue(Target, _value);
        }
    }
}