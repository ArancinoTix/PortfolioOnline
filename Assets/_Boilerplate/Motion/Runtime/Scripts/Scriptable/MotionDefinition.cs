using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace U9.Motion
{
    [CreateAssetMenu(menuName = "Unit9/UI Motion/Motion Definition", fileName = "UIMotionDef_")]
    public class MotionDefinition : ScriptableObject
    {
        [Tooltip("The Animation Curve used to express the motion over time\n\n" +
            "Notes:\n" +
            "1. X value should be between 0 and 1\n" +
            "2. Y can be between 0 and 1, using StartAt and EndAt to say what 0 and 1 really mean or used directly to represent value")]
        public AnimationCurve Curve = AnimationCurve.Linear(0, 0, 1, 1);
    }
}