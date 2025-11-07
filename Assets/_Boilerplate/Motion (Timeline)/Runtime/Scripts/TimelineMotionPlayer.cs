using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

namespace U9.Motion.Timeline
{
    [RequireComponent(typeof(PlayableDirector))]
    public class TimelineMotionPlayer : MonoBehaviour
    {
        [Tooltip("All the motions are assigned in this director")]
        public PlayableDirector m_PlayableDirector;

        [Tooltip("When completed should the director:\n\n" +
            "\tBe reset automatically (None)\n" +
            "\tHeld at the end (Hold) or\n" +
            "\tLoop until told not to (Loop)")]
        public DirectorWrapMode m_WrapMode = DirectorWrapMode.None;

        [Tooltip("Should this Player play when it is enabled in the scene?")]
        public bool PlayOnEnable = false;
        [Tooltip("If the player should play on enable (in which direction should it play?)")]
        public bool AutoPlayBackwards = false;

        [Tooltip("How long to wait (in seconds) before starting the Motion(s)")]
        public float StartDelay = 0;

        [Tooltip("How long to wait (in seconds) after Motion, before being classed as Complete")]
        public float EndDelay = 0;

        [Tooltip("Action that will be invoked immediately when the Player is told to Play")]
        public Action OnPlayMotion = null;

        [Tooltip("Action that will be invoked just as the Motion starts (Can be delayed using Start Delay")]
        public Action OnStartedMotion = null;

        [Tooltip("Action that will be invoked once the Motion has finished (Can be delayed using End Delay)")]
        public Action OnFinishedMotion = null;

        [Tooltip("Action that will be invoked if the Motion is killed")]
        public Action OnKilledMotion = null;

        [Tooltip("Action that will be invoked when the Player is told to Pause")]
        public Action OnPausedMotion = null;

        [Tooltip("Action that will be invoked when the Player is told to Resume")]
        public Action OnResumedMotion = null;

        [Tooltip("Should this motion allow OnFinalize to be called if not active and told to play?\n\n" +
                    "Note: OnFinalize sets the final result of the motion instantly")]
        public bool FinalizeOnInactivePlay = true;

        [Tooltip("If Lockable then when at the final state already the motion cannot be played\n\n" +
                "This is handy if you require something different to play backwards")]
        public bool Lockable = false;

        public bool Complete { get; protected set; } = false;
        public bool IsPlaying { get; protected set; } = false;
        public bool IsPaused { get; protected set; } = false;
        public bool IsInFinalState { get; protected set; } = false;

        private float m_TotalElapsedTime = 0.0f;

        [Serializable]
        public enum KillEndType
        {
            LEAVE,
            INITIALIZE,
            FINALIZE
        }

        private void Awake()
        {
            if (!m_PlayableDirector)
                m_PlayableDirector = GetComponent<PlayableDirector>();

            m_PlayableDirector.extrapolationMode = m_WrapMode;

            Initialise();
        }

        private void OnEnable()
        {
            if (PlayOnEnable)
            {
                Initialise();
                Play(AutoPlayBackwards);
            }
        }

        public void Initialise(float startTime = 0)
        {
            m_PlayableDirector.time = startTime;
            m_PlayableDirector.Evaluate();
            m_PlayableDirector.Pause();

            IsInFinalState = false;
        }

        public void Finalise(bool fromInactivePlay = false)
        {
            m_PlayableDirector.time = m_PlayableDirector.duration;
            m_PlayableDirector.Evaluate();
            m_PlayableDirector.Pause();

            if (fromInactivePlay)
                OnFinishedMotion?.Invoke();

            IsInFinalState = true;
        }

        public void Play(bool backwards = false)
        {
            IsPaused = false;

            StopAllCoroutines();

            if (isActiveAndEnabled)
                StartCoroutine(PlayOverTime(backwards));
            else if (FinalizeOnInactivePlay)
            {
                if (backwards)
                    Initialise();
                else
                    Finalise(true);
            }
        }

        public void PlayFrom(bool backwards = false, float startTime = 0)
        {
            IsPaused = false;

            StopAllCoroutines();

            if (isActiveAndEnabled)
                StartCoroutine(PlayOverTime(backwards, startTime));
            else if (FinalizeOnInactivePlay)
            {
                if (backwards)
                    Initialise();
                else
                    Finalise(true);
            }
        }

        public void Pause()
        {
            m_PlayableDirector.Pause();
            OnPausedMotion?.Invoke();
        }

        public void Resume()
        {
            m_PlayableDirector.Resume();
            OnResumedMotion?.Invoke();
        }

        public void Kill(int killEndType = 0)
        {
            Kill((KillEndType)killEndType);
        }

        public void Kill(KillEndType killEndType)
        {
            Pause();

            if (killEndType == KillEndType.INITIALIZE)
                Initialise();
            else if (killEndType == KillEndType.FINALIZE)
                Finalise();

            OnKilledMotion?.Invoke();
        }

        public float GetNormalizedProgress()
        {
            float totalDuration = (float)m_PlayableDirector.duration + StartDelay + EndDelay;

            if (totalDuration > 0.0f)
                return m_TotalElapsedTime / totalDuration;

            return 1.0f;
        }

        private IEnumerator PlayOverTime(bool backwards = false, float startTime = 0)
        {
            if (Lockable && IsInFinalState && !backwards)
                yield break;
            else if (Lockable && !IsInFinalState && backwards)
                yield break;

            IsPlaying = true;
            Complete = false;

            m_TotalElapsedTime = 0.0f;

            float elapsedTime = 0.0f;

            if (!backwards)
                Initialise();
            else
                Finalise();

            // Play has been started
            OnPlayMotion?.Invoke();

            // Optional delay before starting motion
            if (StartDelay > 0.0f)
            {
                while (elapsedTime < StartDelay)
                {
                    if (!IsPaused)
                    {
                        elapsedTime += Time.deltaTime;
                        m_TotalElapsedTime += Time.deltaTime;
                    }
                    yield return null;
                }

                elapsedTime = 0.0f;
            }

            // Motion has started
            OnStartedMotion?.Invoke();
            IsPlaying = true;

            m_PlayableDirector.time = startTime;
            m_PlayableDirector.Evaluate();

            if (!backwards)
            {
                m_PlayableDirector.Play();

                while (m_PlayableDirector.time < m_PlayableDirector.duration)
                    yield return null;
            }
            else
            {
                // Playing timelines backwards isn't officially supported,
                // only way is to start at the end and evaluate each time going backwards
                // Slightly slower but not by much
                float dt = (float)m_PlayableDirector.duration;

                while (dt > 0)
                {
                    dt -= Time.deltaTime;

                    m_PlayableDirector.time = Mathf.Max(dt, 0);
                    m_PlayableDirector.Evaluate();
                    yield return null;
                }
            }

            IsPlaying = false;

            if (backwards)
                Initialise();
            else
                Finalise();

            // Optional delay before being classed as complete
            if (EndDelay > 0.0f)
            {
                while (elapsedTime < EndDelay)
                {
                    if (!IsPaused)
                    {
                        elapsedTime += Time.deltaTime;
                        m_TotalElapsedTime += Time.deltaTime;
                    }
                    yield return null;
                }
            }

            OnFinishedMotion?.Invoke();

            // Loop again?
            if (m_WrapMode == DirectorWrapMode.Loop)
            {
                StopAllCoroutines();
                StartCoroutine(PlayOverTime(backwards, startTime));
            }

            Complete = true;
        }
    }
}