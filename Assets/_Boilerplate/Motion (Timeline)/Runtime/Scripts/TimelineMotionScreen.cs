using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace U9.Motion.Timeline
{
    /// <summary>
    /// This is a timeline motion compatable version of MotionScreen
    /// <br>A simple interface to play one motion player for opening a "Screen" (Sub View) and another to close it</br>
    /// <br>In addition it will only Open or Close the screen if it's not already opened/closed</br>
    /// </summary>
    public class TimelineMotionScreen : MonoBehaviour
    {
        public TimelineMotionPlayer OpenScreenPlayer;
        public TimelineMotionPlayer CloseScreenPlayer;

        public bool PlayCloseBackwards = false;

        public Action<bool> OnScreenStateChanged;

        public bool useHaptics = false;

        public bool InitOnLaunch = true;

        // Track if screen open or not
        private bool m_ScreenOpen = false;
        private CanvasGroup group;

        private void Awake()
        {
            group = GetComponent<CanvasGroup>();
            OpenScreenPlayer.OnFinishedMotion +=() => ScreenState(true);
            CloseScreenPlayer.OnFinishedMotion +=() => ScreenState(false);

            // Likely this means screen is offscreen, zero in size, fully transparent or otherwise invisible
            if(InitOnLaunch)
                OpenScreenPlayer.Initialise();
        }

        private void ScreenState(bool open)
        {
            m_ScreenOpen = open;
            if (group)
                group.interactable = open;
            OnScreenStateChanged?.Invoke(open);
        }

        public void ResetScreen(bool instant = false)
        {
            m_ScreenOpen = false;
            OpenScreenPlayer.Initialise();
        }

        public void OpenScreen()
        {
            OpenScreen(false, false);
        }

        public void OpenScreen(bool instant = false, bool force = false)
        {
            if (!m_ScreenOpen || force)
            {
                if (instant)
                    OpenScreenPlayer.Finalise();
                else
                    OpenScreenPlayer.Play();
            }
        }

        public void ForceClose()
        {
            CloseScreen(false, true);
        }

        public void CloseScreen()
        {
            CloseScreen(false, false);
        }

        public void CloseScreen(bool instant = false, bool force = false)
        {
            if (m_ScreenOpen || force)
            {
                if (instant)
                    CloseScreenPlayer.Finalise();
                else
                    CloseScreenPlayer.Play(PlayCloseBackwards);
            }
        }
    }
}
