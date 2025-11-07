using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace U9.Motion
{
    public enum PlayType
    {
        PARALLEL,
        SERIAL,
        STAGGERED
    }

    public enum ReplayType
    {
        NONE,
        RESET,
        REPEAT,
        PINGPONG
    }

    [AddComponentMenu("Unit9/Motion/Motion Player", 0)]
    public class MotionPlayer : BaseMotion
    {
        [Tooltip("All the motions to play")]
        public List<BaseMotion> Motions = new List<BaseMotion>();

        [Tooltip("Override the duration of the motions using a Duration Override")]
        public bool OverrideDuration = false;

        [Tooltip("How long (in Seconds) that the motion(s) should take\n\n" +
            "Notes:\n" +
            "This can be used to make a group of Motions finish in an alotted time, " +
            "with each Motions duration scaled as a percentage.\n(i.e One Motion at 1 Second, a second Motion at 2 seconds, " +
            "but you want to have it all done in 2 seconds, one would become 0.6667 seconds and the second would become 1.3333 " +
            "seconds so that both are completed in 2 seconds).\n\n" +
            "0 = take as long as it takes based on the durations of each Motion and how they are played")]
        public float DurationOverride = 0.0f;

        [Tooltip("Should this Player play when it is enabled in the scene?")]
        public bool PlayOnEnable = false;

        [Tooltip("Method in which to play the motions\n\n" +
            "PARALLEL:\tAll played at the same time (All motions scaled so the longest one will take Duration time if Duration > 0)\n" +
            "SERIAL:\t\tEach Motion played one after another (All motions scaled so all can be played within Duration if Duration > 0)\n" +
            "STAGGERED:\tEach motion is played after Delay from when the last Motion started " +
            "(Player Duration is disabled when in Stagger mode)")]
        public PlayType PlayMethod = PlayType.PARALLEL;

        [Tooltip("When motion is finished what should it do next\n\n" +
            "NONE:\t\tThis is the default behaviour, motion stops and leaves it in it's last state\n" + 
            "RESET:\t\tWhen the motion has finished set it back to its initial state\n" +
            "REPEAT:\t\tPlay the motion again automatically from the begining (This will carry on unless you play again with a different mode or Kill the motion)\n" +
            "PINGPONG:\tPlay the motion in the opposing directions once finished (This will carry on unless you play again with a different mode or Kill the motion)")]
        public ReplayType ReplayMethod = ReplayType.NONE;

        [Tooltip("How long to wait before playing the next Motion\n\n" +
            "Note: This doesn't delay the first Motion")]
        public float StaggerDelay = 0.05f;

        public List<BaseMotion> CurrentlyPlayingMotions = new List<BaseMotion>();

        /// <summary>
        /// When motion has finished, depending on ReplayMethod, do action
        /// </summary>
        private event Action OnReplay = null;
        private bool m_HasSentReplay = false;
        private bool m_IsNested = false;

        private void OnEnable()
        {
            // Whenever enabled, recalculate duration adjustments if DurationOverride > 0 and we are not in Stagger mode
            CalculateDurationMultiplier();

            if (PlayOnEnable)
            {
                Initialize();
                StartCoroutine(DelayEnable());
            }
        }

        private IEnumerator DelayEnable()
        {
            yield return new WaitForEndOfFrame();
            
            Play();
        }

        public override void Initialize()
        {
            m_IsPlaying = false;

            // Initialize all motions to their starting value
            foreach (BaseMotion motion in Motions)
            {
                if (motion)
                    motion.Initialize();
            }

            m_IsInFinalState = false;
        }

        /// <summary>
        /// Primary Motion Players Play method, nested motion players do not Play, instead their time gets set by the master
        /// therefore ReplayMethod is done differently (by adjusting the time value, sent by the master, appropriately)
        /// </summary>
        /// <param name="backwards"></param>
        public override void Play(bool backwards = false)
        {
            // Remove old repeating listeners (if they exist)
            OnReplay = null;
            m_HasSentReplay = false;

            switch (ReplayMethod)
            {
                case ReplayType.NONE:
                    break;
                case ReplayType.RESET:
                    if (m_PlayBackwards)
                        OnReplay += Finalize;
                    else
                        OnReplay += Initialize;
                    break;
                case ReplayType.REPEAT:
                    OnReplay += PlayAgain;
                    break;
                case ReplayType.PINGPONG:
                    OnReplay += SwitchDirectionAndPlay;
                    break;

            }

            // For all motions in Motion Player
            foreach (BaseMotion motion in Motions)
            {
                // If the motion itself is a motionplayer then make sure to also subscribe their Replay type)
                if(motion && motion.GetType() == typeof(MotionPlayer))
                {
                    MotionPlayer motionPlayer = (MotionPlayer)motion;
                    motionPlayer.RegisterNestedReplay();
                }
            }

            PlayFrom(backwards, 0.0f);
        }

        /// <summary>
        /// By registering a motion player as Nested the replay is done via mode the time value
        /// </summary>
        public void RegisterNestedReplay()
        {
            m_IsNested = true;
        }

        private void PlayAgain()
        {
            Play(m_PlayBackwards);
        }

        private void SwitchDirectionAndPlay()
        {
            m_PlayBackwards = !m_PlayBackwards;
            
            Play(m_PlayBackwards);
        }

        public override void SetTime(float time)
        {
            // |----SD-PT-ED----|

            // Remove any dangling motions
            Motions.RemoveAll(item => item == null);

            float totalTime = 0.0f;
            float playTime = GetMotionDuration();
            totalTime += StartDelay;
            totalTime += playTime;
            totalTime += EndDelay;

            // If this is a nested Motion Player then we cannot rely on Play as the nested motion players are not Played (instead time is set by the master motion player)
            // Therefore the repeating pattern must be done to the time value being sent here from the master
            if (m_IsNested)
            {
                // Simply repeat time
                if (ReplayMethod == ReplayType.REPEAT)
                    time %= playTime;
                // Time must go forward then backwards based on the mod
                else if (ReplayMethod == ReplayType.PINGPONG)
                    time = Mathf.PingPong(time, playTime);
                // Else if time is at the end of this players playtime
                else if (time >= playTime)
                {
                    // And is set to None: Set time as 1 (it is finished and should stay where it is)
                    if (ReplayMethod == ReplayType.NONE)
                    {
                        foreach (BaseMotion motion in Motions)
                        {
                            if (motion)
                                motion.SetTime(playTime);
                        }

                        // No need to do anything more
                        return;
                    }
                    // And is set to Resset: Set time as 0 (it is finished and should go back to it's start)
                    else if (ReplayMethod == ReplayType.RESET)
                    {
                        foreach (BaseMotion motion in Motions)
                        {
                            if(motion)
                                motion.SetTime(0);
                        }

                        // No need to do anything more
                        return;
                    }
                }
            }

            if (time <= StartDelay)
            {
                foreach (BaseMotion motion in Motions)
                {
                    if(motion)
                        motion.SetTime(0);
                }

                // If in play mode and havent sent on OnReplay yet
                if (Application.isPlaying && !m_HasSentReplay && m_PlayBackwards)
                {
                    // Do so but only once per Play();
                    m_HasSentReplay = true;
                    OnReplay?.Invoke();
                }
            }
            else if (time < playTime + StartDelay)
            {
                if (PlayMethod == PlayType.PARALLEL)
                {
                    foreach (BaseMotion motion in Motions)
                    {
                        if (motion)
                        {
                            motion.DurationMultiplier = DurationMultiplier;
                            motion.SetTime(time - StartDelay);
                        }
                    }
                }
                else if (PlayMethod == PlayType.SERIAL)
                {
                    float cumulativeEndTime = 0.0f;

                    for (int i = 0; i < Motions.Count; i++)
                    {
                        BaseMotion motion = Motions[i];

                        if (motion)
                        {
                            motion.DurationMultiplier = DurationMultiplier;

                            float segmentTime = motion.GetTotalMotionDuration();
                            float startTime = cumulativeEndTime;
                            float endTime = cumulativeEndTime + segmentTime;
                            cumulativeEndTime = endTime;

                            if (time >= startTime && time <= endTime)
                                motion.SetTime(time - startTime);
                            else if (time < startTime)
                                motion.SetTime(0);
                            else
                                motion.SetTime(playTime);
                        }
                    }
                }
                else if (PlayMethod == PlayType.STAGGERED)
                {
                    for (int i = 0; i < Motions.Count; i++)
                    {
                        BaseMotion motion = Motions[i];

                        if (motion)
                        {
                            motion.DurationMultiplier = DurationMultiplier;

                            float segmentTime = motion.GetTotalMotionDuration();
                            float startTime = i * StaggerDelay;
                            float normalizedSegmentEndTime = startTime + segmentTime;

                            if (time >= startTime && time <= normalizedSegmentEndTime)
                                motion.SetTime(time - startTime);
                            else if (time < startTime)
                                motion.SetTime(0);
                            else
                                motion.SetTime(playTime);
                        }
                    }
                }

                // If in play mode and havent sent on Started yet
                if (Application.isPlaying && !m_HasSentStarted)
                {
                    // Do so but only once per Play();
                    m_HasSentStarted = true;
                    OnStartedMotion?.Invoke();
                }
            }
            else
            {
                foreach (BaseMotion motion in Motions)
                {
                    if (motion)
                        motion.SetTime(playTime);
                }

                // If in play mode and havent sent on OnReplay yet
                if (Application.isPlaying && !m_HasSentReplay && !m_PlayBackwards)
                {
                    // Do so but only once per Play();
                    m_HasSentReplay = true;
                    OnReplay?.Invoke();
                }
            }
        }

        public override void Pause()
        {
            m_IsPlaying = false;

            foreach (BaseMotion motion in Motions)
            {
                if (motion)
                    motion.Pause();
            }
        }

        public override void Resume()
        {
            m_IsPlaying = true;

            foreach (BaseMotion motion in Motions)
            {
                if (motion)
                    motion.Resume();
            }
        }

        public void ReverseOrder()
        {
            if (Motions != null)
                Motions.Reverse();
        }

        public override void Finalize(bool fromInactivePlay = false)
        {
            m_IsPlaying = false;

            // Finalise all transitions to their final values
            foreach (BaseMotion motion in Motions)
            {
                if (motion)
                    motion.Finalize(fromInactivePlay);
            }

            Complete = true;

            m_IsInFinalState = true;
        }

        /// <summary>
        /// Not used in normal operations, but can be used to set time as a normalized value
        /// This will account for any Start and End Delay as well
        /// </summary>
        /// <param name="normalizedTime"></param>
        public override void SetTarget(float normalizedTime)
        {
            SetTime(normalizedTime * GetTotalMotionDuration());
        }

        /// <summary>
        /// Override: This is the calculated motion players duration, a total of all the motions being played in their given method
        /// </summary>
        /// <returns></returns>
        public override float GetMotionDuration()
        {
            if(OverrideDuration)
            {
                return DurationOverride;
            }
            else
            {
                float totalDuration = 0.0f;

                if (PlayMethod == PlayType.PARALLEL)
                {
                    float longestDuration = 0.0f;
                    foreach (BaseMotion motion in Motions)
                    {
                        if (motion)
                        {
                            float localTotal = motion.GetTotalMotionDuration();
                            if (localTotal > longestDuration)
                                longestDuration = localTotal;
                        }
                    }
                    totalDuration += longestDuration;
                }
                else if(PlayMethod == PlayType.SERIAL)
                {
                    foreach (BaseMotion motion in Motions)
                    {
                        if (motion)
                        {
                            float localTotal = motion.GetTotalMotionDuration();
                            totalDuration += localTotal;
                        }
                    }
                }
                else if (PlayMethod == PlayType.STAGGERED)
                {
                    float longestDuration = 0.0f;
                    float totalStaggeredDelay = 0.0f;

                    for (int i = 0; i < Motions.Count; i++)
                    {
                        if (Motions[i])
                        {
                            BaseMotion motion = Motions[i];

                            if (motion)
                            {
                                float localTotal = motion.GetTotalMotionDuration();
                                if (localTotal > longestDuration)
                                    longestDuration = localTotal;
                            }
                        }

                        if (i > 0)
                        {
                            totalStaggeredDelay += StaggerDelay;
                        }
                    }

                    totalDuration = longestDuration + totalStaggeredDelay;
                }

                return totalDuration;
            }
        }

        /// <summary>
        /// Override: This is the Total calculated duration for playing all the motions by this player plus any additional start/end delay
        /// </summary>
        /// <returns></returns>
        public override float GetTotalMotionDuration()
        {
            return GetMotionDuration() + StartDelay + EndDelay;
        }

        /// <summary>
        /// The duration multiplier is affected by the play type and is not used during Stagger mode
        /// </summary>
        /// <returns></returns>
        public float CalculateDurationMultiplier()
        {
            if (OverrideDuration && DurationOverride > 0.0f && PlayMethod != PlayType.STAGGERED)
            {
                float requiredTime = 0.0f;

                // Calculate the total amount of time required to play all the motions in Serial
                if (PlayMethod == PlayType.SERIAL)
                {
                    foreach (BaseMotion motion in Motions)
                    {
                        if (motion)
                            requiredTime += motion.Duration;
                    }
                }
                // Find the longest duration when in Parallel
                else
                {
                    foreach (BaseMotion motion in Motions)
                    {
                        if (motion && motion.Duration > requiredTime)
                            requiredTime = motion.Duration;
                    }
                }

                if (requiredTime > 0.0f)
                {
                    DurationMultiplier = DurationOverride / requiredTime;
                }
            }
            else
                DurationMultiplier = 1.0f;

            return DurationMultiplier;
        }
    }
}
