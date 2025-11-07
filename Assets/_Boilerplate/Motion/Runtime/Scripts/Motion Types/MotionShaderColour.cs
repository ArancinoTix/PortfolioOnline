using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace U9.Motion
{
    public class MotionShaderColour : ColourTypedMotion<MaterialComponent, Color>
    {
        public bool GlobalParameter = false;

        public string ParameterName = "_Color";

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
                    StartAt = Shader.GetGlobalColor(m_PropertyID);
                else
                    StartAt = Target.material.GetColor(m_PropertyID);
            }

            base.PlayFrom(backwards, startTime);
        }

        /// <summary>
        /// Set target at specified time (0-1)
        /// </summary>
        /// <param name="time"></param>
        public override void SetTarget(float time)
        {
            if (GlobalParameter)
                Shader.SetGlobalColor(m_PropertyID, Color.Lerp(StartAt, EndAt, Curve.Evaluate(time)));
            else if(Target && Target.material)
                Target.material.SetColor(m_PropertyID, Color.Lerp(StartAt, EndAt, Curve.Evaluate(time)));
        }
    }
}