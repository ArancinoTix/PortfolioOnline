using System;
using U9.Audio.Data;
using U9.Audio.Service;
using UnityEngine;

namespace U9.Audio.UnityAudio
{
    public class UnityAudioObject : IAudioObject
    {
        private AudioSource _source;
        private AudioId _audioId;

        private float _speedOfPlayFade;
        private float _speedOfPauseFade;
        private bool _isTransitioningPlayVolume = false;
        private bool _isTransitioningPauseVolume = false;

        private float _baseVolume;
        private float _categoryVolume;
        private float _currentVolumePlayFadeMultiplier;
        private float _currentVolumePauseFadeMultiplier;
        private float _volumeMultiplier;

        private float _transitionPlayLerp;
        private float _transitionPlayFromVolume;
        private float _transitionPlayToVolume;

        private float _transitionPauseLerp;
        private float _transitionPauseFromVolume;
        private float _transitionPauseToVolume;

        private float _delay;
        private bool _isDelayed;
        private bool _isPausing;
        private bool _isStopping;
        private bool _loopsByDefault = false;

        public UnityAudioObject(AudioSource source)
        {
            _source = source;
        }

        public AudioSource Source { get => _source; }
        public AudioId AudioId { get => _audioId; }

        public bool SourceIsInvalid()
        {
            return _source != null;
        }

        public bool IsStopping()
        {
            return _isStopping;
        }

        public bool IsPaused()
        {
            return _isPausing;
        }

        public void AssignAudio(UnityAudioIdClip clip, float categoryVolume)
        {
            _audioId = clip.AudioId;
            _currentVolumePlayFadeMultiplier = 0;
            _currentVolumePauseFadeMultiplier = 1;
            _baseVolume = clip.Volume;
            _categoryVolume = categoryVolume;
            clip.AssignTo(this);
            _loopsByDefault = Source.loop;
            _source.volume = GetVolume();
            _isTransitioningPauseVolume = false;
            _isDelayed = false;
            _isStopping = false;
            _volumeMultiplier = 1;
        }

        public void AssignParent(Transform parent)
        {
            if (parent != null)
            {
                _source.transform.SetParent(parent);
                _source.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            }
        }

        public void SetWorldPosition(Vector3 position)
        {
            _source.transform.position = position;
        }

        public void CleanUp()
        {
            ClearOnEnded();
            _source.Stop();
            _source.clip = null;
            _isDelayed = false;
            _isPausing = false;
            _isStopping = false;
            _isTransitioningPauseVolume = false;
            _isTransitioningPlayVolume = false;
        }

        public bool Update(float deltaTime)
        {
            if (_isTransitioningPauseVolume)
            {
                //Update the volume
                _transitionPauseLerp += _speedOfPauseFade * deltaTime;
                _currentVolumePauseFadeMultiplier = Mathf.Lerp(_transitionPauseFromVolume, _transitionPauseToVolume, _transitionPauseLerp);
                _source.volume = GetVolume();

                //If we completed, and we were fading out, pause the audio
                if (_transitionPauseLerp >= 1)
                {
                    _isTransitioningPauseVolume = false;
                    if (_transitionPauseToVolume < 0.5f)
                    {
                        //Finalise the pause
                        _source.Pause();
                        return false;
                    }
                }

                //We do not return, as we will still allow other processes whilst we pause
            }
            else if (IsPaused())
            {
                //We have finished transitioning to a paused state
                return false;
            }

            //If delayed, process the delay
            if (_isDelayed)
            {
                _delay -= deltaTime;
                //Of the delay ended, play
                if (_delay <= 0)
                {
                    _isDelayed = false;
                    _source.Play();
                }

                //The source has not ended
                return false;
            }
            //Are we transitioning volume?
            else if (_isTransitioningPlayVolume)
            {
                //Update the volume
                _transitionPlayLerp += _speedOfPlayFade * deltaTime;
                _currentVolumePlayFadeMultiplier = Mathf.Lerp(_transitionPlayFromVolume, _transitionPlayToVolume, _transitionPlayLerp);
                _source.volume = GetVolume();

                //If we completed, and we were fading out, stop the audio
                if (_transitionPlayLerp >= 1)
                {
                    _isTransitioningPlayVolume = false;
                    if (_transitionPlayToVolume < 0.5f)
                    {
                        _source.Stop();
                    }
                }

                //Do nothing, the source has not ended
                return false;
            }

            //Finally, check if the source has ended (not playing)
            else
                return !_source.isPlaying;
        }

        public bool IsPlaying()
        {
            return _source.isPlaying;
        }


