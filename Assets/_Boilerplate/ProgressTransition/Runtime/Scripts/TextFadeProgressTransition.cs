using UnityEngine;
using TMPro;
using VisualInspector;
using HeaderAttribute = VisualInspector.HeaderAttribute;
using RangeAttribute = UnityEngine.RangeAttribute;

namespace U9.ProgressTransition
{
    public class TextFadeProgressTransition : BaseProgressTransition
    {
        [HeaderAttribute("State Settings")]
        [Separator] [SerializeField] private TMP_Text _text;
        [SerializeField] [Range(0, 1)] private float _fromFade = 0;
        [Separator] [SerializeField] [Range(0, 1)] private float _toFade = 1;

        protected override void ApplyProgress(float progress)
        {
            Color c = _text.color;
            c.a = Mathf.Lerp(_fromFade, _toFade, progress);
            _text.color = c;
        }

        protected override void SetFromValuesInternal()
        {
            _fromFade = _text.color.a;
        }

        protected override void SetToValuesInternal()
        {
            _toFade = _text.color.a;
        }

        protected override void Reset()
        {
            _text = GetComponent<TMP_Text>();
            base.Reset();
        }
    }
}