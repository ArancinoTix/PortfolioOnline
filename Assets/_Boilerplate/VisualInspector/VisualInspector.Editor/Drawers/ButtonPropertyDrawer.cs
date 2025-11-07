using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Humanizer;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using VisualInspector.Editor.Core;

namespace VisualInspector.Editor.Drawers
{
    /// <summary>
    ///     Buttons are used on functions, and allows for clickable buttons in the inspector.
    /// </summary>
    [VisualDrawerTarget(typeof(ButtonAttribute))]
    public class ButtonPropertyDrawer : VisualDrawer
    {
        private MethodInfo _methodInfo;
        private TextField _returnField;
        private InspectorData _inspectorData;

        /// <summary>
        ///     Create a logic for Button inspector GUI, that allows you to call a method from the target object in inspector
        ///     by a button click.
        /// </summary>
        /// <param name="inspectorData"></param>
        /// <returns></returns>
        public override VisualElement CreateInspectorGUI(InspectorData inspectorData)
        {
            // TODO Move this to some common handler
            if (!Application.isPlaying && MemberInfo.GetCustomAttribute<HideInEditModeAttribute>() != null)
                return null;

            Initialize(inspectorData);

            var container = new Box();

            // TODO Move this to some common handler
            if (
                !Application.isPlaying && MemberInfo.GetCustomAttribute<DisableInEditModeAttribute>() != null
                ||
                Application.isPlaying && MemberInfo.GetCustomAttribute<DisableInPlayModeAttribute>() != null
                )
                container.SetEnabled(false);

            // TODO Move this to some common handler
            var orderAttribute = MemberInfo.GetCustomAttribute<OrderAttribute>();
            if (orderAttribute != null)
                container.AddToClassList($"order-[{orderAttribute.Order}]");

            var parameters = _methodInfo.GetParameters();
            if (parameters.Length == 0 && _methodInfo.ReturnType == typeof(void))
                CreateNoParamButton(container, $"Invoke {SimplifyMethodName(_methodInfo)}");
            else
            {
                container.style.paddingBottom = new StyleLength(4);
                container.style.paddingTop = new StyleLength(4);
                container.style.paddingLeft = new StyleLength(4);
                container.style.paddingRight = new StyleLength(4);
                container.style.marginBottom = new StyleLength(2);
                container.style.marginTop = new StyleLength(2);
                container.style.marginLeft = new StyleLength(2);
                container.style.marginRight = new StyleLength(2);
                container.style.borderBottomColor = new StyleColor(Color.black);
                container.style.borderTopColor = new StyleColor(Color.black);
                container.style.borderLeftColor = new StyleColor(Color.black);
                container.style.borderRightColor = new StyleColor(Color.black);

                var header = new Label($"Method: {SimplifyMethodName(_methodInfo)}")
                {
                    style = { unityFontStyleAndWeight = new StyleEnum<FontStyle>(FontStyle.Bold) }
                };
                container.Add(header);

                if (parameters.Length == 0) CreateNoParamButton(container, null);
                else CreateParamButton(container, parameters);

                CreateReturnField(container);
            }

            return container;
        }

        /// <summary>
        ///     Creates a button that can have parameters
        /// </summary>
        /// <param name="container"></param>
        /// <param name="parameters"></param>
        private void CreateParamButton(Box container, ParameterInfo[] parameters)
        {
            var inputFields = new List<VisualElement>();

            foreach (var paramInfo in parameters)
            {
                ParameterTypeSwitch(paramInfo, inputFields);
            }

            foreach (var field in inputFields)
            {
                container.Add(field);
            }

            var button = new Button(() =>
            {
                var paramValues = new object[parameters.Length];
                for (var i = 0; i < parameters.Length; i++)
                {
                    var paramInfo = parameters[i];
                    var paramValue = ((dynamic)inputFields[i]).value;
                    paramValues[i] = paramValue;
                }

                object returnValue;
                try
                {
                    returnValue = _methodInfo.Invoke(Target, paramValues);
                    if (_returnField != null) _returnField.value = returnValue.ToString();
                }
                catch (Exception e)
                {
                    if (_returnField != null) _returnField.value = e.Message;
                    Debug.LogError(e);
                }
            });
            button.text = "Invoke";
            container.Add(button);
        }

