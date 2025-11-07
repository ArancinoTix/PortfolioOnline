using System.Collections;
using System.Collections.Generic;
using U9.Audio.Data;
using U9.Audio.Service;
using UnityEngine;
using UnityEngine.Pool;

namespace U9.Audio.UnityAudio
{
    public class UnityAudioService : MonoBehaviour, IAudioService
    {
        [SerializeField] private AudioSource _sourcePrefab;
        [SerializeField] private float _musicFade = .5f;
        [SerializeField] private int _maxPooledSourceCount = 20;
        [SerializeField] private UnityAudioIdClip[] _availableAudioClips;

        private Dictionary<AudioCategory, float> _volumesPerCategory;        
        private List<UnityAudioObject> _activeAudioObjects;
        private List<UnityAudioObject> _activeMusicObjects;
        private IObjectPool<UnityAudioObject> _inactiveAudioObjects;

        private bool _isInitted = false;
        private bool _isMusicPaused = false;

        private void Start()
        {
            Init();
            AudioController.Instance.RegisterAudioService(this);
            Debug.Log("### UnityAudioService: Registered Unity Audio Service");
        }

        private void Init()
        {
            if (_isInitted)
                return;

            _isInitted = true;
            _inactiveAudioObjects = new ObjectPool<UnityAudioObject>(CreatePooledAudioObject, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, true, _maxPooledSourceCount);
            _volumesPerCategory = new Dictionary<AudioCategory, float>();
            _activeAudioObjects = new List<UnityAudioObject>();
            _activeMusicObjects = new List<UnityAudioObject>();
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;

            //Process music
            for (int i = 0, ni = _activeMusicObjects.Count; i < ni; i++)
            {
                var activeSource = _activeMusicObjects[i];
                bool isValid = activeSource.SourceIsInvalid();

                //If update returns true, it needs to be released
                if (activeSource.Update(deltaTime) || !isValid)
                {
                    Debug.Log("### UnityAudioService.Update: Music object ended");
                    activeSource.GetOnEndedEvent()?.Invoke();
                    activeSource.CleanUp();
                    _activeMusicObjects.RemoveAt(i);
                    i--;
                    ni--;

                    if (isValid)
                        _inactiveAudioObjects.Release(activeSource);
                    else
                    {
                        //Do nothing, the object is worthless and will be cleaned up.
                        //Can occur if it was parented to a destroyed object.
                    }
                }
            }

            //Process other audio objects
            for (int i =0, ni = _activeAudioObjects.Count; i<ni; i++)
            {
                var activeSource = _activeAudioObjects[i];
                bool isValid = activeSource.SourceIsInvalid();

                //If update returns true, it needs to be released
                if (activeSource.Update(deltaTime) || !isValid)
                {
                    Debug.Log("### UnityAudioService.Update: Audio object ended");
                    activeSource.GetOnEndedEvent()?.Invoke();
                    activeSource.CleanUp();
                    _activeAudioObjects.RemoveAt(i);
                    i--;
                    ni--;

                    if(isValid)
                        _inactiveAudioObjects.Release(activeSource);
                    else
                    {
                        //Do nothing, the object is worthless and will be cleaned up.
                        //Can occur if it was parented to a destroyed object.
                    }
                }
            }
        }

        private UnityAudioObject CreatePooledAudioObject()
        {
            var source = Instantiate<AudioSource>(_sourcePrefab,transform);
            source.enabled = false;
            return new UnityAudioObject(source);
        }

        void OnReturnedToPool(UnityAudioObject audioObject)
        {
            if (audioObject.SourceIsInvalid())
            {
                audioObject.CleanUp();
                audioObject.Source.enabled = false;
                audioObject.Source.transform.SetParent(transform);
            }
        }

        void OnTakeFromPool(UnityAudioObject audioObject)
        {
            if (audioObject.SourceIsInvalid())
                audioObject.Source.enabled = true;
        }

        void OnDestroyPoolObject(UnityAudioObject audioObject)
        {
            if(audioObject.SourceIsInvalid())
                Destroy(audioObject.Source.gameObject);
        }

        private UnityAudioObject GetAvailableAudioObject()
        {
            return _inactiveAudioObjects.Get();
        }

        public float GetVolume(AudioCategory category)
        {
            if (_volumesPerCategory.ContainsKey(category))
                return _volumesPerCategory[category];
            else
                return 1;
        }

