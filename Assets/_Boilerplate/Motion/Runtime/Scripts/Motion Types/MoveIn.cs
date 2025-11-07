using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace U9.Motion
{ 
    public class MoveIn : ThreeAxisMotion<Transform, Vector3>
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
                m_ActivatedStartAt = LocalSpace ? Target.transform.localPosition : Target.transform.position;
            else
                m_ActivatedStartAt = new Vector3(XEnabled ? StartAt.x : LocalSpace ? Target.transform.localPosition.x : Target.transform.position.x,
                                                 YEnabled ? StartAt.y : LocalSpace ? Target.transform.localPosition.y : Target.transform.position.y,
                                                 ZEnabled ? StartAt.z : LocalSpace ? Target.transform.localPosition.z : Target.transform.position.z);

            m_ActivatedEndAt = new Vector3(XEnabled ? EndAt.x : LocalSpace ? Target.transform.localPosition.x : Target.transform.position.x,
                                           YEnabled ? EndAt.y : LocalSpace ? Target.transform.localPosition.y : Target.transform.position.y,
                                           ZEnabled ? EndAt.z : LocalSpace ? Target.transform.localPosition.z : Target.transform.position.z);

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
            Vector3 currentPosition = LocalSpace ? Target.localPosition : Target.position;
            Vector3 newPosition = currentPosition;

            if (CurveUsage == CurveUsage.NON_DIRECT)
            {
                    newPosition = new Vector3(XEnabled ? Mathf.Lerp(m_ActivatedStartAt.x, m_ActivatedEndAt.x, CurveX.Evaluate(time)) : currentPosition.x,
                                              YEnabled ? Mathf.Lerp(m_ActivatedStartAt.y, m_ActivatedEndAt.y, CurveY.Evaluate(time)) : currentPosition.y,
                                              ZEnabled ? Mathf.Lerp(m_ActivatedStartAt.z, m_ActivatedEndAt.z, CurveZ.Evaluate(time)) : currentPosition.z);
            }
            else if (CurveUsage == CurveUsage.DIRECT)
            {
                newPosition = new Vector3(XEnabled ? CurveX.Evaluate(time) : currentPosition.x,
                                          YEnabled ? CurveY.Evaluate(time) : currentPosition.y,
                                          ZEnabled ? CurveZ.Evaluate(time) : currentPosition.z);
            }
            else if (CurveUsage == CurveUsage.DIRECT_WITH_SCALE)
            {
                newPosition = new Vector3(XEnabled ? CurveX.Evaluate(time) * m_ActivatedScale.x : currentPosition.x,
                                          YEnabled ? CurveY.Evaluate(time) * m_ActivatedScale.y : currentPosition.y,
                                          ZEnabled ? CurveZ.Evaluate(time) * m_ActivatedScale.z : currentPosition.z);
            }

            if (LocalSpace)
                Target.localPosition = newPosition;
            else
                Target.position = newPosition;
        }
    }
}