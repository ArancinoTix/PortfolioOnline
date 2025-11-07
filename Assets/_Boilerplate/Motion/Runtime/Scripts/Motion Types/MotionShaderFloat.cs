using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace U9.Motion
{
    public class MotionShaderFloat : TypedMotion<MaterialComponent, float>
    {
        public bool GlobalParameter = false;

        public string ParameterName = "_SomeFloat";

        private int m_PropertyID;

        private void OnEnable()
        {
            m_PropertyID = Shader.PropertyToID(ParameterName);
        }

        public override void PlayFrom(bool backwards = false, float startTime = 0.0f)
        {
            if (StartAtCurrent && CurveUsage == CurveUsage.NON_DIRECT)
            {
                if (GlobalParameter)
                    StartAt = Shader.GetGlobalFloat(m_PropertyID);
                else
                    StartAt = Target.material.GetFloat(m_PropertyID);
            }

            base.PlayFrom(backwards, startTime);
        }

        /// <summary>
        /// Set target at specified time (0-1)
        /// </summary>
        /// <param name="time"></param>
        public override void SetTarget(float time)
        {
            float value;

            if (CurveUsage == CurveUsage.DIRECT)
                value = Curve.Evaluate(time);
            else if (CurveUsage == CurveUsage.DIRECT_WITH_SCALE)
                value = Curve.Evaluate(time) * Scale;
            else
                value = Mathf.Lerp(StartAt, EndAt, Curve.Evaluate(time));

            if (GlobalParameter)
                Shader.SetGlobalFloat(m_PropertyID, value);
            else if (Target && Target.material)
                Target.material.SetFloat(m_PropertyID, value);
        }
    }
}