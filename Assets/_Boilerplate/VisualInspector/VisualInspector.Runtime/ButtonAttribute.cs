using System;

namespace VisualInspector
{
    /// <summary>
    ///     Buttons are used on functions, and allows for clickable buttons in the inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class ButtonAttribute : VisualAttribute
    {
        
    }
}