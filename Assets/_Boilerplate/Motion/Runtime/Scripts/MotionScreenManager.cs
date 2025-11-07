using System;
using System.Collections.Generic;
using UnityEngine;

namespace U9.Motion
{
    /// <summary>
    /// A simple interface to control a group of MotionScreens
    /// </summary>
    public class MotionScreenManager : MonoBehaviour
    {
        [SerializeField] private List<MotionScreen> m_Screens = new List<MotionScreen>();

        public Action<int> MotionScreenedOpened;
        public Action<int> MotionScreenClosed;

        private int m_CurrentActiveScreen = -1;
        public int CurrentScreen { get { return m_CurrentActiveScreen; } }

        private void Awake()
        {
            for(int I = 0; I < m_Screens.Count; I++)
            {
                int V = I;
                m_Screens[I].OpenScreenMotion.OnPlayMotion.AddListener(ScreenChanging);
                m_Screens[I].OpenScreenMotion.OnFinishedMotion.AddListener(() => ScreenOpened(V));
                m_Screens[I].CloseScreenMotion.OnPlayMotion.AddListener(ScreenChanging);
                m_Screens[I].CloseScreenMotion.OnFinishedMotion.AddListener(() => ScreenClosed(V));
            }
        }

        /// <summary>
        /// Disable all interactivity in realation to this canvas group hierachy
        /// this is then re-enabled only when open motion has finished.
        /// </summary>
        private void ScreenChanging()
        {
          

        }

        private void ScreenOpened(int screenID)
        {
            MotionScreenedOpened?.Invoke(screenID);
        }

        private void ScreenClosed(int screenID)
        {
            if (m_CurrentActiveScreen != -1)
            {
                m_Screens[m_CurrentActiveScreen].OpenScreen();
                MotionScreenClosed?.Invoke(screenID);
            }
            else
            {
                foreach (var screen in m_Screens)
                    screen.ResetScreen();
            }  
        }

        /// <summary>
        /// Closes previous screen (if there was one), then opens up the selected screen
        /// </summary>
        /// <param name="screenID"></param>
        public void SwitchToScreen(int screenID)
        {
            // If the screen to go to exists
            if(screenID >= 0 && m_Screens.Count > screenID)
            {
                // Kill all screen motions in case one was playing
                foreach (var screen in m_Screens)
                {
                    if(screen.CloseScreenMotion)
                        screen.CloseScreenMotion.Kill();
                    if(screen.OpenScreenMotion)
                        screen.OpenScreenMotion.Kill();
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
                    m_Screens[m_CurrentActiveScreen].OpenScreen();
                }

            }
            // If screen to go to is -1, then actually just close the active screen (if one is active)
            else if(screenID == -1 && m_CurrentActiveScreen != -1)
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
            if(m_CurrentActiveScreen != -1 && m_CurrentActiveScreen != 0)
            {
                SwitchToScreen(m_CurrentActiveScreen - 1);
                return true;
            }

            // Couldn't go back
            return false;
        }
    }
}
