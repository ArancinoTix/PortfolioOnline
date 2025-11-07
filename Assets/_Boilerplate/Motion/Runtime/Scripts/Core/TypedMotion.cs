using UnityEngine;

namespace U9.Motion
{
    public abstract class TypedMotion<T, T2> : BaseMotion where T : Component
    {
        [Tooltip("The target to motion\n\n" +
            "Note: Will try to find relevant component on this gameobject if not manually set")]
        public T Target;

        [Tooltip("If enabled then this motion will try to use the objects current status as its starting state")]
        public bool StartAtCurrent = false;
        [Tooltip("Designated value to represent what the Curves Y = 0 equates to")]
        public T2 StartAt = default;
        [Tooltip("Designated value to represent what the Curves Y = 1 equates to")]
        public T2 EndAt = default;

        // Optional field, if using curve with scale
        [Tooltip("Multiplies Curve Y values (Only in use if Curve Usage is DIRECT_WITH_SCALE)")]
        public T2 Scale = default;

        private void Awake()
        {
            // Try to find matching component on this gameobject
            if (Target == null)
                Target = GetComponent<T>();
        }

        public override void Initialize()
        {
            m_IsPlaying = false;

            if (Target)
                SetTarget(0);

            m_IsInFinalState = false;
            m_CurrentTime = 0;
        }

        public override void Finalize(bool fromInactivePlay = false)
        {
            m_IsPlaying = false;

            // If playing finalize because Play was called on inactive object and this Motion disallows this, then don't carry on here
            if (fromInactivePlay && !FinalizeOnInactivePlay)
                return;

            if (Target)
                SetTarget(1);

            if (fromInactivePlay)
                OnFinishedMotion?.Invoke();

            m_IsInFinalState = true;
            m_CurrentTime = GetTotalMotionDuration();
        }

        public override void SetTime(float time)
        {
            if (Target)
                base.SetTime(time);
        }
    }
}
