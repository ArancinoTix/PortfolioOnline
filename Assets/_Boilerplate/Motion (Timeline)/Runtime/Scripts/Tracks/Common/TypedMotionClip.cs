using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace U9.Motion.Timeline
{
    public class TypedMotionClip<T> : PlayableAsset, ITimelineClipAsset
    {
        public TypedControlBehaviour<T> values = new TypedControlBehaviour<T>();

        public ClipCaps clipCaps => ClipCaps.Blending;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return ScriptPlayable<TypedControlBehaviour<T>>.Create(graph, values);
        }
    }
}