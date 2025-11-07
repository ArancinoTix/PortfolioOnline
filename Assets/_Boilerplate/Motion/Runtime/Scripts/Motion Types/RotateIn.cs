using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace U9.Motion
{
    public class RotateIn : ThreeAxisMotion<Transform, Vector3>
    {
        public bool LocalSpace = true;

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
                m_ActivatedStartAt = LocalSpace ? Target.transform.localEulerAngles : Target.transform.eulerAngles;
            else
                m_ActivatedStartAt = new Vector3(XEnabled ? StartAt.x : LocalSpace ? Target.transform.localEulerAngles.x : Target.transform.eulerAngles.x,
                                                 YEnabled ? StartAt.y : LocalSpace ? Target.transform.localEulerAngles.y : Target.transform.eulerAngles.y,
                                                 ZEnabled ? StartAt.z : LocalSpace ? Target.transform.localEulerAngles.z : Target.transform.eulerAngles.z);

            m_ActivatedEndAt = new Vector3(XEnabled ? EndAt.x : LocalSpace ? Target.transform.localEulerAngles.x : Target.transform.eulerAngles.x,
                                           YEnabled ? EndAt.y : LocalSpace ? Target.transform.localEulerAngles.y : Target.transform.eulerAngles.y,
                                           ZEnabled ? EndAt.z : LocalSpace ? Target.transform.localEulerAngles.z : Target.transform.eulerAngles.z);

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
            Vector3 currentEuler = LocalSpace ? Target.localEulerAngles : Target.eulerAngles;
            Vector3 newEuler = currentEuler;

            if (CurveUsage == CurveUsage.NON_DIRECT)
            {
                newEuler = new Vector3(
                    XEnabled ? Mathf.Lerp(m_ActivatedStartAt.x, m_ActivatedEndAt.x, CurveX.Evaluate(time)) : currentEuler.x,
                    YEnabled ? Mathf.Lerp(m_ActivatedStartAt.y, m_ActivatedEndAt.y, CurveY.Evaluate(time)) : currentEuler.y,
                    ZEnabled ? Mathf.Lerp(m_ActivatedStartAt.z, m_ActivatedEndAt.z, CurveZ.Evaluate(time)) : currentEuler.z
                );
            }
            else if (CurveUsage == CurveUsage.DIRECT)
            {
                newEuler = new Vector3(
                    XEnabled ? CurveX.Evaluate(time) : currentEuler.x,
                    YEnabled ? CurveY.Evaluate(time) : currentEuler.y,
                    ZEnabled ? CurveZ.Evaluate(time) : currentEuler.z
                );
            }
            else if (CurveUsage == CurveUsage.DIRECT_WITH_SCALE)
            {
                newEuler = new Vector3(
                    XEnabled ? CurveX.Evaluate(time) * m_ActivatedScale.x : currentEuler.x,
                    YEnabled ? CurveY.Evaluate(time) * m_ActivatedScale.y : currentEuler.y,
                    ZEnabled ? CurveZ.Evaluate(time) * m_ActivatedScale.z : currentEuler.z
                );
            }

            if (LocalSpace)
                Target.localRotation = Quaternion.Euler(newEuler);
            else
                Target.rotation = Quaternion.Euler(newEuler);
        }
    }
}