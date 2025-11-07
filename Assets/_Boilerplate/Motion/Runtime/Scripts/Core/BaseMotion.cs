using System;
using UnityEngine;
using UnityEngine.Events;

namespace U9.Motion
{
    public enum CurveUsage
    {
        NON_DIRECT,
        DIRECT,
        DIRECT_WITH_SCALE,
        [InspectorName(null)] NONE // Not selectable but this is used for Image Sequence
    }

    public abstract class BaseMotion : MonoBehaviour
    {
        public string Identifier = "Motion (Task)";

        [Tooltip("A Pre Defined Curve to use (This makes it easier to reuse/edit curves for many parts at once)")]
        public MotionDefinition PreDefinedCurve = null;

        [Tooltip("If a pre defined curve is not set then this is the curve to use")]
        public AnimationCurve Curve = AnimationCurve.Linear(0, 0, 1, 1);

        [Tooltip("How long it should take to get from X = 0 to X = 1 (in Seconds)")]
        public float Duration = 1.0f;

        [Tooltip("How to use the curve\n\n" +
                "NON_DIRECT = The Y values should be between 0 and 1 (where 0 = StartAt and 1 = EndAt)\n" +
                "DIRECT = The Y values are used directly\n" +
                "DIRECT_WITH_SCALE = The Y values are used directly but then multiplied by Scale")]
        public CurveUsage CurveUsage = CurveUsage.NON_DIRECT;

        [Tooltip("How long to wait (in seconds) before starting the Motion(s)")]
        public float StartDelay = 0;
        [Tooltip("How long to wait (in seconds) after Motion, before being classed as Complete")]
        public float EndDelay = 0;

        [Tooltip("Should we use unscaled time to move the motion onwards? Useful for UI motion when gameplay is paused")]
        public bool UseUnscaledTime = false;

        [Tooltip("Event that will be played immediately when the Player is told to Play")]
        public UnityEvent OnPlayMotion = null;

        [Tooltip("Event that will be played just as the Motion starts (Can be delayed using Start Delay")]
        public UnityEvent OnStartedMotion = null;

        [Tooltip("Event that will be played once all Motions have been finished (Can be delayed using End Delay)")]
        public UnityEvent OnFinishedMotion = null;

        [Tooltip("Event that will be played if the motion is killed (Not visible in the editor list (Internal only))")]
        public UnityEvent OnKilledMotion = null;

        [Tooltip("Should this motion allow OnFinalize to be called if not active and told to play?\n\n" +
                "Note: OnFinalize sets the final result of the motion instantly")]
        public bool FinalizeOnInactivePlay = true;

        [Tooltip("If Lockable then when at final state already the motion cannot be played\n\n" +
            "This is handy if you require something different to play backwards")]
        public bool Lockable = false;

        /// <summary>
        /// This can be calculated and set by Motion Players to adjust the duration of this motion
        /// </summary>
        public float DurationMultiplier = 1.0f;

        /// <summary>
        /// Motion Complete?
        /// </summary>
        public bool Complete { get; protected set; } = false;

        /// <summary>
        /// The current time of the motion (in Seconds)
        /// </summary>
        protected float m_CurrentTime = 0.0f;

        /// <summary>
        /// Is the motion playing? This can be forwards or backwards, when it's reached either end then it stops playing and OnFinished is invoked
        /// </summary>
        protected bool m_IsPlaying = false;

        /// <summary>
        /// A motion can be playing but Paused, when paused the Update doesnt increment/decrement time but logically it is still set to playing
        /// </summary>
        protected bool m_IsPaused = false;

        /// <summary>
        /// Should the motion play forwards or backwards
        /// </summary>
        protected bool m_PlayBackwards = false;

        /// <summary>
        /// This is used for locking the motion, if locking is enabled and we are playing forwards but we are already in the final state, then ignore the play of the motion
        /// <br>Similarly, if playing backwards but we are already not in the final state then ignore</br>
        /// </summary>
        protected bool m_IsInFinalState = false;

