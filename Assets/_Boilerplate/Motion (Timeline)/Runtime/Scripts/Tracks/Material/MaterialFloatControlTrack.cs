using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace U9.Motion.Timeline.MaterialTrack
{
    [TrackColor(0.8f, 1.0f, 0.0f)]
    [TrackBindingType(typeof(Material))]
    [TrackClipType(typeof(FloatClip))]
    public class MaterialFloatControlTrack : TrackAsset
    {
        [Tooltip("The parameter the clips in this track should manipulate")]
        [SerializeField] private string m_ParameterName;

        MaterialFloatControlMixer m_Mixer;

        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            var mixer = ScriptPlayable<MaterialFloatControlMixer>.Create(graph, inputCount);

            m_Mixer = mixer.GetBehaviour();
            m_Mixer.SetParameterID(Shader.PropertyToID(m_ParameterName));

            return mixer;
        }

        protected override void OnCreateClip(TimelineClip clip)
        {
            clip.displayName = "Material (Float)";
            base.OnCreateClip(clip);
        }
    }
}