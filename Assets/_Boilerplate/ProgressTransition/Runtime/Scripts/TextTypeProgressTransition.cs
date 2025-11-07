using UnityEngine;
using TMPro;
using VisualInspector;
using HeaderAttribute = VisualInspector.HeaderAttribute;

namespace U9.ProgressTransition
{
    public class TextTypeProgressTransition : BaseProgressTransition
    {
        [HeaderAttribute("State Settings")]
        [Separator] [SerializeField] private TMP_Text _text;

        protected override void ApplyProgress(float progress)
        {
            if (progress >= 1)
            {
                _text.maxVisibleCharacters = 999999;
            }
            else
            {
                float limit = _text.text.Length;
                _text.maxVisibleCharacters = Mathf.CeilToInt(limit * progress);
            }
        }

        protected override void SetFromValuesInternal()
        {
        }

        protected override void SetToValuesInternal()
        {
        }
        protected override void Reset()
        {
            _text = GetComponent<TMP_Text>();
            base.Reset();
        }
    }
}