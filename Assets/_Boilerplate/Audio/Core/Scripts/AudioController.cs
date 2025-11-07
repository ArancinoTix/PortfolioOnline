using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using U9.SaveDataManagement;
using U9.Audio.Data;
using Cysharp.Threading.Tasks;
using U9.Audio.Service;

namespace U9.Audio
{
    public class AudioController : MonoSingleton<AudioController>
    {
        private IAudioService _audioService;
        [SerializeField] private AudioCategory _masterCategory;
        [SerializeField] private AudioCategory _bgmCategory;
        [SerializeField] private AudioCategory[] _availableSubCategories;
        [SerializeField] private AudioSettingsDataStructure _audioSettings;

#if UNITY_EDITOR
        [SerializeField] private string _configSavePath = "Assets/_Project/Audio/Configurations/Audio Ids";
        public string ConfigSavePath { get => _configSavePath; set => _configSavePath = value; }

#endif

        public AudioCategory[] AvailableSubCategories => _availableSubCategories;

        public AudioCategory MasterCategory => _masterCategory;

        public AudioCategory BgmCategory { get => _bgmCategory;  }

        // Start is called before the first frame update
        void Awake()
        {
            _audioSettings = new AudioSettingsDataStructure(_masterCategory, _bgmCategory, _availableSubCategories);
            _audioSettings.onUpdated += ApplySettings;
            Instance = this;
        }

        public void RegisterAudioService(IAudioService audioService)
        {
            _audioService = audioService;
        }

        public AudioCategory[] GetAllCategories()
        {
            List<AudioCategory> categories = new List<AudioCategory>();
            if (_bgmCategory != null)
                categories.Add(_bgmCategory);

            foreach (var c in _availableSubCategories)
                if (c != null)
                    categories.Add(c);

            return categories.ToArray();
        }

        private void Start()
        {
            SaveDataController.Instance.AddData(_audioSettings);
        }

        public async UniTask<SaveDataServiceResponse> LoadData()
        {
            //Add if not already added
            SaveDataController.Instance.AddData(_audioSettings);
            var result = await SaveDataController.Instance.Load<AudioSettingsDataStructure>();

            _audioSettings.Repair(_masterCategory, _bgmCategory, _availableSubCategories);

            if (result.isSuccessful)
            {
                //Service did not have a chance to register yet
                while (_audioService == null)
                    await UniTask.DelayFrame(1);
                ApplySettings(_audioSettings);
            }


            return result;
        }

        private void ApplySettings(AudioSettingsDataStructure updatedData)
        {
            _audioService.SetVolume(_audioSettings.BgmVolumeSetting.Category, _audioSettings.BgmVolumeSetting.Volume*_audioSettings.MasterVolumeSetting.Volume);

            foreach (var setting in _audioSettings.OtherCategoryVolumeSettings)
                _audioService.SetVolume(setting.Category, setting.Volume* _audioSettings.MasterVolumeSetting.Volume);
        }

        private float GetVolume(AudioCategory category)
        {
            var volume = _audioSettings.MasterVolumeSetting.Volume;
            var subSetting = _audioSettings.GetSettingsForCategory(category);

            if (subSetting != null)
                volume *= subSetting.Volume;

            return volume;
        }

        /// <summary>
        /// Plays music
        /// </summary>
        /// <param name="audioId">Id of the music to play</param>
        /// <param name="volume">volume to play it at</param>
        /// <param name="delay">Delay before it plays</param>
        /// <param name="startTime">Start playing it at the specified offset</param>
        /// <returns></returns>
        public IAudioObject PlayMusic(AudioId audioId, float volume, float delay = 0, float startTime = 0)
        {
            return _audioService.PlayMusic(audioId, volume, delay, startTime);
        }

        /// <summary>
        /// Plays music
        /// </summary>
        /// <param name="audioId">Id of the music to play</param>
        /// <param name="volume">volume to play it at</param>
        /// <param name="loopOverride">override the loop paremeter</param>
        /// <param name="delay">Delay before it plays</param>
        /// <param name="startTime">Start playing it at the specified offset</param>
        /// <returns></returns>
        public IAudioObject PlayMusic(AudioId audioId, float volume, bool loopOverride, float delay = 0, float startTime = 0)
        {
            return _audioService.PlayMusic(audioId, volume, loopOverride, delay, startTime);
        }

