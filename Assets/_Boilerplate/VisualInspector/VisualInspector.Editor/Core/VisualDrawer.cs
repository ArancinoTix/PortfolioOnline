using System;
using System.Reflection;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace VisualInspector.Editor.Core
{
    /// <summary>
    ///     Fancy equivalent of <see cref="PropertyDrawer"/> but better!
    ///
    ///     Base class to derive custom property drawers from. Use this to create custom drawers
    ///     for your own classes or for script variables with custom VisualAttribute.
    /// </summary>
    public abstract class VisualDrawer : IDisposable
    {
        private bool _isDisposed;
        
        /// <summary>
        ///     The object this drawer is drawing.
        /// </summary>
        public Object Target { get; set; }
        
        /// <summary>
        ///     The property this drawer is drawing.
        /// </summary>
        public MemberInfo MemberInfo { get; set; }

        /// <summary>
        ///     The attribute this drawer is drawing.
        /// </summary>
        public VisualAttribute Attribute { get; set; }
        
        /// <summary>
        ///     The attribute this drawer is targeting is possible.
        /// </summary>
        [CanBeNull]
        public VisualElement TargetVisualElement { get; set; }
        
        /// <summary>
        ///     Entry point to the drawing, see <see cref="VisualElement"/> form more info.
        /// </summary>
        /// <param name="inspectorData">All data about the object</param>
        /// <returns>Root inspector editor</returns>
        public abstract VisualElement CreateInspectorGUI(InspectorData inspectorData);
        
        /// <summary>
        ///     Called when the drawer is enabled.
        /// </summary>
        public virtual void OnEnable(){}
        
        /// <summary>
        ///     Called when the drawer is disabled.
        /// </summary>
        public virtual void OnDisable(){}
        
        /// <summary>
        ///     Called when the drawer is destroyed.
        /// </summary>
        public virtual void OnDestroy(){}
        
        public void Dispose()
        {
            if (_isDisposed) return;
            OnDisable();
            OnDestroy();
            _isDisposed = true;
        }
    }
}