        /// <summary>
        ///     Switch on parameter type to create the correct input field
        /// </summary>
        /// <param name="paramInfo"></param>
        /// <param name="inputFields"></param>
        private void ParameterTypeSwitch(ParameterInfo paramInfo, List<VisualElement> inputFields)
        {
            var attributes = paramInfo.GetCustomAttributes<VisualAttribute>(true);
            var rangeAttribute = (RangeAttribute)attributes.FirstOrDefault(a => a is RangeAttribute);
            switch (paramInfo.ParameterType)
            {
                // First check for custom attributes
                case { } t when attributes.FirstOrDefault(a => a is DropdownAttribute) != null:
                    var dropdownField = new DropdownField(paramInfo.Name.Humanize());
                    var dropdownAttribute = (DropdownAttribute)attributes.First(a => a is DropdownAttribute);
                    dropdownField.choices =
                        (dropdownAttribute.IsDynamic
                            ? new Evaluator().Evaluate<string[]>(_inspectorData, dropdownAttribute.Evaluate)
                            : dropdownAttribute.Options!).ToList();
                    dropdownField.value = dropdownField.choices[0];
                    //TODO Add evaluate on open to dropdown
                    inputFields.Add(dropdownField);
                    break;
                // Then check for built in types
                case { } t when t == typeof(int):
                    if (rangeAttribute != null)
                    {
                        var intField = new SliderInt((int)rangeAttribute.Min, (int)rangeAttribute.Max)
                        {
                            label = paramInfo.Name.Humanize(),
                            showInputField = rangeAttribute.ShowInputField
                        };
                        inputFields.Add(intField);
                    }
                    else
                    {
                        var intField = new IntegerField(paramInfo.Name.Humanize());
                        inputFields.Add(intField);
                    }

                    break;
                case { } t when t == typeof(float):
                    if (rangeAttribute != null)
                    {
                        var floatField = new Slider(rangeAttribute.Min, rangeAttribute.Max)
                        {
                            label = paramInfo.Name.Humanize(),
                            showInputField = rangeAttribute.ShowInputField
                        };
                        inputFields.Add(floatField);
                    }
                    else
                    {
                        var floatField = new FloatField(paramInfo.Name.Humanize());
                        inputFields.Add(floatField);
                    }

                    break;
                case { } t when t == typeof(string):
                    var stringField = new TextField(paramInfo.Name.Humanize());
                    inputFields.Add(stringField);
                    break;
                case { } t when t == typeof(bool):
                    var boolField = new Toggle(paramInfo.Name.Humanize());
                    inputFields.Add(boolField);
                    break;
                case { } t when t == typeof(Vector2):
                    var vector2Field = new Vector2Field(paramInfo.Name.Humanize());
                    inputFields.Add(vector2Field);
                    break;
                case { } t when t == typeof(Vector3):
                    var vector3Field = new Vector3Field(paramInfo.Name.Humanize());
                    inputFields.Add(vector3Field);
                    break;
                case { } t when t == typeof(Vector4):
                    var vector4Field = new Vector4Field(paramInfo.Name.Humanize());
                    inputFields.Add(vector4Field);
                    break;
                case { } t when t == typeof(Color):
                    var colorField = new ColorField(paramInfo.Name.Humanize());
                    inputFields.Add(colorField);
                    break;
                case { } t when t == typeof(AnimationCurve):
                    var curveField = new CurveField(paramInfo.Name.Humanize());
                    inputFields.Add(curveField);
                    break;
                case { } t when t == typeof(Gradient):
                    var gradientField = new GradientField(paramInfo.Name.Humanize());
                    inputFields.Add(gradientField);
                    break;
                case { IsEnum: true }:
                    var enumField = new EnumField(paramInfo.Name.Humanize());
                    // add enum values
                    var values = Enum.GetValues(paramInfo.ParameterType);
                    enumField.Init(values.GetValue(0) as Enum);
                    inputFields.Add(enumField);
                    break;
                default:
                    var objectField = new ObjectField(paramInfo.Name.Humanize());
                    objectField.objectType = paramInfo.ParameterType;
                    objectField.allowSceneObjects = true;
                    inputFields.Add(objectField);
                    break;
                /*default:
                    Debug.LogError($"Parameter type {paramInfo.ParameterType} is not supported");
                    break;*/
            }
        }

