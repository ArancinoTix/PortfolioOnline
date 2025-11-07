using System;
using System.Collections.Generic;
using UnityEngine;

namespace U9.Motion.Timeline
{
    /// <summary>
    /// This is a timeline motion compatable version of MotionScreen
    /// <br>A simple interface to control a group of MotionScreens</br>
    /// </summary>
    public class TimelineMotionScreenManager : MonoBehaviour
    {
        [SerializeField] private int m_InitialScreen = -1;
        [SerializeField] private List<TimelineMotionScreen> m_Screens = new List<TimelineMotionScreen>();

        public Action<int> MotionScreenedOpened;
        public Action<int> MotionScreenClosed;

        private int m_CurrentActiveScreen = -1;
        public int CurrentScreen { get { return m_CurrentActiveScreen; } }

        private CanvasGroup m_FoundParentCanvasGroup;

        private void Awake()
        {
            for (int I = 0; I < m_Screens.Count; I++)
            {
                int V = I;
                m_Screens[I].OpenScreenPlayer.OnPlayMotion += ScreenChanging;
                m_Screens[I].OpenScreenPlayer.OnFinishedMotion += () => ScreenOpened(V);
                m_Screens[I].CloseScreenPlayer.OnPlayMotion += ScreenChanging;
                m_Screens[I].CloseScreenPlayer.OnFinishedMotion += () => ScreenClosed(V);
            }

            SwitchToScreen(m_InitialScreen);
        }

        /// <summary>
        /// Disable all interactivity in realation to this canvas group hierachy
        /// this is then re-enabled only when open motion has finished.
        /// </summary>
        private void ScreenChanging()
        {
            if (m_FoundParentCanvasGroup)
                m_FoundParentCanvasGroup.interactable = false;
        }

        private void ScreenOpened(int screenID)
        {
            MotionScreenedOpened?.Invoke(screenID);

            if (m_FoundParentCanvasGroup)
                m_FoundParentCanvasGroup.interactable = true;
        }

        private void ScreenClosed(int screenID)
        {
            if (m_CurrentActiveScreen != -1)
            {
                m_Screens[m_CurrentActiveScreen].OpenScreen();
            }
            else
            {
                foreach (var screen in m_Screens)
                    screen.ResetScreen();

                if (m_FoundParentCanvasGroup)
                    m_FoundParentCanvasGroup.interactable = true;
            }

            MotionScreenClosed?.Invoke(screenID);
        }

        /// <summary>
        /// Closes previous screen (if there was one), then opens up the selected screen
        /// </summary>
        /// <param name="screenID"></param>
        public void SwitchToScreen(int screenID)
        {
            // ALready on this screen so cannot go to it again
            if(m_CurrentActiveScreen == screenID) 
                return;

            // If the screen to go to exists
            if (screenID >= 0 && m_Screens.Count > screenID)
            {
                // Kill all screen motions in case one was playing
                foreach (var screen in m_Screens)
                {
                    if (screen.CloseScreenPlayer)
                        screen.CloseScreenPlayer.Kill();
                    if (screen.OpenScreenPlayer)
                        screen.OpenScreenPlayer.Kill();
                }

                // If the was an active screen then close that first
                if (m_CurrentActiveScreen != -1)
                {
                    m_Screens[m_CurrentActiveScreen].ForceClose();
                    // When it is closed it will then Open the new selected screen
                    m_CurrentActiveScreen = screenID;
                }
                // Otherwise we can just Open the new screen
                else
                {
                    m_CurrentActiveScreen = screenID;
                    m_Screens[m_CurrentActiveScreen].OpenScreen(true);
                }

            }
            // If screen to go to is -1, then actually just close the active screen (if one is active)
            else if (screenID == -1 && m_CurrentActiveScreen != -1)
            {
                m_Screens[m_CurrentActiveScreen].ForceClose();
                m_CurrentActiveScreen = -1;
            }
        }

        /// <summary>
        /// Attempts to go back one screen, if already on the first the returns false (Go back to previous state instead?)
        /// </summary>
        /// <returns></returns>
        public bool GoBackOne()
        {
            if (m_CurrentActiveScreen != -1 && m_CurrentActiveScreen != 0)
            {
                SwitchToScreen(m_CurrentActiveScreen - 1);
                return true;
            }

            // Couldn't go back
            return false;
        }

        /// <summary>
        /// Attempts to go forward one screen, if already on the last the returns false (Go forward to next state instead?)
        /// </summary>
        /// <returns></returns>
        public bool GoForwardOne()
        {
            if (m_CurrentActiveScreen < m_Screens.Count - 1)
            {
                SwitchToScreen(m_CurrentActiveScreen + 1);
                return true;
            }

            // Couldn't go forward
            return false;
        }
    }
}
