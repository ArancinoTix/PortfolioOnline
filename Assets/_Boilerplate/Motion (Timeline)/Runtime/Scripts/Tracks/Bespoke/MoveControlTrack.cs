using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace U9.Motion.Timeline.BespokeTrack
{
    [TrackColor(0.2f, 0.2f, 1.0f)]
    [TrackBindingType(typeof(Transform))]
    [TrackClipType(typeof(Vector3Clip))]
    public class MoveControlTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            PrepareClips();

            return ScriptPlayable<MoveControlMixer>.Create(graph, inputCount);
        }

        protected override void OnCreateClip(TimelineClip clip)
        {
            clip.displayName = "Move";
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
                // UnLock the toggle for world space toggle
                asset.values.spaceUnlocked = true;

                // Unlock the toggle to switch to relative movement
                asset.values.relativeUnlocked = true;
            }
        }
    }
}
