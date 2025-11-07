using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace U9.Motion.Timeline
{
    public class Vector3Clip : PlayableAsset, ITimelineClipAsset
    {
        public Vector3ControlBehaviour values = new();

        public ClipCaps clipCaps => ClipCaps.Blending;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return ScriptPlayable<Vector3ControlBehaviour>.Create(graph, values);
        }
    }
}