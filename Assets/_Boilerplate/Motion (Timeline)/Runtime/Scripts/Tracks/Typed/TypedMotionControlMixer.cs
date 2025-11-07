using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Playables;

namespace U9.Motion.Timeline
{
    public class TypedMotionControlMixer<T> : PlayableBehaviour
    {
        protected T defaultValue;
        protected T blendedValue;
        protected Component component;
        protected bool firstFrameHappened;
        protected string parameterName;
        protected Type type;
        protected PropertyInfo pInfo;

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            firstFrameHappened = false;
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            firstFrameHappened = false;
        }

        public void SetParameterName(string parameterName)
        {
            this.parameterName = parameterName;

            if (component != null && parameterName != null)
            {
                type = component.GetType();
                pInfo = type.GetProperty(parameterName);
            }
        }
    }
}
