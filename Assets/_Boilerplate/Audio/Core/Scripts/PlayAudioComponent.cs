using System.Collections;
using System.Collections.Generic;
using U9.Audio.Data;
using UnityEngine;

namespace U9.Audio
{
    public class PlayAudioComponent : MonoBehaviour
    {
        [SerializeField] private AudioId _audioIdToTrigger;
        [SerializeField] [Range(0, 1)] float _volume = 1;

        public void Play()
        {
            var instance = AudioController.Instance.Play(_audioIdToTrigger, _volume);
        }
    }
}
