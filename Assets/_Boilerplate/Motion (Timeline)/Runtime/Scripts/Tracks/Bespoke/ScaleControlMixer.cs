using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace U9.Motion.Timeline
{
    public class ScaleControlMixer : PlayableBehaviour
    {
        private Vector3 defaultScale;
        private Vector3 blendedScale;
        private Transform transform;
        private bool firstFrameHappened;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            transform = playerData as Transform;

            if (transform != null)
            {
                if (!firstFrameHappened)
                {
                    defaultScale = transform.localScale;

                    // Caluclate all the clips on this tracks, Start and End times
                    RecalculateAllClipsStartAndEnd(playable);

                    firstFrameHappened = true;
                }

                blendedScale = Vector3.zero;

                float blendedWeight = 0.0f;
                bool onATrack = false;

                double time = PlayableExtensions.GetTime(playable);

                for (int I = 0; I < playable.GetInputCount(); I++)
                {
                    float inputWeight = playable.GetInputWeight(I);
                    blendedWeight += inputWeight;

                    ScriptPlayable<Vector3ControlBehaviour> inputPlayable = (ScriptPlayable<Vector3ControlBehaviour>)playable.GetInput(I);
                    Vector3ControlBehaviour behaviour = inputPlayable.GetBehaviour();

                    // If not on on any clip at all yet then use the last clips endAt
                    if (blendedWeight == 0)
                    {
                        // Find out if we are after it
                        if (time >= behaviour.end)
                        {
                            // If told to hold then the value is this unless a clip, that is in play, comes along
                            if (behaviour.clipExtrapolation == ClipExtrapolation.Hold)
                                transform.localScale = GetValue(behaviour, 1.0f);
                            else if (behaviour.clipExtrapolation == ClipExtrapolation.Default)
                                transform.localScale = defaultScale;
                        }
                        // If we are before the first track then use default value
                        else if (I == 0)
                            transform.localScale = defaultScale;

                        // Go to next input clip
                        continue;
                    }
                    else
                    {
                        blendedScale += GetValue(behaviour, (float)(inputPlayable.GetTime() / inputPlayable.GetDuration())) * inputWeight;

                        // We are on at least one clip, so we will use the blended value (in case there are multiple clips at this point on the track)
                        onATrack = true;
                    }
                }

                if (onATrack)
                    transform.localScale = blendedScale;
            }
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            firstFrameHappened = false;

            if (transform != null)
            {
                transform.localScale = defaultScale;
            }
        }

        private Vector3 GetValue(Vector3ControlBehaviour behaviour, float normalizedTime)
        {
            if(behaviour.useRelative)
                return new Vector3(behaviour.xEnabled ? Mathf.Lerp(behaviour.startAt.x, behaviour.endAt.x, behaviour.curveX.Evaluate(normalizedTime)) + defaultScale.x : defaultScale.x,
                                   behaviour.yEnabled ? Mathf.Lerp(behaviour.startAt.y, behaviour.endAt.y, behaviour.curveY.Evaluate(normalizedTime)) + defaultScale.y : defaultScale.y,
                                   behaviour.zEnabled ? Mathf.Lerp(behaviour.startAt.z, behaviour.endAt.z, behaviour.curveZ.Evaluate(normalizedTime)) + defaultScale.z : defaultScale.z);

            return new Vector3(behaviour.xEnabled ? Mathf.Lerp(behaviour.startAt.x, behaviour.endAt.x, behaviour.curveX.Evaluate(normalizedTime)) : defaultScale.x,
                               behaviour.yEnabled ? Mathf.Lerp(behaviour.startAt.y, behaviour.endAt.y, behaviour.curveY.Evaluate(normalizedTime)) : defaultScale.y,
                               behaviour.zEnabled ? Mathf.Lerp(behaviour.startAt.z, behaviour.endAt.z, behaviour.curveZ.Evaluate(normalizedTime)) : defaultScale.z);
        }

        private void RecalculateAllClipsStartAndEnd(Playable playable)
        {
            var director = playable.GetGraph().GetResolver() as PlayableDirector;
            if (director != null && director.playableAsset is TimelineAsset timelineAsset)
            {
                foreach (var track in timelineAsset.GetOutputTracks())
                {
                    var trackBinding = director.GetGenericBinding(track) as Transform;
                    if (trackBinding == transform)
                    {
                        foreach (var clip in track.GetClips())
                        {
                            var asset = clip.asset as Vector3Clip;
                            if (asset != null)
                            {
                                asset.values.start = clip.start;
                                asset.values.end = clip.end;
                            }
                        }
                    }
                }
            }
        }
    }
}
