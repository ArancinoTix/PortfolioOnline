using UnityEngine;
using VisualInspector;
using HeaderAttribute = VisualInspector.HeaderAttribute;

namespace U9.ProgressTransition
{
    public class MoveProgressTransition : BaseProgressTransition
    {
        [MessageBox("The from value should reflect the \"Displayed\" state." +
            "\nThe to value should reflect the \"Hidden\" state.")]
        [HeaderAttribute("State Settings")]        
        [SerializeField] private Vector3 _fromPosition;
        [Separator][SerializeField] private Vector3 _toPosition;

        protected override void ApplyProgress(float progress)
        {
            transform.localPosition = Vector3.Lerp(_fromPosition, _toPosition, progress);
        }

        protected override void SetFromValuesInternal()
        {
            _fromPosition = transform.localPosition;
        }

        protected override void SetToValuesInternal()
        {
            _toPosition = transform.localPosition;
        }
    }
}
