namespace VisualInspector
{
    /// <summary>
    ///     Attribute to show a range slider in the inspector.
    /// </summary>
    public class RangeAttribute : VisualAttribute
    {
        public RangeAttribute(float min, float max)
        {
            Min = min;
            Max = max;
        }

        /// <summary>
        ///     Min value of the range.
        /// </summary>
        public float Min { get; private set; }
        
        /// <summary>
        ///     Max value of the range.
        /// </summary>
        public float Max { get; private set; }
        
        /// <summary>
        ///     Show the input field.
        /// </summary>
        public bool ShowInputField { get; set; } = true;
    }
}