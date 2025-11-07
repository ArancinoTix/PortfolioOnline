using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace U9.Motion.Timeline
{
    [Serializable]
    public enum ClipExtrapolation
    {
        /// <summary>
        /// Do Nothing: When playing this will seem to act like hold, but it actually just does nothing, setting time to somewhere there is no clip directly will do nothing!
        /// </summary>
        None,
        /// <summary>
        /// Hold: When playing this will leave the value where it was at the end of the clip, setting time to somewhere there is no clip will result it the value being the last clip set to Hold
        /// This is the default behaviour you'll likely want
        /// </summary>
        Hold,
        /// <summary>
        /// Default: When not on a clip the value will go back to what it was before the track edited the value, this will happen unless after a track that is set to None or Hold
        /// </summary>
        Default
    }

    [Serializable]
    public class TypedControlBehaviour<T> : PlayableBehaviour
    {
        // The Start and End times of this clip
        [HideInInspector] public double start;
        [HideInInspector] public double end;

        public T startAt = default;
        public T endAt = default;

        public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Tooltip("What to do after this clip has finished \n" +
            "None: Do nothing \n" +
            "Hold: Keep last value\n" +
            "Default: Return to default value")]
        public ClipExtrapolation clipExtrapolation = ClipExtrapolation.Hold;
    }

    [Serializable]
    public class SpriteControlBehaviour : PlayableBehaviour
    {
        // The Start and End times of this clip
        [HideInInspector] public double start;
        [HideInInspector] public double end;

        public Sprite[] sprites;

        [Tooltip("What to do after this clip has finished \n" +
            "None: Do nothing \n" +
            "Hold: Keep last value\n" +
            "Default: Return to default value")]
        public ClipExtrapolation clipExtrapolation = ClipExtrapolation.Hold;
    }

    [Serializable]
    public class MultiAxisControlBehaviour<T> : PlayableBehaviour
    {
        // The Start and End times of this clip
        [HideInInspector] public double start;
        [HideInInspector] public double end;

        // Special toggle enabled by certain bespoke tracks where things can be set local or world
        [HideInInspector] public bool spaceUnlocked = false;
        [HideInInspector] public bool useWorldSpace = false;

        // Special toggle enabled by certain bespoke tracks where things can be set relative or not
        [HideInInspector] public bool relativeUnlocked = false;
        [HideInInspector] public bool useRelative = false;

        public T startAt = default;
        public T endAt = default;

        [Tooltip("What to do after this clip has finished \n" +
            "None: Do nothing \n" +
            "Hold: Keep last value\n" +
            "Default: Return to default value")]
        public ClipExtrapolation clipExtrapolation = ClipExtrapolation.Hold;

        public bool xEnabled = default;
        public AnimationCurve curveX = AnimationCurve.EaseInOut(0, 0, 1, 1);
        public bool yEnabled = default;
        public AnimationCurve curveY = AnimationCurve.EaseInOut(0, 0, 1, 1);
    }

    [Serializable]
    public class Vector3ControlBehaviour : MultiAxisControlBehaviour<Vector3>
    {
        public bool zEnabled = default;
        public AnimationCurve curveZ = AnimationCurve.EaseInOut(0, 0, 1, 1);
    }

    [Serializable]
    public class Vector4ControlBehaviour : MultiAxisControlBehaviour<Vector4>
    {
        public bool zEnabled = default;
        public AnimationCurve curveZ = AnimationCurve.EaseInOut(0, 0, 1, 1);
        public bool wEnabled = default;
        public AnimationCurve curveW = AnimationCurve.EaseInOut(0, 0, 1, 1);
    }
}