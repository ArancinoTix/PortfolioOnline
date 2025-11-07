using UnityEngine;
using UnityEngine.UI;
using VisualInspector;
using HeaderAttribute = VisualInspector.HeaderAttribute;
using RangeAttribute = UnityEngine.RangeAttribute;

namespace U9.ProgressTransition
{
    public class ImageFadeProgressTransition : BaseProgressTransition
    {
        [MessageBox("The from value should reflect the \"Displayed\" state." +
            "\nThe to value should reflect the \"Hidden\" state.")]
        [HeaderAttribute("State Settings")] 
        [SerializeField] private Image _image;
        [SerializeField] [Range(0, 1)] private float _fromFade = 0;
        [Separator] [SerializeField] [Range(0, 1)] private float _toFade = 1;

        protected override void ApplyProgress(float progress)
        {
            Color c = _image.color;
            c.a = Mathf.Lerp(_fromFade, _toFade, progress);
            _image.color = c;
        }

        protected override void SetFromValuesInternal()
        {
            _fromFade = _image.color.a;
        }

        protected override void SetToValuesInternal()
        {
            _toFade = _image.color.a;
        }
        protected override void Reset()
        {
            _image = GetComponent<Image>();
            base.Reset();
        }
    }
}