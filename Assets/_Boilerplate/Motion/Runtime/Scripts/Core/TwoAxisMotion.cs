using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace U9.Motion
{
    public abstract class TwoAxisMotion<T, T2> : TypedMotion<T, T2> where T : Component
    {
        public bool XEnabled, YEnabled = false;

        public MotionDefinition PreDefinedCurveX = default;
        public MotionDefinition PreDefinedCurveY = default;

        public AnimationCurve CurveX = AnimationCurve.Linear(0, 0, 1, 1);
        public AnimationCurve CurveY = AnimationCurve.Linear(0, 0, 1, 1);

        protected abstract bool CalculateActivated();

        public override void Initialize()
        {
            StopAllCoroutines();

            if (Target)
                CalculateActivated();

            base.Initialize();
        }

        public override void SetTime(float time)
        {
            if (Target)
            {
                CalculateActivated();

                base.SetTime(time);
            }
        }
    }
}
