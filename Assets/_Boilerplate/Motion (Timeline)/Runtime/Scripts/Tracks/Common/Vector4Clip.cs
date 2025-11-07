using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace U9.Motion.Timeline
{
    public class Vector4Clip : PlayableAsset, ITimelineClipAsset
    {
        public Vector4ControlBehaviour values = new();

        public ClipCaps clipCaps => ClipCaps.Blending;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return ScriptPlayable<Vector4ControlBehaviour>.Create(graph, values);
        }
    }
}