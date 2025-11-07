using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace U9.Motion
{
    public abstract class ThreeAxisMotion<T, T2> : TwoAxisMotion<T, T2> where T : Component
    {
        public bool ZEnabled = false;

        public MotionDefinition PreDefinedCurveZ = default;

        public AnimationCurve CurveZ = AnimationCurve.Linear(0, 0, 1, 1);
    }
}
