using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace U9.Motion
{
    public class MoveInRect : ThreeAxisMotion<RectTransform, Vector3>
    {
        // These are what are actually used, before any motion the flags for X Y and Z are checked
        // Ones that are enabled copy StartAt and EndAt values, those not ticked are the current transforms value
        private Vector3 m_ActivatedStartAt;
        private Vector3 m_ActivatedEndAt;
        private Vector3 m_ActivatedScale;

        protected override bool CalculateActivated()
        {
            if (!Target)
                return false;

            if (StartAtCurrent && CurveUsage == CurveUsage.NON_DIRECT)
                m_ActivatedStartAt = Target.anchoredPosition3D;
            else
                m_ActivatedStartAt = new Vector3(XEnabled ? StartAt.x : Target.anchoredPosition3D.x,
                                                 YEnabled ? StartAt.y : Target.anchoredPosition3D.y,
                                                 ZEnabled ? StartAt.z : Target.anchoredPosition3D.z);

            m_ActivatedEndAt = new Vector3(XEnabled ? EndAt.x : Target.anchoredPosition3D.x,
                                           YEnabled ? EndAt.y : Target.anchoredPosition3D.y,
                                           ZEnabled ? EndAt.z : Target.anchoredPosition3D.z);

            m_ActivatedScale = new Vector3(XEnabled ? Scale.x : 1,
                                           YEnabled ? Scale.y : 1,
                                           ZEnabled ? Scale.z : 1);

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
                Target.anchoredPosition3D = new Vector3(XEnabled ? Mathf.Lerp(m_ActivatedStartAt.x, m_ActivatedEndAt.x, CurveX.Evaluate(time)) : Target.anchoredPosition3D.x,
                                                        YEnabled ? Mathf.Lerp(m_ActivatedStartAt.y, m_ActivatedEndAt.y, CurveY.Evaluate(time)) : Target.anchoredPosition3D.y,
                                                        ZEnabled ? Mathf.Lerp(m_ActivatedStartAt.z, m_ActivatedEndAt.z, CurveZ.Evaluate(time)) : Target.anchoredPosition3D.z);
            }
            else if (CurveUsage == CurveUsage.DIRECT)
            {
                Target.anchoredPosition3D = new Vector3(XEnabled ? CurveX.Evaluate(time) : Target.anchoredPosition3D.x,
                                                        YEnabled ? CurveY.Evaluate(time) : Target.anchoredPosition3D.y,
                                                        ZEnabled ? CurveZ.Evaluate(time) : Target.anchoredPosition3D.z);
            }
            else if (CurveUsage == CurveUsage.DIRECT_WITH_SCALE)
            {
                Target.anchoredPosition3D = new Vector3(XEnabled ? CurveX.Evaluate(time) * m_ActivatedScale.x : Target.anchoredPosition3D.x,
                                                        YEnabled ? CurveY.Evaluate(time) * m_ActivatedScale.y : Target.anchoredPosition3D.y,
                                                        ZEnabled ? CurveZ.Evaluate(time) * m_ActivatedScale.z : Target.anchoredPosition3D.z);
            }
        }
    }
}