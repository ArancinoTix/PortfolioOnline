using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace U9.Motion
{
    public class ImageFill : TypedMotion<Image, float>
    {
        public override void PlayFrom(bool backwards = false, float startTime = 0.0f)
        {
            if (StartAtCurrent && CurveUsage == CurveUsage.NON_DIRECT)
                StartAt = Target.fillAmount;

            base.PlayFrom(backwards, startTime);
        }

        /// <summary>
        /// Set target at specified time (0-1)
        /// </summary>
        /// <param name="time"></param>
        public override void SetTarget(float time)
        {
            Target.fillAmount = Mathf.Lerp(StartAt, EndAt, Curve.Evaluate(time));
        }
    }
}