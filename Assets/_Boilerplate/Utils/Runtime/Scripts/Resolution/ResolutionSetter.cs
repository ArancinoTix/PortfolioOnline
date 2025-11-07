using UnityEngine;
using UnityEngine.UI;
using VisualInspector;
using RangeAttribute = UnityEngine.RangeAttribute;

namespace U9.Utils.Resolution
{
    public class ResolutionSetter : MonoBehaviour
    {
        [MessageBox("Resizes the rendering resolution to the given percentage." +
            "\nUseful if you need to concerve processing power." +
            "\n\n[IMPORTANT] - You must use ResolutionSetter.width/height instead of Screen.width/height if used.")]
        [SerializeField] [Range(0.25f, 1)] float _resolutionPercentage = 1f;
        [SerializeField] [Range(0.25f, 1)] float _minPercentange =.5f;
        [SerializeField] [Range(0.25f, 1)] float _maxPercentange = 1f;
        [SerializeField] bool _resizeOnAwake = false;
        
        public static int width;
        public static int height;
        public static float ratio;
        public static float expectedRatio = 1560f / 3377f;
        public static float expectedWidth = 1560f;
        public CanvasScaler _mainCanvasScaler;
        private static bool Resized = false;

        private static float originalWidth;
        private static float originalHeight;

        public static float minScale = .5f;
        public static float maxScale = 1f;

        // Start is called before the first frame update
        void Awake()
        {
            minScale = _minPercentange;
            maxScale = _maxPercentange;

            if (!Resized && _resizeOnAwake)
            {
                Resize(_resolutionPercentage);
            }

            if (_mainCanvasScaler != null)
            {
                expectedWidth = _mainCanvasScaler.referenceResolution.x;
                expectedRatio = _mainCanvasScaler.referenceResolution.x / _mainCanvasScaler.referenceResolution.y;
            }
        }

        public static void Resize(float scale)
        {
            scale = Mathf.Clamp(scale,minScale, maxScale);

            if (!Resized)
            {
                Resized = true;
                originalWidth = Screen.width;
                originalHeight = Screen.height;
            }

            var w = originalWidth * scale;
            var h = originalHeight * scale;

            ratio = w / h;
            width = Mathf.RoundToInt(w);
            height = Mathf.RoundToInt(h);

            Debug.Log("Resizing from: " + Screen.width + " x " + Screen.height);
            Debug.Log("Resizing to: " + width + " x " + height);

            Screen.SetResolution(width, height, true);
        }
    }
}
