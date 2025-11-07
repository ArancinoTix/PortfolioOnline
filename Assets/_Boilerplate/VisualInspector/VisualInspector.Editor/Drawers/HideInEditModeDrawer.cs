using UnityEngine;
using UnityEngine.UIElements;
using VisualInspector.Editor.Core;

namespace VisualInspector.Editor.Drawers
{
    /// <summary>
    ///     Hide the drawer during edit mode
    /// </summary>
    [VisualDrawerTarget(typeof(HideInEditModeAttribute))]
    public class HideInEditModeDrawer : VisualDrawer
    {
        public override VisualElement CreateInspectorGUI(InspectorData inspectorData)
        {
            if(TargetVisualElement == null)
                return null;
            if(Application.isPlaying) TargetVisualElement.style.display = DisplayStyle.Flex;
            else TargetVisualElement.style.display = DisplayStyle.None;
            return null;
        }
    }
}