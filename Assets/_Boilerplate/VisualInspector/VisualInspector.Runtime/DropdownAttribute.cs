using JetBrains.Annotations;

namespace VisualInspector
{
    public class DropdownAttribute : VisualAttribute
    {
        [CanBeNull]
        public string Evaluate { get; }
        
        [CanBeNull]
        public string[] Options { get; }
        
        public bool IsDynamic => Evaluate != null;

        public DropdownAttribute(string[] options)
        {
            Options = options;
        }

        public DropdownAttribute(string evaluate)
        {
            Evaluate = evaluate;
        }
    }
}