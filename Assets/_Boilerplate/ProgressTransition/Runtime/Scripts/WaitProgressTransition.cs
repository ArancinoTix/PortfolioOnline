using UnityEngine;
using VisualInspector;
using HeaderAttribute = VisualInspector.HeaderAttribute;

namespace U9.ProgressTransition
{
    public class WaitProgressTransition : BaseProgressTransition
    {
#if UNITY_EDITOR
        [MessageBox("This this transition in conjunction with a sequence transition" +
            "\nto introduce a pause in the middle of a sequence.")]
        [HeaderAttribute("State Settings")]
        [Separator] [SerializeField] private bool _unusedIgnore;
#endif

        protected override void ApplyProgress(float progress)
        {
        }

        protected override void SetFromValuesInternal()
        {
        }

        protected override void SetToValuesInternal()
        {
        }
    }
}
