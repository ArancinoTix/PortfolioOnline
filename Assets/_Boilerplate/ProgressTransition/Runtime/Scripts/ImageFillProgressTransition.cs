using UnityEngine;
using UnityEngine.UI;
using VisualInspector;
using HeaderAttribute = VisualInspector.HeaderAttribute;
using RangeAttribute = UnityEngine.RangeAttribute;

namespace U9.ProgressTransition
{
    public class ImageFillProgressTransition : BaseProgressTransition
    {
        [MessageBox("The from value should reflect the \"Displayed\" state." +
            "\nThe to value should reflect the \"Hidden\" state.")]
        [HeaderAttribute("State Settings")]
        [SerializeField] private Image _image;
        [SerializeField] [Range(0, 1)] private float _fromFill=0;
        [Separator][SerializeField] [Range(0, 1)] private float _toFill=1;

        protected override void ApplyProgress(float progress)
        {
            _image.fillAmount = Mathf.Lerp(_fromFill, _toFill, progress);
        }

        protected override void SetFromValuesInternal()
        {
            _fromFill = _image.fillAmount;
        }

        protected override void SetToValuesInternal()
        {
            _toFill = _image.fillAmount;
        }
        protected override void Reset()
        {
            _image = GetComponent<Image>();
            base.Reset();
        }
    }
}