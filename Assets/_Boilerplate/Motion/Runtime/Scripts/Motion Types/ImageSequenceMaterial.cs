using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace U9.Motion
{
    public class ImageSequenceMaterial : TypedMotion<MaterialComponent, int>
    {
        public Sprite[] Sprites;

        public bool GlobalParameter = false;

        public string ParameterName = "_SomeTexture";

        private int m_PropertyID;

        private void OnEnable()
        {
            m_PropertyID = Shader.PropertyToID(ParameterName);
        }

        /// <summary>
        /// Set target at specified time (0-1)
        /// </summary>
        /// <param name="time"></param>
        public override void SetTarget(float time)
        {
            if (Sprites != null && Sprites.Length > 0)
            {
                if (GlobalParameter)
                    Shader.SetGlobalTexture(m_PropertyID, Sprites[(int)Mathf.Lerp(0, Sprites.Length - 1, time)].texture);
                else if (Target && Target.material)
                    Target.material.SetTexture(m_PropertyID, Sprites[(int)Mathf.Lerp(0, Sprites.Length - 1, time)].texture);
            }
        }
    }
}
