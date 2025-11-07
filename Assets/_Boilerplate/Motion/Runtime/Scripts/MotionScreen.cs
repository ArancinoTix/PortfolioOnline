using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace U9.Motion
{
    /// <summary>
    /// A simple interface to play one motion player for opening a "Screen" (Sub View)
    /// and another to close it
    /// <br>In addition it will only Open or Close the screen if it's not already opened/closed</br>
    /// </summary>
    public class MotionScreen : MonoBehaviour
    {
        public BaseMotion OpenScreenMotion;
        public BaseMotion CloseScreenMotion;

        public bool PlayCloseBackwards = false;

        public Action<bool> OnScreenStateChanged;

        // Track if screen open or not
        private bool m_ScreenOpen = false;
        private CanvasGroup group;

        private void Awake()
        {
            group = GetComponent<CanvasGroup>();
            OpenScreenMotion.OnFinishedMotion.AddListener(() => ScreenState(true));
            CloseScreenMotion.OnFinishedMotion.AddListener(() => ScreenState(false));

            // Likely this means screen is offscreen, zero in size, fully transparent or otherwise invisible
            OpenScreenMotion.Initialize();
        }

        private void ScreenState(bool open)
        {
            m_ScreenOpen = open;
            if(group)
                group.interactable = open;
            OnScreenStateChanged?.Invoke(open);
        }

        public void ResetScreen(bool instant = false)
        {
            OpenScreenMotion.Initialize();
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
                    OpenScreenMotion.Finalize();
                else
                    OpenScreenMotion.Play();
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
                if(instant)
                    CloseScreenMotion.Finalize();
                else
                    CloseScreenMotion.Play(PlayCloseBackwards);
            }
        }
    }
}
