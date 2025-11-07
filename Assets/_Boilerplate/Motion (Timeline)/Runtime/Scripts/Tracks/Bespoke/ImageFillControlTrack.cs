using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

namespace U9.Motion.Timeline.BespokeTrack
{
    [TrackColor(1.0f, 1.0f, 0.4f)]
    [TrackBindingType(typeof(Image))]
    [TrackClipType(typeof(FloatClip))]
    public class ImageFillControlTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            return ScriptPlayable<ImageFillControlMixer>.Create(graph, inputCount);
        }

        protected override void OnCreateClip(TimelineClip clip)
        {
            clip.displayName = "Image (Fill)";
            base.OnCreateClip(clip);
        }
    }
}