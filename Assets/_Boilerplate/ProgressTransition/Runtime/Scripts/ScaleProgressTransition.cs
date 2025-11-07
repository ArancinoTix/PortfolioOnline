using UnityEngine;
using VisualInspector;
using HeaderAttribute = VisualInspector.HeaderAttribute;

namespace U9.ProgressTransition
{
    public class ScaleProgressTransition : BaseProgressTransition
    {
        [MessageBox("The from value should reflect the \"Displayed\" state." +
               "\nThe to value should reflect the \"Hidden\" state.")]
        [HeaderAttribute("State Settings")]
        [SerializeField] private Vector3 _fromScale;
        [Separator] [SerializeField] private Vector3 _toScale;

        protected override void ApplyProgress(float progress)
        {
            transform.localScale = Vector3.Lerp(_fromScale, _toScale, progress);
        }

        protected override void SetFromValuesInternal()
        {
            _fromScale = transform.localScale;
        }

        protected override void SetToValuesInternal()
        {
            _toScale = transform.localScale;
        }
    }
}
