using UnityEngine;
using UnityEngine.UI;
using VisualInspector;
using HeaderAttribute = VisualInspector.HeaderAttribute;

namespace U9.ProgressTransition
{
    public class ImageSpriteProgressTransition : BaseProgressTransition
    {
        [MessageBox("The first sprite should reflect the \"Displayed\" state." +
              "\nThe last sprite should reflect the \"Hidden\" state.")]
        [HeaderAttribute("State Settings")]
        [SerializeField] private Image _image;
        [Separator] [SerializeField] private Sprite[] _sprites;

        protected override void ApplyProgress(float progress)
        {
            int spriteIndex = Mathf.FloorToInt((_sprites.Length-1)*progress);

            if (_image.sprite != _sprites[spriteIndex] && spriteIndex <_sprites.Length)
                _image.sprite = _sprites[spriteIndex];
        }

        protected override void SetFromValuesInternal()
        {
        }

        protected override void SetToValuesInternal()
        {
        }

        protected override void Reset()
        {
            _image = GetComponent<Image>();
            base.Reset();
        }
    }
}