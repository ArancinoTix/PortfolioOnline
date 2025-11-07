using UnityEngine;
using UnityEngine.UIElements;
using VisualInspector.Editor.Core;

namespace VisualInspector.Editor.Drawers
{
    [VisualDrawerTarget(typeof(SeparatorAttribute))]
    public class SeparatorDrawer : VisualDrawer
    {
        public override VisualElement CreateInspectorGUI(InspectorData inspectorData)
        {
            var separator = new VisualElement();
            separator.style.height = 1;
            separator.style.backgroundColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            separator.style.marginTop = 18;
            separator.style.marginBottom = 18;
            if(TargetVisualElement != null)
            {
                TargetVisualElement.Add(separator);
                return null;
            }
            else
            {
                return separator;
            }
        }
    }
}