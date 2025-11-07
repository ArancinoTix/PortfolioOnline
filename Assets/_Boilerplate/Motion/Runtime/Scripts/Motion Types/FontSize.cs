using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace U9.Motion
{
    public class FontSize : TypedMotion<TMP_Text, float>
    {
        public override void PlayFrom(bool backwards = false, float startTime = 0.0f)
        {
            if (StartAtCurrent && CurveUsage == CurveUsage.NON_DIRECT)
                StartAt = Target.fontSize;

            base.PlayFrom(backwards, startTime);
        }

        /// <summary>
        /// Set target at specified time (0-1)
        /// </summary>
        /// <param name="time"></param>
        public override void SetTarget(float time)
        {
            if (CurveUsage == CurveUsage.DIRECT)
                Target.fontSize = Curve.Evaluate(time);
            else if (CurveUsage == CurveUsage.DIRECT_WITH_SCALE)
                Target.fontSize = Curve.Evaluate(time) * Scale;
            else
                Target.fontSize = Mathf.Lerp(StartAt, EndAt, Curve.Evaluate(time));
        }
    }
}