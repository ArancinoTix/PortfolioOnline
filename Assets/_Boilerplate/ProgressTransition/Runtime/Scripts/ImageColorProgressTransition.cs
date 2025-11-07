using UnityEngine;
using UnityEngine.UI;
using VisualInspector;
using HeaderAttribute = VisualInspector.HeaderAttribute;

namespace U9.ProgressTransition
{
    public class ImageColorProgressTransition : BaseProgressTransition
    {
        [MessageBox("The from value should reflect the \"Displayed\" state." +
              "\nThe to value should reflect the \"Hidden\" state.")]
        [HeaderAttribute("State Settings")]
        [SerializeField] private Image _image;
        [SerializeField] private Color _fromColor = new Color(1,1,1,1);
        [Separator] [SerializeField] private Color _toColor = new Color(1, 1, 1, 1);

        protected override void ApplyProgress(float progress)
        {
            _image.color = Color.Lerp(_fromColor, _toColor, progress);
        }

        protected override void SetFromValuesInternal()
        {
            _fromColor = _image.color;
        }

        protected override void SetToValuesInternal()
        {
            _toColor = _image.color;
        }
        protected override void Reset()
        {
            _image = GetComponent<Image>();
            base.Reset();
        }
    }
}