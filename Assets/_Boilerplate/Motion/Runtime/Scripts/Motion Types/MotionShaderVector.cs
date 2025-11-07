using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace U9.Motion
{
    public class MotionShaderVector : ThreeAxisMotion<MaterialComponent, Vector3>
    {
        public bool GlobalParameter = false;

        public string ParameterName = "_SomeVector";

        private int m_PropertyID;

        // Initial value of the parameter
        private Vector3 m_InitialValue;

        // These are what are actually used, before any motion the flags for X Y and Z are checked
        // Ones that are enabled copy StartAt and EndAt values, those not ticked are the current transforms value
        private Vector3 m_ActivatedStartAt;
        private Vector3 m_ActivatedEndAt;
        private Vector3 m_ActivatedScale;

        private void OnEnable()
        {
            m_PropertyID = Shader.PropertyToID(ParameterName);

            if (GlobalParameter)
                m_InitialValue = Shader.GetGlobalVector(m_PropertyID);
            else if(Target && Target.material)
                m_InitialValue = Target.material.GetVector(m_PropertyID);
        }

        protected override bool CalculateActivated()
        {
            if (StartAtCurrent && CurveUsage == CurveUsage.NON_DIRECT)
                m_ActivatedStartAt = m_InitialValue;
            else
                m_ActivatedStartAt = new Vector3(XEnabled ? StartAt.x : m_InitialValue.x,
                                             YEnabled ? StartAt.y : m_InitialValue.y,
                                             ZEnabled ? StartAt.z : m_InitialValue.z);

            m_ActivatedEndAt = new Vector3(XEnabled ? EndAt.x : m_InitialValue.x,
                                           YEnabled ? EndAt.y : m_InitialValue.y,
                                           ZEnabled ? EndAt.z : m_InitialValue.z);

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
            Vector3 vector = m_InitialValue;

            if (CurveUsage == CurveUsage.NON_DIRECT)
            {
                vector = new Vector3(XEnabled ? Mathf.Lerp(m_ActivatedStartAt.x, m_ActivatedEndAt.x, CurveX.Evaluate(time)) : m_InitialValue.x,
                                     YEnabled ? Mathf.Lerp(m_ActivatedStartAt.y, m_ActivatedEndAt.y, CurveY.Evaluate(time)) : m_InitialValue.y,
                                     ZEnabled ? Mathf.Lerp(m_ActivatedStartAt.z, m_ActivatedEndAt.z, CurveZ.Evaluate(time)) : m_InitialValue.z);
                
            }
            else if (CurveUsage == CurveUsage.DIRECT)
            {
                vector = new Vector3(XEnabled ? CurveX.Evaluate(time) : m_InitialValue.x,
                                     YEnabled ? CurveY.Evaluate(time) : m_InitialValue.y,
                                     ZEnabled ? CurveZ.Evaluate(time) : m_InitialValue.z);
            }
            else if (CurveUsage == CurveUsage.DIRECT_WITH_SCALE)
            {
                vector = new Vector3(XEnabled ? CurveX.Evaluate(time) * m_ActivatedScale.x : m_InitialValue.x,
                                     YEnabled ? CurveY.Evaluate(time) * m_ActivatedScale.y : m_InitialValue.y,
                                     ZEnabled ? CurveZ.Evaluate(time) * m_ActivatedScale.z : m_InitialValue.z);
            }

            if (GlobalParameter)
                Shader.SetGlobalVector(m_PropertyID, vector);
            else if (Target && Target.material)
                Target.material.SetVector(m_PropertyID, vector);
        }
    }
}