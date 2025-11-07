using System;

namespace VisualInspector
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class HeaderAttribute : VisualAttribute
    {
        public string Header { get; private set; }
        public HeaderAttribute(string header)
        {
            Header = header;
        }
    }
}