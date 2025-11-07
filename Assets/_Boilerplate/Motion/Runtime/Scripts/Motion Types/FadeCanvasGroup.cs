using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace U9.Motion
{
    public class FadeCanvasGroup : TypedMotion<CanvasGroup, float>
    {
        [Tooltip("Adjust the interactability of the canvas group using AlphaBeforeInteractable value, if less than, then not interactable")]
        public bool AdjustInteractability = true;
        public bool AdjustRaycastBlocking = true;
        public float AlphaBeforeInteractable = 1.0f;

        public override void PlayFrom(bool backwards = false, float startTime = 0.0f)
        {
            if (StartAtCurrent && CurveUsage == CurveUsage.NON_DIRECT)
                StartAt = Target.alpha;

            base.PlayFrom(backwards, startTime);
        }

        /// <summary>
        /// Set target at specified time (0-1)
        /// </summary>
        /// <param name="time"></param>
        public override void SetTarget(float time)
        {
            if (CurveUsage == CurveUsage.DIRECT)
                Target.alpha = Curve.Evaluate(time);
            else if (CurveUsage == CurveUsage.DIRECT_WITH_SCALE)
                Target.alpha = Curve.Evaluate(time) * Scale;
            else
                Target.alpha = Mathf.Lerp(StartAt, EndAt, Curve.Evaluate(time));

            if (AdjustInteractability)
            {
                Target.interactable = Target.alpha >= AlphaBeforeInteractable;

                if (AdjustRaycastBlocking)
                    Target.blocksRaycasts = Target.interactable;
            }
        }
    }
}