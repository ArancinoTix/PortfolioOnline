using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace U9.Audio.Data
{
    [CreateAssetMenu(fileName = "Audio", menuName = "UNIT9/Audio/Audio Id")]
    public class AudioId : ScriptableObject
    {
        [SerializeField] private AudioCategory _category;

        public AudioCategory Category => _category;

#if UNITY_EDITOR
        public void SetCategory(AudioCategory category)
        {
            _category = category;
        }
#endif
    }
}
