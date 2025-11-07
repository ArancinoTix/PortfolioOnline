using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace U9.Motion.Timeline.BespokeTrack
{
    [TrackColor(1.0f, 0.2f, 0.2f)]
    [TrackBindingType(typeof(Transform))]
    [TrackClipType(typeof(Vector3Clip))]
    public class ScaleControlTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            PrepareClips();

            return ScriptPlayable<ScaleControlMixer>.Create(graph, inputCount);
        }

        protected override void OnCreateClip(TimelineClip clip)
        {
            clip.displayName = "Scale";
            PrepareClip(clip, true);

            base.OnCreateClip(clip);
        }

        private void PrepareClips()
        {
            foreach (var clip in GetClips())
            {
                PrepareClip(clip);
            }
        }

        private void PrepareClip(TimelineClip clip, bool initial = false)
        {
            var asset = clip.asset as Vector3Clip;
            if (asset != null)
            {
                // Lock the toggle for world space toggle as we only support local scaling
                asset.values.spaceUnlocked = false;

                // Unlock the toggle to switch to relative scaling
                asset.values.relativeUnlocked = true;

                if (initial)
                {
                    // Start scaling at 1 by default
                    asset.values.startAt = Vector3.one;
                    asset.values.endAt = Vector3.one;
                }
            }
        }
    }
}