using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

namespace U9.Audio.Data
{

    [CreateAssetMenu(fileName = "AudioCategory", menuName = "UNIT9/Audio/Category")]
    public class AudioCategory : ScriptableObject
    {
        [SerializeField] [Range(0, 1)] private float _defaultVolume = 1;
        [SerializeField] private LocalizedString _localisedDisplayName;

        public float DefaultVolume => _defaultVolume; 
        public LocalizedString LocalizedDisplayName => _localisedDisplayName;  
        public string ID => name;
    }
}
