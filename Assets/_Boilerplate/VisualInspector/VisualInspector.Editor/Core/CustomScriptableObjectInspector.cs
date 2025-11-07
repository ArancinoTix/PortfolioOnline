using UnityEditor;
using UnityEngine;

namespace VisualInspector.Editor.Core
{
    [CustomEditor(typeof(ScriptableObject), true, isFallback = true)]
    public class CustomScriptableObjectInspector : CustomMonoBehaviourInspector
    {
        
    }
}