        // When in play mode these are used to determine if we have sent the callbacks already during one play
        // Reset when Play is called again
        protected bool m_HasSentPlaying = false;
        protected bool m_HasSentStarted = false;
        protected bool m_HasSentFinished = false;

        [Serializable]
        public enum KillEndType
        {
            LEAVE,
            INITIALIZE,
            FINALIZE
        }

        /// <summary>
        /// Initialize the motion (Set time to 0)
        /// </summary>
        public abstract void Initialize();

        /// <summary>
        /// Finalize the motion (Set time to total length of the motion (including delays))
        /// </summary>
        /// <param name="fromInactivePlay"></param>
        public abstract void Finalize(bool fromInactivePlay = false);

        /// <summary>
        /// Play a motion from the begining, or the end if playing backwards
        /// </summary>
        /// <param name="backwards"></param>
        public virtual void Play(bool backwards = false)
        {
            PlayFrom(backwards, 0.0f);
        }
        public virtual void PlayFrom(bool backwards = false, float startTime = 0.0f)
        {
            // Complete is assumed unless target starts playing (then it's only Complete when it has finished playing)
            Complete = true;

            // Tell the Update which way to play
            m_PlayBackwards = backwards;

            // If going backwards then we start at the end minus the start time (how long into the motion to start from)
            if (backwards)
                m_CurrentTime = GetTotalMotionDuration() - startTime;
            else
                m_CurrentTime = startTime;

            // This is a new Play, so we havent sent any events yet
            m_HasSentPlaying = false;
            m_HasSentStarted = false;
            m_HasSentFinished = false;

            // If Inactive
            if (!isActiveAndEnabled)
            {
                // But can finalize on InActive Play
                if (FinalizeOnInactivePlay)
                {
                    if (backwards)
                        Initialize();
                    else
                        Finalize(true); // Set Final state
                }
                else
                    return;
            }
            
            // Tell the Update to start playing
            m_IsPlaying = true;

            // If in play mode and havent sent OnPlay yet
            if (Application.isPlaying && !m_HasSentPlaying)
            {
                // Do so but only once per Play();
                m_HasSentPlaying = true;
                OnPlayMotion?.Invoke();
            }
        }

        /// <summary>
        /// Sets the normalized time of the motion
        /// </summary>
        /// <param name="normalizedTime">(0 - 1) to represent time on the curve</param>
        public abstract void SetTarget(float normalizedTime);


        /// <summary>
        /// Sets the time of the complete motion, taking into account of Start and End Delay
        /// </summary>
        /// <param name="time">In seconds</param>
        public virtual void SetTime(float time)
        {
            float motionDuration = GetMotionDuration();
            if (time <= StartDelay)
                SetTarget(0);
            else if (time <= motionDuration + StartDelay)
                SetTarget((time - StartDelay) / motionDuration);
            else
                SetTarget(1); 
        }

        /// <summary>
        /// Pauses the playback in Update
        /// </summary>
        public virtual void Pause()
        {
            m_IsPaused = true;
        }

        /// <summary>
        /// Resumes the playback in Update
        /// </summary>
        public virtual void Resume()
        {
            m_IsPaused = false;
        }
        
        /// <summary>
        /// Get the duration of the actual motion, excluding any start or end delays
        /// </summary>
        /// <returns></returns>
        public virtual float GetMotionDuration() 
        {
            return Duration * DurationMultiplier; 
        }

        /// <summary>
        /// Get the total duration of this motion, this includes and Start and/or End delay
        /// </summary>
        /// <returns></returns>
        public virtual float GetTotalMotionDuration()
        {
            return (Duration * DurationMultiplier) + StartDelay + EndDelay;
        }

        /// <summary>
        /// Stops the motion dead in it's tracks
        /// </summary>
        /// <param name="killEndType">What to do when killed</param>
        public void Kill(int killEndType = 0)
        {
            Kill((KillEndType)killEndType);
        }
        public void Kill(KillEndType killEndType)
        {
            StopAllCoroutines();

            if (killEndType == KillEndType.INITIALIZE)
                Initialize();
            else if (killEndType == KillEndType.FINALIZE)
                Finalize();

            OnKilledMotion.Invoke();
        }