        /// <summary>
        ///     Create a button that has no parameters
        /// </summary>
        /// <param name="container"></param>
        /// <param name="label">Custom label for the no param button</param>
        private void CreateNoParamButton(Box container, [CanBeNull] string label)
        {
            var button = new Button();
            button.text = label ?? "Invoke";
            button.name = SimplifyMethodName(_methodInfo);
            button.clicked += () =>
            {
                try
                {
                    var result = _methodInfo?.Invoke(Target, null);
                    switch (result)
                    {
                        case null:
                            return;
                        case IEnumerable enumerable:
                        {
                            _returnField.value = string.Join("\n", enumerable.Cast<object>());
                            break;
                        }
                        default:
                            _returnField.value = result.ToString();
                            break;
                    }
                }
                catch (Exception e)
                {
                    if (_returnField != null)
                        _returnField.value = e.Message;
                    Debug.LogError(e);
                }
            };
            container.Add(button);
        }


        /// <summary>
        ///     Creates a text field to display the return value of the button.
        /// </summary>
        /// <param name="container"></param>
        private void CreateReturnField(VisualElement container)
        {
            if (_methodInfo.ReturnType != typeof(void))
            {
                // Horizontal container
                var horizontal = new VisualElement
                {
                    style =
                    {
                        flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row),
                        width = new StyleLength(Length.Percent(100))
                    }
                };

                // Label
                var label = new Label("Return Value: ");
                horizontal.Add(label);

                // Button with a clipboard icon to copy return value
                var copyButton = new Button();
                var copyIcon = EditorGUIUtility.IconContent("Clipboard");
                var image = new Image();
                image.image = copyIcon.image;
                copyButton.Add(image);
                copyButton.clicked += () =>
                {
                    if (_returnField != null)
                        EditorGUIUtility.systemCopyBuffer = _returnField.text;
                };
                horizontal.Add(copyButton);

                // Text field with result
                _returnField = new TextField();
                _returnField.style.flexGrow = new StyleFloat(1);
                _returnField.isReadOnly = true;
                horizontal.Add(_returnField);

                // End
                container.Add(horizontal);
            }
        }

        private void Initialize(InspectorData inspectorData)
        {
            _methodInfo = MemberInfo as MethodInfo;
            _inspectorData = inspectorData;
        }

        /// <summary>
        ///     Creates a simplified name for the method
        /// </summary>
        /// <example>
        ///     <code>
        ///     namespace MyNamespace
        ///     {
        ///         public class MyClass
        ///         {
        ///             public Foo MyMethod(Boo a, int b)
        ///             {   
        ///             }
        ///     }
        /// 
        ///     // MemberInfo.ToString() = MyNamespace.Foo MyMethod(MyNamespace.Boo, System.Int32)
        ///     // SimplifyMethodName: Foo MyMethod(Boo, int)
        ///     </code>
        /// </example>
        /// <param name="methodInfo">Method info</param>
        /// <returns>Simplified name</returns>
        private static string SimplifyMethodName(MethodInfo methodInfo)
        {
            // Get the method name without the namespace
            var methodName = methodInfo.Name;
            // Get the return type without the namespace
            var returnType = methodInfo.ReturnType.Name;
            // Get the parameters without the namespace
            var parameters = string.Join(", ",
                methodInfo.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"));
            // Get the generic type arguments without the namespace
            var genericArgs = string.Join(", ", methodInfo.GetGenericArguments().Select(a => a.Name));
            if (!string.IsNullOrEmpty(genericArgs)) methodName += $"<{genericArgs}>";
            return $"{returnType} {methodName}({parameters})";
        }
    }
}