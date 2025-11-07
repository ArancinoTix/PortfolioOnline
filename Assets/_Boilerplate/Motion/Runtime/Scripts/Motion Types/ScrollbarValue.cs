using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace U9.Motion
{
    public class ScrollbarValue : TypedMotion<Scrollbar, float>
    {
        public override void PlayFrom(bool backwards = false, float startTime = 0.0f)
        {
            if (StartAtCurrent && CurveUsage == CurveUsage.NON_DIRECT)
                StartAt = Target.value;

            base.PlayFrom(backwards, startTime);
        }

        public override void SetTarget(float time)
        {
            Target.value = time;
        }
    }
}