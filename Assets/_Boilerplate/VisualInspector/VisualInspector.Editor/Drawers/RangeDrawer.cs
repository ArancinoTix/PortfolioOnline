using System.Reflection;
using Humanizer;
using UnityEngine.UIElements;
using VisualInspector.Editor.Core;

namespace VisualInspector.Editor.Drawers
{
    /// <summary>
    ///     Drawer for slider properties.
    /// </summary>
    [VisualDrawerTarget(typeof(RangeAttribute))]
    public class RangeDrawer : VisualDrawer
    {
        public override VisualElement CreateInspectorGUI(InspectorData inspectorData)
        {
            var rangeAttribute = (RangeAttribute)Attribute;

            switch (MemberInfo)
            {
                case PropertyInfo propertyInfo when propertyInfo.PropertyType == typeof(int):
                {
                    var range = SliderInt(rangeAttribute, propertyInfo);
                    return range;
                }
                case PropertyInfo propertyInfo when propertyInfo.PropertyType == typeof(float):
                {
                    var range = Slider(rangeAttribute, propertyInfo);
                    return range;
                }
                case FieldInfo fieldInfo when fieldInfo.FieldType == typeof(int):
                {
                    var range = SliderInt(rangeAttribute, fieldInfo);
                    return range;
                }
                case FieldInfo fieldInfo when fieldInfo.FieldType == typeof(float):
                {
                    var range = Slider(rangeAttribute, fieldInfo);
                    return range;
                }
                default:
                    return null;
            }
        }

        /// <summary>
        ///     Create a slider for a float property.
        /// </summary>
        /// <param name="rangeAttribute">Attribute to control the slider</param>
        /// <param name="memberInfo">Field or Property info to control the values in the slider as dynamic</param>
        /// <returns>Returns Slider</returns>
        private Slider Slider(RangeAttribute rangeAttribute, dynamic memberInfo) 
        {
            var range = new Slider(rangeAttribute.Min, rangeAttribute.Max)
            {
                label = (memberInfo.Name as string).Humanize(),
                value = memberInfo.GetValue(Target) as float? ?? 0,
                showInputField = rangeAttribute.ShowInputField
            };
            range.RegisterValueChangedCallback(evt => memberInfo.SetValue(Target, evt.newValue));
            if (TargetVisualElement != null) TargetVisualElement.style.display = DisplayStyle.None;
            return range;
        }
        
        /// <summary>
        ///     Create a slider for an int property.
        /// </summary>
        /// <param name="rangeAttribute">Attribute to control the slider</param>
        /// <param name="memberInfo">Field or Property info to control the values in the slider as dynamic</param>
        /// <returns>Returns SliderInt</returns>
        private SliderInt SliderInt(RangeAttribute rangeAttribute, dynamic memberInfo) 
        {
            var range = new SliderInt((int)rangeAttribute.Min, (int)rangeAttribute.Max)
            {
                label = (memberInfo.Name as string).Humanize(),
                value = memberInfo.GetValue(Target) as int? ?? 0,
                showInputField = rangeAttribute.ShowInputField
            };
            range.RegisterValueChangedCallback(evt => memberInfo.SetValue(Target, evt.newValue));
            if (TargetVisualElement != null) TargetVisualElement.style.display = DisplayStyle.None;
            return range;
        }
    }
}