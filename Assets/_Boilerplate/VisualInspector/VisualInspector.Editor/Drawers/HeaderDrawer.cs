using System.Drawing;
using UnityEngine.UIElements;
using VisualInspector.Editor.Core;
using FontStyle = UnityEngine.FontStyle;

namespace VisualInspector.Editor.Drawers
{
    [VisualDrawerTarget(typeof(HeaderAttribute))]
    public class HeaderDrawer : VisualDrawer
    {
        
        
        public override VisualElement CreateInspectorGUI(InspectorData inspectorData)
        {
            if (TargetVisualElement == null) 
                return null;

            var header = new Label
            {
                style =
                {
                    paddingTop = 2,
                    paddingLeft = 3.5f,
                    paddingBottom = 3,
                    fontSize = 18,
                    unityFontStyleAndWeight = new StyleEnum<FontStyle>(FontStyle.Bold)
                },
                text = ((HeaderAttribute)Attribute).Header
            };
            TargetVisualElement.Insert(0, header);
            return null;
        }
    }
}