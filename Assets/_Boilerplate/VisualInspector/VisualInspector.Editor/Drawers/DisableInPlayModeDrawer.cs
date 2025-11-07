using UnityEngine;
using UnityEngine.UIElements;
using VisualInspector.Editor.Core;

namespace VisualInspector.Editor.Drawers
{
    /// <summary>
    ///     Hide the drawer during edit mode
    /// </summary>
    [VisualDrawerTarget(typeof(DisableInPlayModeAttribute))]
    public class DisableInPlayModeDrawer : VisualDrawer
    {
        public override VisualElement CreateInspectorGUI(InspectorData inspectorData)
        {
            if(TargetVisualElement == null)
                return null;
            TargetVisualElement.SetEnabled(!Application.isPlaying);
            return null;
        }
    }
}