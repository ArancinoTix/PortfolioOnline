using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace U9.Motion.Timeline
{
    public class Vector2ControlMixer : TypedMotionControlMixer<Vector2>
    {
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            component = playerData as Component;

            if (component != null && parameterName != null)
            {
                if (!firstFrameHappened)
                {
                    type = component.GetType();
                    pInfo = type.GetProperty(parameterName);
                    if (pInfo != null && pInfo.CanWrite && !pInfo.IsDefined(typeof(ObsoleteAttribute), true))
                    {
                        defaultValue = (Vector2)pInfo.GetValue(component);
                    }

                    // Caluclate all the clips on this tracks, Start and End times
                    RecalculateAllClipsStartAndEnd(playable);

                    firstFrameHappened = true;
                }

                blendedValue = Vector2.zero;

                float blendedWeight = 0.0f;
                bool onATrack = false;

                double time = PlayableExtensions.GetTime(playable);

                for (int I = 0; I < playable.GetInputCount(); I++)
                {
                    float inputWeight = playable.GetInputWeight(I);
                    blendedWeight += inputWeight;

                    ScriptPlayable<MultiAxisControlBehaviour<Vector2>> inputPlayable = (ScriptPlayable<MultiAxisControlBehaviour<Vector2>>)playable.GetInput(I);
                    MultiAxisControlBehaviour<Vector2> behaviour = inputPlayable.GetBehaviour();

                    // If not on on any clip at all yet then use the last clips endAt
                    if (blendedWeight == 0)
                    {
                        // Find out if we are after it
                        if (time >= behaviour.end)
                        {
                            if (component != null && parameterName != null && pInfo != null)
                            {
                                // If told to hold then the value is this unless a clip, that is in play, comes along
                                if (behaviour.clipExtrapolation == ClipExtrapolation.Hold)
                                    pInfo.SetValue(component, GetValue(behaviour, 1.0f));
                                else if (behaviour.clipExtrapolation == ClipExtrapolation.Default)
                                    pInfo.SetValue(component, defaultValue);
                            }
                        }
                        // If we are before the first track then use default value
                        else if (I == 0)
                            pInfo.SetValue(component, defaultValue);

                        // Go to next input clip
                        continue;
                    }
                    else
                    {
                        blendedValue += GetValue(behaviour, (float)(inputPlayable.GetTime() / inputPlayable.GetDuration())) * inputWeight;

                        // We are on at least one clip, so we will use the blended value (in case there are multiple clips at this point on the track)
                        onATrack = true;
                    }
                }

                if (component != null && parameterName != null && pInfo != null && onATrack)
                    pInfo.SetValue(component, blendedValue);
            }
        }

        private Vector2 GetValue(MultiAxisControlBehaviour<Vector2> behaviour, float normalizedTime)
        {
            return new Vector2(behaviour.xEnabled ? Mathf.Lerp(behaviour.startAt.x, behaviour.endAt.x, behaviour.curveX.Evaluate(normalizedTime)) : defaultValue.x,
                               behaviour.yEnabled ? Mathf.Lerp(behaviour.startAt.y, behaviour.endAt.y, behaviour.curveY.Evaluate(normalizedTime)) : defaultValue.y);
        }

        private void RecalculateAllClipsStartAndEnd(Playable playable)
        {
            var director = playable.GetGraph().GetResolver() as PlayableDirector;
            if (director != null && director.playableAsset is TimelineAsset timelineAsset)
            {
                foreach (var track in timelineAsset.GetOutputTracks())
                {
                    var trackBinding = director.GetGenericBinding(track) as Component;
                    if (trackBinding == component)
                    {
                        foreach (var clip in track.GetClips())
                        {
                            var asset = clip.asset as Vector2Clip;
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
