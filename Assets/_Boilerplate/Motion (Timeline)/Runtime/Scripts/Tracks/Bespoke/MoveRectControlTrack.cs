using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace U9.Motion.Timeline.BespokeTrack
{
    [TrackColor(0.0f, 0.0f, 1.0f)]
    [TrackBindingType(typeof(RectTransform))]
    [TrackClipType(typeof(Vector3Clip))]
    public class MoveRectControlTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            PrepareClips();

            return ScriptPlayable<MoveRectControlMixer>.Create(graph, inputCount);
        }

        protected override void OnCreateClip(TimelineClip clip)
        {
            clip.displayName = "Move (Rect)";

            PrepareClip(clip);

            base.OnCreateClip(clip);
        }

        private void PrepareClips()
        {
            foreach (var clip in GetClips())
            {
                PrepareClip(clip);
            }
        }

        private void PrepareClip(TimelineClip clip)
        {
            var asset = clip.asset as Vector3Clip;
            if (asset != null)
            {
                // Lock the toggle for world space toggle as we use anchored positioning only
                asset.values.spaceUnlocked = false;

                // Unlock the toggle to switch to relative movement
                asset.values.relativeUnlocked = true;
            }
        }
    }
}