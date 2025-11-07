using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using U9.Audio.Data;

namespace U9.Audio.Service
{
    public interface IAudioService
    {
        /// <summary>
        /// Gets the volume of the audio category
        /// </summary>
        /// <returns></returns>
        public float GetVolume(AudioCategory category);

        /// <summary>
        /// Sets the volume of the audio category
        /// </summary>
        /// <param name="volume"></param>
        public void SetVolume(AudioCategory category, float volume);

        /// <summary>
        /// Applies the volume of the given settings
        /// </summary>
        /// <param name="categorySettings"></param>
        public void SetVolume(AudioCategorySettings categorySettings)
        {
            SetVolume(categorySettings.Category, categorySettings.Volume);
        }

        /// <summary>
        /// Plays music
        /// </summary>
        /// <param name="audioId">Id of the music to play</param>
        /// <param name="volume">volume to play it at</param>
        /// <param name="delay">Delay before it plays</param>
        /// <param name="startTime">Start playing it at the specified offset</param>
        /// <returns></returns>
        public IAudioObject PlayMusic(AudioId audioId, float volume, float delay = 0, float startTime = 0);

        /// <summary>
        /// Plays music
        /// </summary>
        /// <param name="audioId">Id of the music to play</param>
        /// <param name="volume">volume to play it at</param>
        /// <param name="loopOverride">override the loop paremeter</param>
        /// <param name="delay">Delay before it plays</param>
        /// <param name="startTime">Start playing it at the specified offset</param>
        /// <returns></returns>
        public IAudioObject PlayMusic(AudioId audioId, float volume, bool loopOverride, float delay = 0, float startTime = 0);

        /// <summary>
        /// Plays music
        /// </summary>
        /// <param name="audioId">Id of the music to play</param>
        /// <param name="worldPosition">World position to play at</param>
        /// <param name="volume">volume to play it at</param>
        /// <param name="delay">Delay before it plays</param>
        /// <param name="startTime">Start playing it at the specified offset</param>
        /// <returns></returns>
        public IAudioObject PlayMusic(AudioId audioId, Vector3 worldPosition, float volume, float delay = 0, float startTime = 0);

        /// <summary>
        /// Plays music
        /// </summary>
        /// <param name="audioId">Id of the music to play</param>
        /// <param name="worldPosition">World position to play at</param>
        /// <param name="volume">volume to play it at</param>
        /// <param name="loopOverride">override the loop paremeter</param>
        /// <param name="delay">Delay before it plays</param>
        /// <param name="startTime">Start playing it at the specified offset</param>
        /// <returns></returns>
        public IAudioObject PlayMusic(AudioId audioId, Vector3 worldPosition, float volume, bool loopOverride, float delay = 0, float startTime = 0);

        /// <summary>
        /// Plays music
        /// </summary>
        /// <param name="audioId">Id of the music to play</param>
        /// <param name="parent">Transform to assign the audio to</param>
        /// <param name="volume">volume to play it at</param>
        /// <param name="delay">Delay before it plays</param>
        /// <param name="startTime">Start playing it at the specified offset</param>
        /// <returns></returns>
        public IAudioObject PlayMusic(AudioId audioId, Transform parent, float volume, float delay = 0, float startTime = 0);

        /// <summary>
        /// Plays music
        /// </summary>
        /// <param name="audioId">Id of the music to play</param>
        /// <param name="parent">Transform to assign the audio to</param>
        /// <param name="volume">volume to play it at</param>
        /// <param name="loopOverride">override the loop paremeter</param>
        /// <param name="delay">Delay before it plays</param>
        /// <param name="startTime">Start playing it at the specified offset</param>
        /// <returns></returns>
        public IAudioObject PlayMusic(AudioId audioId, Transform parent, float volume, bool loopOverride, float delay = 0, float startTime = 0);

        /// <summary>
        /// Returns the current music playing
        /// </summary>
        /// <returns></returns>
        public IAudioObject GetCurrentMusic();

        /// <summary>
        /// Stops the active music
        /// </summary>
        /// <param name="fadeOut">How long to fade it out over</param>
        public void StopMusic(float fadeOut = 0, float delay = 0);

        /// <summary>
        /// Pauses the active music
        /// </summary>
        /// <param name="fadeOut">How long to fade it out over</param>
        public void PauseMusic(float fadeOut = 0);

        /// <summary>
        /// Is the music paused?
        /// </summary>
        /// <returns></returns>
        public bool IsMusicPaused();

        /// <summary>
        /// Unpauses the music
        /// </summary>
        /// <param name="fadeIn">Duration to fade the music in over</param>
        public void UnpauseMusic(float fadeIn = 0);