        public void SetVolume(AudioCategory category, float volume)
        {
            if (category == null)
                return;

            if (_volumesPerCategory.ContainsKey(category))
                _volumesPerCategory[category] = volume;
            else
                _volumesPerCategory.Add(category,volume);

            foreach(var activeAudioObject in _activeAudioObjects)
            {
                if(activeAudioObject.AudioId.Category == category)
                {
                    activeAudioObject.SetVolume(volume);
                }
            }
        }

        private UnityAudioIdClip GetClip(AudioId audioId)
        {
            if (audioId == null)
                return null;

            foreach(var clip in _availableAudioClips)
            {
                if (clip.AudioId == audioId)
                    return clip;
            }

            return null;
        }

        public IAudioObject PlayMusic(AudioId audioId, float volume, float delay = 0, float startTime = 0)
        {
            var clip = GetClip(audioId);
            if (clip != null)
            {
                Debug.Log($"### UnityAudioService.PlayMusic: Playing [{audioId.name}]");

                var audioObject = GetAvailableAudioObject();
                audioObject.AssignAudio(clip, GetVolume(audioId.Category));
                StopMusic(_musicFade, delay);
                audioObject.Play(volume, _musicFade, delay, startTime);
                _activeMusicObjects.Add(audioObject);
                return audioObject;
            }
            else
            {
                Debug.Log($"### UnityAudioService.PlayMusic: No matching clip for [{audioId.name}] was found");
                return null;
            }
        }

        public IAudioObject PlayMusic(AudioId audioId, float volume, bool loopOverride, float delay = 0, float startTime = 0)
        {
            var clip = GetClip(audioId);
            if (clip != null)
            {
                Debug.Log($"### UnityAudioService.PlayMusic: Playing [{audioId.name}]");

                var audioObject = GetAvailableAudioObject();
                audioObject.AssignAudio(clip, GetVolume(audioId.Category));
                StopMusic(_musicFade, delay);
                audioObject.Play(volume, loopOverride, _musicFade, delay, startTime);
                _activeMusicObjects.Add(audioObject);
                return audioObject;
            }
            else
            {
                Debug.Log($"### UnityAudioService.PlayMusic: No matching clip for [{audioId.name}] was found");
                return null;
            }
        }

        public IAudioObject PlayMusic(AudioId audioId, Vector3 worldPosition, float volume, float delay = 0, float startTime = 0)
        {
            var obj = (UnityAudioObject)PlayMusic(audioId,volume,delay,startTime);
            if (obj != null)
            {
                obj.SetWorldPosition(worldPosition);
            }
            return obj;
        }

        public IAudioObject PlayMusic(AudioId audioId, Vector3 worldPosition, float volume, bool loopOverride, float delay = 0, float startTime = 0)
        {
            var obj = (UnityAudioObject)PlayMusic(audioId, volume, loopOverride, delay, startTime);
            if (obj != null)
            {
                obj.SetWorldPosition(worldPosition);
            }
            return obj;
        }

        public IAudioObject PlayMusic(AudioId audioId, Transform parent, float volume, float delay = 0, float startTime = 0)
        {
            var obj = (UnityAudioObject)PlayMusic(audioId, volume, delay, startTime);
            if (obj != null)
            {
                obj.AssignParent(parent);
            }
            return obj;
        }

        public IAudioObject PlayMusic(AudioId audioId, Transform parent, float volume, bool loopOverride, float delay = 0, float startTime = 0)
        {
            var obj = (UnityAudioObject)PlayMusic(audioId, volume, loopOverride, delay, startTime);
            if (obj != null)
            {
                obj.AssignParent(parent);
            }
            return obj;
        }

        public IAudioObject GetCurrentMusic()
        {
            foreach(var obj in _activeMusicObjects)
            {
                //we could be transitioning between tracks, so we want the one not stopping
                if (obj.IsPlaying() && !obj.IsStopping())
                    return obj;
            }
            return null;
        }
        public void StopMusic(float fadeOut = 0, float delay = 0)
        {
            foreach(var obj in _activeMusicObjects)
            {
                if (!obj.IsStopping())
                    obj.Stop(fadeOut, delay);
            }
        }

        public void PauseMusic(float fadeOut = 0)
        {
            if (!_isMusicPaused)
            {
                _isMusicPaused = true;
                foreach (var obj in _activeMusicObjects)
                {
                    obj.Pause(fadeOut);
                }
            }
        }

        public bool IsMusicPaused()
        {
            return _isMusicPaused;
        }

        public void UnpauseMusic(float fadeIn = 0)
        {
            if (!_isMusicPaused)
            {
                _isMusicPaused = false;
                foreach (var obj in _activeMusicObjects)
                {
                    obj.Unpause(fadeIn);
                }
            }
        }       

