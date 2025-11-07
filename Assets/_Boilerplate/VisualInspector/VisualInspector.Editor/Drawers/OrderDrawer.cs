using UnityEngine.UIElements;
using VisualInspector.Editor.Core;

namespace VisualInspector.Editor.Drawers
{
    [VisualDrawerTarget(typeof(OrderAttribute))]
    public class OrderDrawer : VisualDrawer
    {
        public override VisualElement CreateInspectorGUI(InspectorData inspectorData)
        {
            var orderAttribute = (OrderAttribute)Attribute;
            TargetVisualElement?.AddToClassList($"order-[{orderAttribute.Order}]");
            return null;
        }
    }
}