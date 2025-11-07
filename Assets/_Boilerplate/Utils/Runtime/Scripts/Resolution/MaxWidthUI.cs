using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VisualInspector;

namespace U9.Utils.Resolution
{
    public class MaxWidthUI : MonoBehaviour
    {
        [MessageBox("Max Width will ensure that any nexted UI elements will not exceed the expected width." +
            "\n\nThe width is determined by the canvas scaler that is assigned to the ResolutionSetter prefab in the scene. " +
            "\n\nFailure to assign a scaler will have the UI default to a ratio of 1560/3377")]

        // Start is called before the first frame update
        void Start()
        {
            float expectedWidth = ResolutionSetter.expectedWidth;
            float expectedRatio = ResolutionSetter.expectedRatio;
            float trueRatio = ResolutionSetter.ratio;

            if (trueRatio > expectedRatio)
            {
                RectTransform rt = (RectTransform)transform;
                Vector2 sd = rt.sizeDelta;
                sd.x = expectedWidth - (expectedWidth / expectedRatio * trueRatio);
                rt.sizeDelta = sd;
            }
        }
    }
}