        public IAudioObject Play(AudioId audioId, float volume, float delay = 0, float startTime = 0)
        {
            var clip = GetClip(audioId);
            if (clip != null)
            {
                Debug.Log($"### UnityAudioService.Play: Playing [{audioId.name}]");
                var audioObject = GetAvailableAudioObject();
                audioObject.AssignAudio(clip, GetVolume(audioId.Category));
                _activeAudioObjects.Add(audioObject);
                audioObject.Play(volume, 0, delay, startTime);
                return audioObject;
            }
            else
            {
                Debug.Log($"### UnityAudioService.Play: No matching clip for [{audioId.name}] was found");
                return null;
            }
        }

        public IAudioObject Play(AudioId audioId, float volume, bool loopOverride, float delay = 0, float startTime = 0)
        {
            var clip = GetClip(audioId);
            if (clip != null)
            {
                Debug.Log($"### UnityAudioService.Play: Playing [{audioId.name}]");
                var audioObject = GetAvailableAudioObject();
                audioObject.AssignAudio(clip, GetVolume(audioId.Category));
                _activeAudioObjects.Add(audioObject);
                audioObject.Play(volume, loopOverride, 0, delay, startTime);
                return audioObject;
            }
            else
            {
                Debug.Log($"### UnityAudioService.Play: No matching clip for [{audioId.name}] was found");
                return null;
            }
        }

        public IAudioObject Play(AudioId audioId, Transform parent, float volume, float delay = 0, float startTime = 0)
        {
            var obj = (UnityAudioObject)Play(audioId, volume, delay, startTime);
            if (obj != null)
            {
                obj.AssignParent(parent);
            }
            return obj;
        }

        public IAudioObject Play(AudioId audioId, Transform parent, float volume, bool loopOverride, float delay = 0, float startTime = 0)
        {
            var obj = (UnityAudioObject)Play(audioId, volume, loopOverride, delay, startTime);
            if (obj != null)
            {
                obj.AssignParent(parent);
            }
            return obj;
        }

        public IAudioObject Play(AudioId audioId, Vector3 worldPosition, float volume, float delay = 0, float startTime = 0)
        {
            var obj = (UnityAudioObject)Play(audioId, volume, delay, startTime);
            if (obj != null)
            {
                obj.SetWorldPosition(worldPosition);
            }
            return obj;
        }

        public IAudioObject Play(AudioId audioId, Vector3 worldPosition, float volume, bool loopOverride, float delay = 0, float startTime = 0)
        {
            var obj = (UnityAudioObject)Play(audioId, volume, loopOverride, delay, startTime);
            if (obj != null)
            {
                obj.SetWorldPosition(worldPosition);
            }
            return obj;
        }

        public void Stop(AudioId audioId, float fadeOut = 0)
        {
            foreach (var audioObject in _activeAudioObjects)
            {
                if(audioObject.AudioId == audioId)
                    audioObject.Stop(fadeOut);
            }
        }

        public void Stop(AudioCategory category, float fadeOut = 0)
        {
            foreach (var audioObject in _activeAudioObjects)
            {
                if (audioObject.AudioId.Category == category)
                    audioObject.Stop(fadeOut);
            }
        }

        public void Pause(AudioId audioId, float fadeOut = 0)
        {
            foreach (var audioObject in _activeAudioObjects)
            {
                if (audioObject.AudioId == audioId)
                    audioObject.Pause(fadeOut);
            }
        }

        public void Pause(AudioCategory category, float fadeOut = 0)
        {
            foreach (var audioObject in _activeAudioObjects)
            {
                if (audioObject.AudioId.Category == category)
                    audioObject.Pause(fadeOut);
            }
        }

        public void Unpause(AudioId audioId, float fadeIn = 0)
        {
            foreach (var audioObject in _activeAudioObjects)
            {
                if (audioObject.AudioId == audioId)
                    audioObject.Unpause(fadeIn);
            }
        }

        public void Unpause(AudioCategory category, float fadeIn = 0)
        {
            foreach (var audioObject in _activeAudioObjects)
            {
                if (audioObject.AudioId.Category == category)
                    audioObject.Unpause(fadeIn);
            }
        }

        public bool IsPlaying(AudioId audioId)
        {
            foreach (var audioObject in _activeAudioObjects)
            {
                if (audioObject.AudioId == audioId && audioObject.IsPlaying() && !audioObject.IsStopping())
                    return true;
            }

            return false;
        }        
    }
}
