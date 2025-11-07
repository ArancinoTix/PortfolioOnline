using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace U9.Motion
{
    public class ImageSequence : TypedMotion<Image, int>
    {
        public Sprite[] Sprites;

        /// <summary>
        /// Set target at specified time (0-1)
        /// </summary>
        /// <param name="time"></param>
        public override void SetTarget(float time)
        {
            if(Sprites != null && Sprites.Length > 0)
                Target.sprite = Sprites[(int)Mathf.Lerp(0, Sprites.Length - 1, time)];
        }
    }
}
