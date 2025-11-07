using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace U9.Motion.Timeline
{
    public class ScaleRectControlMixer : PlayableBehaviour
    {
        private Vector2 defaultScale;
        private Vector2 blendedScale;
        private RectTransform rectTransform;
        private bool firstFrameHappened;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            rectTransform = playerData as RectTransform;

            if (rectTransform != null)
            {
                if (!firstFrameHappened)
                {
                    defaultScale = rectTransform.sizeDelta;

                    // Caluclate all the clips on this tracks, Start and End times
                    RecalculateAllClipsStartAndEnd(playable);

                    firstFrameHappened = true;
                }

                blendedScale = Vector2.zero;

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
                            // If told to hold then the value is this unless a clip, that is in play, comes along
                            if (behaviour.clipExtrapolation == ClipExtrapolation.Hold)
                                rectTransform.sizeDelta = GetValue(behaviour, 1.0f);
                            else if (behaviour.clipExtrapolation == ClipExtrapolation.Default)
                                rectTransform.sizeDelta = defaultScale;
                        }
                        // If we are before the first track then use default value
                        else if (I == 0)
                            rectTransform.sizeDelta = defaultScale;

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
                    rectTransform.sizeDelta = blendedScale;
            }
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            firstFrameHappened = false;

            if (rectTransform != null)
            {
                rectTransform.sizeDelta = defaultScale;
            }
        }

        private Vector2 GetValue(MultiAxisControlBehaviour<Vector2> behaviour, float normalizedTime)
        {
            if(behaviour.useRelative)
                return new Vector2(behaviour.xEnabled ? Mathf.Lerp(behaviour.startAt.x, behaviour.endAt.x, behaviour.curveX.Evaluate(normalizedTime)) + defaultScale.x : defaultScale.x,
                                   behaviour.yEnabled ? Mathf.Lerp(behaviour.startAt.y, behaviour.endAt.y, behaviour.curveY.Evaluate(normalizedTime)) + defaultScale.y : defaultScale.y);

            return new Vector2(behaviour.xEnabled ? Mathf.Lerp(behaviour.startAt.x, behaviour.endAt.x, behaviour.curveX.Evaluate(normalizedTime)) : defaultScale.x,
                               behaviour.yEnabled ? Mathf.Lerp(behaviour.startAt.y, behaviour.endAt.y, behaviour.curveY.Evaluate(normalizedTime)) : defaultScale.y);
        }

        private void RecalculateAllClipsStartAndEnd(Playable playable)
        {
            var director = playable.GetGraph().GetResolver() as PlayableDirector;
            if (director != null && director.playableAsset is TimelineAsset timelineAsset)
            {
                foreach (var track in timelineAsset.GetOutputTracks())
                {
                    var trackBinding = director.GetGenericBinding(track) as RectTransform;
                    if (trackBinding == rectTransform)
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