using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace U9.Audio.Service
{
    public interface IAudioObject
    {
        public void SetVolume(float volume);
        public void SetPitch(float pitch);
        public bool IsPlaying();
        public bool IsPaused();
        public void Play(float volume, float fadeIn = 0, float delay = 0, float startTime = 0);
        public void Play(float volume, bool loopOverride, float fadeIn = 0, float delay = 0, float startTime = 0);
        public void Stop(float fadeOut = 0, float delay = 0);
        public void Pause(float fadeOut = 0);
        public void Unpause(float fadeIn = 0);

        public Action GetOnEndedEvent();
        public void AddOnEnded(Action onEnded);
        public void ClearOnEnded();
    }
}
