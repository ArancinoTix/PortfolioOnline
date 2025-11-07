using UnityEngine;
using VisualInspector;
using HeaderAttribute = VisualInspector.HeaderAttribute;

namespace U9.ProgressTransition
{
    public class RectDimensionsProgressTransition : BaseProgressTransition
    {
        [MessageBox("The from value should reflect the \"Displayed\" state." +
           "\nThe to value should reflect the \"Hidden\" state.")]
        [HeaderAttribute("State Settings")]
        [SerializeField] private Vector2 _fromDimensions;
        [Separator] [SerializeField] private Vector2 _toDimensions;

        protected override void ApplyProgress(float progress)
        {
            ((RectTransform)transform).sizeDelta = Vector2.Lerp(_fromDimensions, _toDimensions, progress);
        }

        protected override void SetFromValuesInternal()
        {
            _fromDimensions = ((RectTransform)transform).sizeDelta;
        }

        protected override void SetToValuesInternal()
        {
            _toDimensions = ((RectTransform)transform).sizeDelta;
        }
    }
}
