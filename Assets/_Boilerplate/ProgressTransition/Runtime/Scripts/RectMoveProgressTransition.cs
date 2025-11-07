using UnityEngine;
using VisualInspector;
using HeaderAttribute = VisualInspector.HeaderAttribute;

namespace U9.ProgressTransition
{
    public class RectMoveProgressTransition : BaseProgressTransition
    {
        [MessageBox("The from value should reflect the \"Displayed\" state." +
            "\nThe to value should reflect the \"Hidden\" state.")]
        [HeaderAttribute("State Settings")]
        [SerializeField] private Vector2 _fromPosition;
        [Separator] [SerializeField] private Vector2 _toPosition;

        protected override void ApplyProgress(float progress)
        {
            ((RectTransform)transform).anchoredPosition = Vector2.Lerp(_fromPosition, _toPosition, progress);
        }

        protected override void SetFromValuesInternal()
        {
            _fromPosition = ((RectTransform)transform).anchoredPosition;
        }

        protected override void SetToValuesInternal()
        {
            _toPosition = ((RectTransform)transform).anchoredPosition;
        }
    }
}
