using UnityEngine;

namespace VisualInspector
{
    /// <summary>
    ///     Shows the field or property in the inspector, without serializing it.
    ///     If field is serialized with <see cref="SerializeField"/> attribute, it will be serialized.
    /// </summary>
    public class ShowInInspectorAttribute : VisualAttribute
    {
        
    }
}