        public void Pause(float fadeOut = 0)
        {
            _delay = 0;
            _isDelayed = false;
            _isPausing = true;

            if (fadeOut > 0)
            {
                _transitionPauseLerp = 0;
                _transitionPauseFromVolume = _currentVolumePauseFadeMultiplier;
                _transitionPauseToVolume = 0;
                _speedOfPauseFade = 1f / fadeOut;
                _isTransitioningPauseVolume = true;
            }
            else
            {
                _currentVolumePauseFadeMultiplier = 0;
                _isTransitioningPauseVolume = false;
                _source.Pause();
            }
            _source.volume = GetVolume();
        }

        private float GetVolume()
        {
            return _currentVolumePauseFadeMultiplier * _currentVolumePlayFadeMultiplier * _categoryVolume *_baseVolume* _volumeMultiplier;
        }

        public void Play(float volumeMultiplier, float fadeIn = 0, float delay = 0, float startTime = 0)
        {
            Source.loop = _loopsByDefault;
            _isPausing = false;
            _isStopping = false;
            _isTransitioningPauseVolume = false;
            _volumeMultiplier = volumeMultiplier;
            _currentVolumePlayFadeMultiplier *= _currentVolumePauseFadeMultiplier;
            _currentVolumePauseFadeMultiplier = 1;

            _delay = delay;
            _isDelayed = delay > 0;
            _source.time = startTime;

            if (fadeIn > 0)
            {
                _transitionPlayLerp = 0;
                _transitionPlayFromVolume = _currentVolumePlayFadeMultiplier;
                _transitionPlayToVolume = 1;
                _speedOfPlayFade = 1f / fadeIn;
                _isTransitioningPlayVolume = true;
                _source.volume = GetVolume();
            }
            else
            {
                _currentVolumePlayFadeMultiplier = 1;
                _isTransitioningPlayVolume = false;
                _source.volume = GetVolume();
            }

            if(!_isDelayed)
                _source.Play();
        }

        public void Play(float volumeMultiplier, bool loopOverride, float fadeIn = 0, float delay = 0, float startTime = 0)
        {
            Source.loop = loopOverride;
            _isPausing = false;
            _isStopping = false;
            _isTransitioningPauseVolume = false;
            _volumeMultiplier = volumeMultiplier;
            _currentVolumePlayFadeMultiplier *= _currentVolumePauseFadeMultiplier;
            _currentVolumePauseFadeMultiplier = 1;

            _delay = delay;
            _isDelayed = delay > 0;
            _source.time = startTime;

            if (fadeIn > 0)
            {
                _transitionPlayLerp = 0;
                _transitionPlayFromVolume = _currentVolumePlayFadeMultiplier;
                _transitionPlayToVolume = 1;
                _speedOfPlayFade = 1f / fadeIn;
                _isTransitioningPlayVolume = true;
                _source.volume = GetVolume();
            }
            else
            {
                _currentVolumePlayFadeMultiplier = 1;
                _isTransitioningPlayVolume = false;
                _source.volume = GetVolume();
            }

            if (!_isDelayed)
                _source.Play();
        }

        public void SetVolume(float volume)
        {
            _categoryVolume = volume;
            _source.volume = GetVolume();
        }

        public void SetPitch(float pitch)
        {
            _source.pitch = pitch;
        }

        public void Stop(float fadeOut = 0, float delay = 0)
        {
            _delay = delay;
            _isDelayed = delay > 0;
            _isPausing = false;
            _currentVolumePlayFadeMultiplier *= _currentVolumePauseFadeMultiplier;
            _currentVolumePauseFadeMultiplier = 1;
            _isStopping = true;

            if (fadeOut > 0)
            {
                _transitionPlayLerp = 0;
                _transitionPlayFromVolume = _currentVolumePlayFadeMultiplier;
                _transitionPlayToVolume = 0;
                _speedOfPlayFade = 1f / fadeOut;
                _isTransitioningPlayVolume = true;
            }
            else
            {
                _currentVolumePlayFadeMultiplier = 0;
                _isTransitioningPlayVolume = false;

                if(!_isDelayed)
                    _source.Stop();
            }
            _source.volume = GetVolume();
        }

        public void Unpause(float fadeIn = 0)
        {
            _isDelayed = false;
            if (fadeIn > 0)
            {
                _transitionPauseLerp = 0;
                _transitionPauseFromVolume = _currentVolumePauseFadeMultiplier;
                _transitionPauseToVolume = 1;
                _speedOfPauseFade = 1f / fadeIn;
                _isTransitioningPauseVolume = true;
            }
            else
            {
                _currentVolumePauseFadeMultiplier = 1;
                _isTransitioningPauseVolume = false;
            }

            _source.volume = GetVolume();
            _source.Play();
            _isPausing = false;
        }

        private Action onEnded;
        public Action GetOnEndedEvent()
        {
            return onEnded;
        }
        public void AddOnEnded(Action onEnded)
        {
            this.onEnded += onEnded;
        }

        public void ClearOnEnded()
        {
            onEnded = null;
        }
    }
}