        /// <summary>
        /// Plays music
        /// </summary>
        /// <param name="audioId">Id of the music to play</param>
        /// <param name="worldPosition">World position to play at</param>
        /// <param name="volume">volume to play it at</param>
        /// <param name="delay">Delay before it plays</param>
        /// <param name="startTime">Start playing it at the specified offset</param>
        /// <returns></returns>
        public IAudioObject PlayMusic(AudioId audioId, Vector3 worldPosition, float volume, float delay = 0, float startTime = 0)
        {
            return _audioService.PlayMusic(audioId, worldPosition, volume, delay, startTime);
        }

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
        public IAudioObject PlayMusic(AudioId audioId, Vector3 worldPosition, float volume, bool loopOverride, float delay = 0, float startTime = 0)
        {
            return _audioService.PlayMusic(audioId, worldPosition, volume, loopOverride, delay, startTime);
        }

        /// <summary>
        /// Plays music
        /// </summary>
        /// <param name="audioId">Id of the music to play</param>
        /// <param name="parent">Transform to assign the audio to</param>
        /// <param name="volume">volume to play it at</param>
        /// <param name="delay">Delay before it plays</param>
        /// <param name="startTime">Start playing it at the specified offset</param>
        /// <returns></returns>
        public IAudioObject PlayMusic(AudioId audioId, Transform parent, float volume, float delay = 0, float startTime = 0)
        {
            return _audioService.PlayMusic(audioId, parent, volume, delay, startTime);
        }

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
        public IAudioObject PlayMusic(AudioId audioId, Transform parent, float volume, bool loopOverride, float delay = 0, float startTime = 0)
        {
            return _audioService.PlayMusic(audioId, parent, volume, loopOverride, delay, startTime);
        }

        /// <summary>
        /// Returns the current music playing
        /// </summary>
        /// <returns></returns>
        public IAudioObject GetCurrentMusic()
        {
            return _audioService.GetCurrentMusic();
        }

        /// <summary>
        /// Stops the active music
        /// </summary>
        /// <param name="fadeOut">How long to fade it out over</param>
        public void StopMusic(float fadeOut = 0)
        {
            _audioService.StopMusic(fadeOut);
        }

        /// <summary>
        /// Pauses the active music
        /// </summary>
        /// <param name="fadeOut">How long to fade it out over</param>
        public void PauseMusic(float fadeOut = 0)
        {
            _audioService.PauseMusic(fadeOut);
        }

        /// <summary>
        /// Is the music paused?
        /// </summary>
        /// <returns></returns>
        public bool IsMusicPaused()
        {
           return _audioService.IsMusicPaused();
        }

        /// <summary>
        /// Unpauses the music
        /// </summary>
        /// <param name="fadeIn">Duration to fade the music in over</param>
        public void UnpauseMusic(float fadeIn = 0)
        {
            _audioService.UnpauseMusic(fadeIn);
        }

        /// <summary>
        /// Plays the specified audio (SFX etc)
        /// </summary>
        /// <param name="audio">Id of the audio to play</param>
        /// <param name="volume">Volume to play it at</param>
        /// <param name="delay">Delay before it plays </param>
        /// <param name="startTime">Start playing it at the specified offset</param>
        /// <returns></returns>
        public IAudioObject Play(AudioId audio, float volume, float delay = 0, float startTime = 0)
        {
            return _audioService.Play(audio, volume, delay, startTime);
        }

        /// <summary>
        /// Plays the specified audio (SFX etc)
        /// </summary>
        /// <param name="audio">Id of the audio to play</param>
        /// <param name="volume">Volume to play it at</param>
        /// <param name="loopOverride">override the loop paremeter</param>
        /// <param name="delay">Delay before it plays </param>
        /// <param name="startTime">Start playing it at the specified offset</param>
        /// <returns></returns>
        public IAudioObject Play(AudioId audio, float volume, bool loopOverride, float delay = 0, float startTime = 0)
        {
            return _audioService.Play(audio, volume, loopOverride, delay, startTime);
        }