        /// <summary>
        /// Plays the specified audio (SFX etc)
        /// </summary>
        /// <param name="audio">Id of the audio to play</param>
        /// <param name="volume">Volume to play it at</param>
        /// <param name="delay">Delay before it plays </param>
        /// <param name="startTime">Start playing it at the specified offset</param>
        /// <returns></returns>
        public IAudioObject Play(AudioId audio, float volume, float delay = 0, float startTime = 0);

        /// <summary>
        /// Plays the specified audio (SFX etc)
        /// </summary>
        /// <param name="audio">Id of the audio to play</param>
        /// <param name="volume">Volume to play it at</param>
        /// <param name="loopOverride">override the loop paremeter</param>
        /// <param name="delay">Delay before it plays </param>
        /// <param name="startTime">Start playing it at the specified offset</param>
        /// <returns></returns>
        public IAudioObject Play(AudioId audio, float volume, bool loopOverride, float delay = 0, float startTime = 0);

        /// <summary>
        /// Plays the specified audio (SFX etc)
        /// </summary>
        /// <param name="audio">Id of the audio to play</param>
        /// <param name="parent">Transform to assign to the Audio</param>
        /// <param name="volume">Volume to play it at</param>
        /// <param name="delay">Delay before it plays</param>
        /// <param name="startTime">Start playing it at the specified offse</param>
        /// <returns></returns>
        public IAudioObject Play(AudioId audio, Transform parent, float volume, float delay = 0, float startTime = 0);

        /// <summary>
        /// Plays the specified audio (SFX etc)
        /// </summary>
        /// <param name="audio">Id of the audio to play</param>
        /// <param name="parent">Transform to assign to the Audio</param>
        /// <param name="volume">Volume to play it at</param>
        /// <param name="loopOverride">override the loop paremeter</param>
        /// <param name="delay">Delay before it plays</param>
        /// <param name="startTime">Start playing it at the specified offse</param>
        /// <returns></returns>
        public IAudioObject Play(AudioId audio, Transform parent, float volume, bool loopOverride, float delay = 0, float startTime = 0);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="audio">Id of the audio to play</param>
        /// <param name="worldPosition">Location to play it at</param>
        /// <param name="volume">Volume to play it at</param>
        /// <param name="delay">Delay before it plays</param>
        /// <param name="startTime">Start playing it at the specified offse</param>
        /// <returns></returns>
        public IAudioObject Play(AudioId audio, Vector3 worldPosition, float volume, float delay = 0, float startTime = 0);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="audio">Id of the audio to play</param>
        /// <param name="worldPosition">Location to play it at</param>
        /// <param name="volume">Volume to play it at</param>
        /// <param name="loopOverride">override the loop paremeter</param>
        /// <param name="delay">Delay before it plays</param>
        /// <param name="startTime">Start playing it at the specified offse</param>
        /// <returns></returns>
        public IAudioObject Play(AudioId audio, Vector3 worldPosition, float volume, bool loopOverride, float delay = 0, float startTime = 0);

        /// <summary>
        /// Stops all audio that is playing with this Audio Id
        /// </summary>
        /// <param name="audioId">Id of the audio to stop</param>
        /// <param name="fadeOut">Duration to fade out over</param>
        public void Stop(AudioId audioId, float fadeOut = 0);

        /// <summary>
        /// Stops all audio in the specified category
        /// </summary>
        /// <param name="category">Category of audio to stop</param>
        /// <param name="fadeOut">Duration to fade out over</param>
        public void Stop(AudioCategory category, float fadeOut = 0);

        /// <summary>
        /// Pauses the audio matching the specific Id
        /// </summary>
        /// <param name="audioId">Id of the audio to pause</param>
        /// <param name="fadeOut">duration to fade out over</param>
        public void Pause(AudioId audioId, float fadeOut = 0);

        /// <summary>
        /// Pauses the audio matching the category
        /// </summary>
        /// <param name="category">Category of audio to pause</param>
        /// <param name="fadeOut">duration to fade out over</param>
        public void Pause(AudioCategory category, float fadeOut = 0);

        /// <summary>
        /// Unpauses the audio matching the specific Id
        /// </summary>
        /// <param name="audioId">Id of the audio to unpause</param>
        /// <param name="fadeIn">duration to fade in over</param>
        public void Unpause(AudioId audioId, float fadeIn = 0);

        /// <summary>
        /// Unpauses the audio matching the category
        /// </summary>
        /// <param name="category">Category of audio to unpause</param>
        /// <param name="fadeIn">duration to fade in over</param>
        public void Unpause(AudioCategory category, float fadeIn = 0);

        /// <summary>
        /// Is any audio with a matching Id playing?
        /// </summary>
        /// <param name="audioId">Id of audio to check for</param>
        /// <returns></returns>
        public bool IsPlaying(AudioId audioId);
    }
}