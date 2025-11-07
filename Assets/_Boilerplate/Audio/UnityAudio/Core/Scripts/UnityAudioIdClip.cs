using System.Collections;
using System.Collections.Generic;
using U9.Audio.Data;
using UnityEngine;

namespace U9.Audio.UnityAudio
{
    [System.Serializable]
    public class UnityAudioIdClip
    {
        [SerializeField] AudioClip _clip;
        [SerializeField] AudioId _audioId;
        [SerializeField] bool _loop = false;
        [SerializeField] [Range(0, 256)] int _priority = 128;
        [SerializeField] [Range(0,1)] float _volume = 1;
        [SerializeField] [Range(-3, 3)] float _pitch = 1;
        [SerializeField] [Range(-1, 1)] float _stereoPan;
        [SerializeField] [Range(0, 1)] float _spatialBlend;

        public AudioId AudioId { get => _audioId; }
        public float Volume { get => _volume;  }

        public void AssignTo(UnityAudioObject obj)
        {
            obj.Source.clip = _clip;
            obj.Source.loop = _loop;
            obj.Source.priority = _priority;
            obj.Source.pitch = _pitch;
            obj.Source.panStereo = _stereoPan;
            obj.Source.spatialBlend = _spatialBlend;
        }
    }
}
