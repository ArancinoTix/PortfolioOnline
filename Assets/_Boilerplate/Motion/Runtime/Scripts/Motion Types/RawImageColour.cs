using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace U9.Motion
{
    public class RawImageColour : TypedMotion<RawImage, Color>
    {
        public override void PlayFrom(bool backwards = false, float startTime = 0.0f)
        {
            if (StartAtCurrent && CurveUsage == CurveUsage.NON_DIRECT)
                StartAt = Target.color;

            base.PlayFrom(backwards, startTime);
        }

        /// <summary>
        /// Set target at specified time (0-1)
        /// </summary>
        /// <param name="time"></param>
        public override void SetTarget(float time)
        {
            Target.color = Color.Lerp(StartAt, EndAt, Curve.Evaluate(time));
        }
    }
}