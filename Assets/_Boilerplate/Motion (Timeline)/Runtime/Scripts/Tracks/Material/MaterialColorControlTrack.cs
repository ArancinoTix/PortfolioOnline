using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace U9.Motion.Timeline.MaterialTrack
{
    [TrackColor(1.0f, 0.5f, 0.0f)]
    [TrackBindingType(typeof(Material))]
    [TrackClipType(typeof(ColorClip))]

    public class MaterialColorControlTrack : TrackAsset
    {
        [Tooltip("The parameter the clips in this track should manipulate")]
        [SerializeField] private string m_ParameterName;

        MaterialColorControlMixer m_Mixer;

        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            var mixer = ScriptPlayable<MaterialColorControlMixer>.Create(graph, inputCount);

            m_Mixer = mixer.GetBehaviour();
            m_Mixer.SetParameterID(Shader.PropertyToID(m_ParameterName));

            return mixer;
        }

        protected override void OnCreateClip(TimelineClip clip)
        {
            clip.displayName = "Material (Color)";
            base.OnCreateClip(clip);
        }
    }
}