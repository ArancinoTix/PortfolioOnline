using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

namespace U9.Motion.Timeline.BespokeTrack
{
    [TrackColor(1.0f, 1.0f, 0.0f)]
    [TrackBindingType(typeof(Image))]
    [TrackClipType(typeof(ColorClip))]
    public class ImageColorControlTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            return ScriptPlayable<ImageColorControlMixer>.Create(graph, inputCount);
        }

        protected override void OnCreateClip(TimelineClip clip)
        {
            clip.displayName = "Image (Color)";
            base.OnCreateClip(clip);
        }
    }
}