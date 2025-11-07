using System;
using U9.SaveDataManagement;
using UnityEngine;

namespace U9.Audio.Data
{
    [System.Serializable]
    public class AudioSettingsDataStructure : BaseSaveDataStructure
    {
        [SerializeField] private AudioCategorySettings _masterVolumeSetting;
        [SerializeField] private AudioCategorySettings _bgmVolumeSetting;
        [SerializeField] private AudioCategorySettings[] _otherVolumeSettings;

        public Action<AudioSettingsDataStructure> onUpdated;

        public AudioSettingsDataStructure(AudioCategory masterCategory, AudioCategory musicCategory, AudioCategory[] availableSubCategories)
        {
            _masterVolumeSetting = new AudioCategorySettings(masterCategory);
            _bgmVolumeSetting = new AudioCategorySettings(musicCategory);
            _otherVolumeSettings = new AudioCategorySettings[availableSubCategories.Length];

            for(int i = 0, ni = _otherVolumeSettings.Length; i<ni;i++)
            {
                _otherVolumeSettings[i] = new AudioCategorySettings(availableSubCategories[i]);
            }
        }

        /// <summary>
        /// Incase references are lost
        /// </summary>
        /// <param name="masterCategory"></param>
        /// <param name="musicCategory"></param>
        /// <param name="availableSubCategories"></param>
        public void Repair(AudioCategory masterCategory, AudioCategory musicCategory, AudioCategory[] availableSubCategories)
        {
            if (_masterVolumeSetting.Category == null || _masterVolumeSetting.Category != masterCategory)
                _masterVolumeSetting = new AudioCategorySettings(masterCategory, _masterVolumeSetting.Volume);
            if (_bgmVolumeSetting.Category == null || _bgmVolumeSetting.Category != musicCategory)
                _bgmVolumeSetting = new AudioCategorySettings(musicCategory, _bgmVolumeSetting.Volume);

            if (availableSubCategories.Length != _otherVolumeSettings.Length)
            {
                _otherVolumeSettings = new AudioCategorySettings[availableSubCategories.Length];

                for (int i = 0, ni = _otherVolumeSettings.Length; i < ni; i++)
                {
                    _otherVolumeSettings[i] = new AudioCategorySettings(availableSubCategories[i]);
                }
            }
            else
            {
                for (int i = 0, ni = _otherVolumeSettings.Length; i < ni; i++)
                {
                    if (_otherVolumeSettings[i].Category == null || _otherVolumeSettings[i].Category != availableSubCategories[i])
                        _otherVolumeSettings[i] = new AudioCategorySettings(availableSubCategories[i], _otherVolumeSettings[i].Volume);
                }
            }
        }

        public AudioCategorySettings[] OtherCategoryVolumeSettings => _otherVolumeSettings;  

        public AudioCategorySettings MasterVolumeSetting => _masterVolumeSetting;

        public AudioCategorySettings BgmVolumeSetting  => _bgmVolumeSetting;

        public AudioCategorySettings GetSettingsForCategory(AudioCategory category)
        {
            if (category == _masterVolumeSetting.Category)
                return _masterVolumeSetting;
            else if (category == _bgmVolumeSetting.Category)
                return _bgmVolumeSetting;
            else
            {
                foreach (var setting in _otherVolumeSettings)
                {
                    if (setting.Category == category)
                        return setting;
                }
            }

            return null;
        }
    }

    [System.Serializable]
    public class AudioCategorySettings
    {
        [SerializeField] private AudioCategory _category;
        [SerializeField] [Range(0,1)] private float _volume;

        public AudioCategorySettings(AudioCategory category, float volume)
        {
            _category = category;
            _volume = Mathf.Clamp01(volume);
        }
        public AudioCategorySettings(AudioCategory category)
        {
            _category = category;
            _volume = Mathf.Clamp01(category.DefaultVolume);
        }

        public AudioCategory Category => _category;
        public float Volume
        {
            get => _volume;
            set
            {
                _volume = Mathf.Clamp01(value);
            }
        }
    }
}