        /// <summary>
        /// Get the current time of the motion
        /// </summary>
        /// <returns></returns>
        public float GetCurrentTime()
        {
            return m_CurrentTime;
        }

        /// <summary>
        /// This can be ussed to automatically destroy the motions gameobject
        /// <br>For example call this OnFinished and you have yourself a Toast</br>
        /// </summary>
        public void DestroyMe()
        {
            Destroy(gameObject);
        }

        /// <summary>
        /// This is where current time is moved forwards or backwards (updated within Unitys call stack)
        /// </summary>
        protected virtual void Update()
        {
            if (m_IsPlaying)
            {
                // Don't allow play if already at end and is lockable
                if (Lockable && m_IsInFinalState && !m_PlayBackwards)
                {
                    m_IsPlaying = false;
                    return;
                }
                else if (Lockable && !m_IsInFinalState && m_PlayBackwards)
                {
                    m_IsPlaying = false;
                    return;
                }

                float deltaTime = UseUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

                if (m_PlayBackwards)
                {
                    if (m_CurrentTime > 0.0f + deltaTime)
                    {
                        if (!m_IsPaused)
                            m_CurrentTime -= deltaTime;

                        SetTime(m_CurrentTime);
                    }
                    else
                    {
                        m_CurrentTime = 0.0f;
                        m_IsPlaying = false;
                        Complete = true;
                        m_IsInFinalState = false;

                        SetTime(m_CurrentTime);

                        // If in play mode and havent sent on Finished yet
                        if (Application.isPlaying && !m_HasSentFinished)
                        {
                            // Do so but only once per Play();
                            m_HasSentFinished = true;
                            OnFinishedMotion?.Invoke();
                        }
                    }
                }
                else
                {
                    if (m_CurrentTime < GetTotalMotionDuration() - deltaTime)
                    {
                        if (!m_IsPaused)
                            m_CurrentTime += deltaTime;
                        SetTime(m_CurrentTime);
                    }
                    else
                    {
                        m_CurrentTime = GetTotalMotionDuration();
                        m_IsPlaying = false;
                        Complete = true;
                        m_IsInFinalState = true;

                        SetTime(m_CurrentTime);

                        // If in play mode and havent sent on Finished yet
                        if (Application.isPlaying && !m_HasSentFinished)
                        {
                            // Do so but only once per Play();
                            m_HasSentFinished = true;
                            OnFinishedMotion?.Invoke();
                        }
                    }
                }
            }
        }

#if UNITY_EDITOR

        [HideInInspector]
        public bool foldoutCurveDef;
        [HideInInspector]
        public bool foldoutCurveDefs;
        [HideInInspector]
        public bool foldoutMotionSettings;
        [HideInInspector]
        public bool foldoutPlaybackSettings;
        [HideInInspector]
        public bool foldoutCallbacks;

        public bool EditorPreviewEnabled = false;
        public float EditorPreviewValue = 0.0f;

        
#endif

#if MOTION_VERBOSE

        private enum LogType
        {
            Play,
            Finish,
            Kill
        }
        private void Awake()
        {
            OnPlayMotion.AddListener(() => Log(LogType.Play, OnPlayMotion));
            OnFinishedMotion.AddListener(() => Log(LogType.Finish, OnFinishedMotion));
            OnKilledMotion.AddListener(() => Log(LogType.Kill, OnKilledMotion));
        }

        private void Log(LogType logType, UnityEvent theEvent)
        {
            string[] listners = new string[theEvent.GetPersistentEventCount()];
            for(int I = 0; I < listners.Length; I++)
            {
                listners[I] = theEvent.GetPersistentMethodName(I);
            }
            string listOfListners = string.Join(", ", listners);

            Debug.LogFormat("Motion: {0} - Action: {1} \nCallbacks: {2}", gameObject.name, logType.ToString(), listOfListners);
        }
#endif

    }
}
