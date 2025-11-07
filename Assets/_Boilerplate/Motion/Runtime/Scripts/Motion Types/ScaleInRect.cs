using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace U9.Motion
{
    public class ScaleInRect : TwoAxisMotion<RectTransform, Vector2>
    {
        // These are what are actually used, before any motion the flags for X and Z are checked
        // Ones that are enabled copy StartAt and EndAt values, those not ticked are the current transforms value
        private Vector2 m_ActivatedStartAt;
        private Vector2 m_ActivatedEndAt;
        private Vector2 m_ActivatedScale;

        protected override bool CalculateActivated()
        {
            if (!Target)
                return false;

            if (StartAtCurrent && CurveUsage == CurveUsage.NON_DIRECT)
                m_ActivatedStartAt = Target.sizeDelta;
            else
                m_ActivatedStartAt = new Vector3(XEnabled ? StartAt.x : Target.sizeDelta.x,
                                             YEnabled ? StartAt.y : Target.sizeDelta.y);

            m_ActivatedEndAt = new Vector3(XEnabled ? EndAt.x : Target.sizeDelta.x,
                                           YEnabled ? EndAt.y : Target.sizeDelta.y);

            m_ActivatedScale = new Vector3(XEnabled ? Scale.x : 1,
                                           YEnabled ? Scale.y : 1);

            return true;
        }



        /// <summary>
        /// Set target at specified time (0-1)
        /// </summary>
        /// <param name="time"></param>
        public override void SetTarget(float time)
        {
            if (CurveUsage == CurveUsage.NON_DIRECT)
            {
                Target.sizeDelta = new Vector3(XEnabled ? Mathf.Lerp(m_ActivatedStartAt.x, m_ActivatedEndAt.x, CurveX.Evaluate(time)) : Target.sizeDelta.x,
                                               YEnabled ? Mathf.Lerp(m_ActivatedStartAt.y, m_ActivatedEndAt.y, CurveY.Evaluate(time)) : Target.sizeDelta.y);
            }
            else if (CurveUsage == CurveUsage.DIRECT)
            {
                Target.sizeDelta = new Vector3(XEnabled ? CurveX.Evaluate(time) : Target.sizeDelta.x,
                                               YEnabled ? CurveY.Evaluate(time) : Target.sizeDelta.y);
            }
            else if (CurveUsage == CurveUsage.DIRECT_WITH_SCALE)
            {
                Target.sizeDelta = new Vector3(XEnabled ? CurveX.Evaluate(time) * m_ActivatedScale.x : Target.sizeDelta.x,
                                               YEnabled ? CurveY.Evaluate(time) * m_ActivatedScale.y : Target.sizeDelta.y);
            }
        }
    }
}