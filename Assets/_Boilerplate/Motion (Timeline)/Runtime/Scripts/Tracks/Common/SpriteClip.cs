using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace U9.Motion.Timeline
{
    public class SpriteClip : PlayableAsset, ITimelineClipAsset
    {
        public SpriteControlBehaviour values = new();

        public ClipCaps clipCaps => ClipCaps.None;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return ScriptPlayable<SpriteControlBehaviour>.Create(graph, values);
        }
    }
}