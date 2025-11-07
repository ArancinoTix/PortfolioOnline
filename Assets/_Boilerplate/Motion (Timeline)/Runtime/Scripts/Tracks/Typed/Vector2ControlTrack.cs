using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace U9.Motion.Timeline.TypedTrack
{
    [TrackColor(1.0f, 0.8f, 0.8f)]
    [TrackBindingType(typeof(Component))]
    [TrackClipType(typeof(Vector2Clip))]
    public class Vector2ControlTrack : TrackAsset
    {
        [Tooltip("The parameter the clips in this track should manipulate")]
        [SerializeField] private string m_ParameterName;

        Vector2ControlMixer m_Mixer;

        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            var mixer = ScriptPlayable<Vector2ControlMixer>.Create(graph, inputCount);

            m_Mixer = mixer.GetBehaviour();
            m_Mixer.SetParameterName(m_ParameterName);

            return mixer;
        }

        protected override void OnCreateClip(TimelineClip clip)
        {
            clip.displayName = "Vector2 Clip";
            base.OnCreateClip(clip);
        }
        
    }
}