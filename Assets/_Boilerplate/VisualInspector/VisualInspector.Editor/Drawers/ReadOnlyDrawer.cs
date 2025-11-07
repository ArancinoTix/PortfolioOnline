using UnityEngine.UIElements;
using VisualInspector.Editor.Core;

namespace VisualInspector.Editor.Drawers
{
    /// <summary>
    ///     Makes the field read-only in the inspector.
    /// </summary>
    [VisualDrawerTarget(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : VisualDrawer
    {
        public override VisualElement CreateInspectorGUI(InspectorData inspectorData)
        {
            TargetVisualElement?.SetEnabled(false);
            return null;
        }
    }
}