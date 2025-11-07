using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace U9.Motion.Timeline
{
    public class Vector2Clip : PlayableAsset, ITimelineClipAsset
    {
        public MultiAxisControlBehaviour<Vector2> values = new();

        public ClipCaps clipCaps => ClipCaps.Blending;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return ScriptPlayable<MultiAxisControlBehaviour<Vector2>>.Create(graph, values);
        }
    }
}