using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace U9.Motion.Timeline.BespokeTrack
{
    [TrackColor(0.0f, 1.0f, 0.0f)]
    [TrackBindingType(typeof(Transform))]
    [TrackClipType(typeof(Vector3Clip))]
    public class RotateControlTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            PrepareClips();

            return ScriptPlayable<RotateControlMixer>.Create(graph, inputCount);
        }

        protected override void OnCreateClip(TimelineClip clip)
        {
            clip.displayName = "Rotate";
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
                // Unlock the toggle for world space toggle
                asset.values.spaceUnlocked = true;

                // Unlock the toggle to switch to relative
                asset.values.relativeUnlocked = true;
            }
        }
    }
}