        /// <summary>
        /// Plays the specified audio (SFX etc)
        /// </summary>
        /// <param name="audio">Id of the audio to play</param>
        /// <param name="parent">Transform to assign to the Audio</param>
        /// <param name="volume">Volume to play it at</param>
        /// <param name="delay">Delay before it plays</param>
        /// <param name="startTime">Start playing it at the specified offse</param>
        /// <returns></returns>
        public IAudioObject Play(AudioId audio, Transform parent, float volume, float delay = 0, float startTime = 0)
        {
            return _audioService.Play(audio, parent, volume, delay, startTime);
        }

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
        public IAudioObject Play(AudioId audio, Transform parent, float volume, bool loopOverride, float delay = 0, float startTime = 0)
        {
            return _audioService.Play(audio, parent, volume, loopOverride, delay, startTime);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="audio">Id of the audio to play</param>
        /// <param name="worldPosition">Location to play it at</param>
        /// <param name="volume">Volume to play it at</param>
        /// <param name="delay">Delay before it plays</param>
        /// <param name="startTime">Start playing it at the specified offse</param>
        /// <returns></returns>
        public IAudioObject Play(AudioId audio, Vector3 worldPosition, float volume, float delay = 0, float startTime = 0)
        {
            return _audioService.Play(audio, worldPosition, volume, delay, startTime);
        }

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
        public IAudioObject Play(AudioId audio, Vector3 worldPosition, float volume, bool loopOverride, float delay = 0, float startTime = 0)
        {
            return _audioService.Play(audio, worldPosition, volume, loopOverride, delay, startTime);
        }

        /// <summary>
        /// Stops all audio that is playing with this Audio Id
        /// </summary>
        /// <param name="audioId">Id of the audio to stop</param>
        /// <param name="fadeOut">Duration to fade out over</param>
        public void Stop(AudioId audioId, float fadeOut = 0)
        {
            _audioService.Stop(audioId,fadeOut);
        }

        /// <summary>
        /// Stops all audio in the specified category
        /// </summary>
        /// <param name="category">Category of audio to stop</param>
        /// <param name="fadeOut">Duration to fade out over</param>
        public void Stop(AudioCategory category, float fadeOut = 0)
        {
            _audioService.Stop(category, fadeOut);
        }

        /// <summary>
        /// Pauses the audio matching the specific Id
        /// </summary>
        /// <param name="audioId">Id of the audio to pause</param>
        /// <param name="fadeOut">duration to fade out over</param>
        public void Pause(AudioId audioId, float fadeOut = 0)
        {
            _audioService.Pause(audioId, fadeOut);
        }

        /// <summary>
        /// Pauses the audio matching the category
        /// </summary>
        /// <param name="category">Category of audio to pause</param>
        /// <param name="fadeOut">duration to fade out over</param>
        public void Pause(AudioCategory category, float fadeOut = 0)
        {
            _audioService.Pause(category, fadeOut);
        }

        /// <summary>
        /// Unpauses the audio matching the specific Id
        /// </summary>
        /// <param name="audioId">Id of the audio to unpause</param>
        /// <param name="fadeIn">duration to fade in over</param>
        public void Unpause(AudioId audioId, float fadeIn = 0)
        {
            _audioService.Unpause(audioId, fadeIn);
        }

        /// <summary>
        /// Unpauses the audio matching the category
        /// </summary>
        /// <param name="category">Category of audio to unpause</param>
        /// <param name="fadeIn">duration to fade in over</param>
        public void Unpause(AudioCategory category, float fadeIn = 0)
        {
            _audioService.Unpause(category, fadeIn);
        }

        /// <summary>
        /// Is any audio with a matching Id playing?
        /// </summary>
        /// <param name="audioId">Id of audio to check for</param>
        /// <returns></returns>
        public bool IsPlaying(AudioId audioId)
        {
            return _audioService.IsPlaying(audioId);
        }
    }
}
