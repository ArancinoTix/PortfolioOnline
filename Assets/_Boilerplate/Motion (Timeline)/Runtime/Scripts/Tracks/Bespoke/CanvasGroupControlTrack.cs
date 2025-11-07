using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace U9.Motion.Timeline.BespokeTrack
{
    [TrackColor(0.0f, 1.0f, 1.0f)]
    [TrackBindingType(typeof(CanvasGroup))]
    [TrackClipType(typeof(FloatClip))]
    public class CanvasGroupControlTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            return ScriptPlayable<CanvasGroupControlMixer>.Create(graph, inputCount);
        }

        protected override void OnCreateClip(TimelineClip clip)
        {
            clip.displayName = "Canvas Group (Alpha)";
            base.OnCreateClip(clip);
        }
    }
}