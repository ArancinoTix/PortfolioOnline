using UnityEngine;
using VisualInspector;
using HeaderAttribute = VisualInspector.HeaderAttribute;
using RangeAttribute = UnityEngine.RangeAttribute;

namespace U9.ProgressTransition
{
    public class CanvasFadeProgressTransition : BaseProgressTransition
    {
        [MessageBox("The from value should reflect the \"Displayed\" state." +
              "\nThe to value should reflect the \"Hidden\" state.")]
        [HeaderAttribute("State Settings")]
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] [Range(0, 1)] private float _fromFade =0;
        [Separator] [SerializeField] [Range(0, 1)] private float _toFade =1;

        protected override void ApplyProgress(float progress)
        {
            _canvasGroup.alpha = Mathf.Lerp(_fromFade, _toFade, progress);
        }

        protected override void SetFromValuesInternal()
        {
            _fromFade = _canvasGroup.alpha;
        }

        protected override void SetToValuesInternal()
        {
            _toFade = _canvasGroup.alpha;
        }
        protected override void Reset()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            base.Reset();
        }
    }
}
