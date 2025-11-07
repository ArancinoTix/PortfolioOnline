using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace U9.Motion.Timeline.MaterialTrack
{
    [TrackColor(0.8f, 0.0f, 1.0f)]
    [TrackBindingType(typeof(Material))]
    [TrackClipType(typeof(SpriteClip))]
    public class MaterialTextureControlTrack : TrackAsset
    {
        [Tooltip("The parameter the clips in this track should manipulate")]
        [SerializeField] private string m_ParameterName;

        MaterialTextureControlMixer m_Mixer;

        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            var mixer = ScriptPlayable<MaterialTextureControlMixer>.Create(graph, inputCount);

            m_Mixer = mixer.GetBehaviour();
            m_Mixer.SetParameterID(Shader.PropertyToID(m_ParameterName));

            return mixer;
        }

        protected override void OnCreateClip(TimelineClip clip)
        {
            clip.displayName = "Material (Texture)";
            base.OnCreateClip(clip);
        }
    }
}