using UnityEngine;
using VisualInspector;
using HeaderAttribute = VisualInspector.HeaderAttribute;

namespace U9.ProgressTransition
{
    public class RotateProgressTransition : BaseProgressTransition
    {
        [MessageBox("The from value should reflect the \"Displayed\" state." +
            "\nThe to value should reflect the \"Hidden\" state.")]
        [HeaderAttribute("State Settings")]
        [SerializeField] private Vector3 _fromRotation;
        [Separator] [SerializeField] private Vector3 _toRotation;

        protected override void ApplyProgress(float progress)
        {
            transform.localEulerAngles = Vector3.Lerp(_fromRotation, _toRotation, progress);
        }

        protected override void SetFromValuesInternal()
        {
            _fromRotation = transform.localEulerAngles;
        }

        protected override void SetToValuesInternal()
        {
            _toRotation = transform.localEulerAngles;
        }
    }
}
