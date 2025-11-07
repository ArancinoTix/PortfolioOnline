using System;

namespace VisualInspector.Editor.Core
{
    /// <summary>
    ///     A information for <see cref="VisualDrawer"/> what attributes it should target for drawing custom inspector.
    /// </summary>
    public class VisualDrawerTargetAttribute : Attribute
    {
        public VisualDrawerTargetAttribute(Type targetType)
        {
            TargetType = targetType;
        }

        public Type TargetType { get; }
    }
}