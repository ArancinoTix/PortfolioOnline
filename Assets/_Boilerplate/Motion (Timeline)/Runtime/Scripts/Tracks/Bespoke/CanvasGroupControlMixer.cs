using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace U9.Motion.Timeline
{
    public class CanvasGroupControlMixer : PlayableBehaviour
    {
        private float defaultValue;
        private float blendedValue;
        private CanvasGroup canvasGroup;
        private bool firstFrameHappened;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            canvasGroup = playerData as CanvasGroup;

            if (canvasGroup != null)
            {
                if (!firstFrameHappened)
                {
                    defaultValue = canvasGroup.alpha;

                    // Caluclate all the clips on this tracks, Start and End times
                    RecalculateAllClipsStartAndEnd(playable);

                    firstFrameHappened = true;
                }

                blendedValue = 0;

                float blendedWeight = 0.0f;
                bool onATrack = false;

                double time = PlayableExtensions.GetTime(playable);

                for (int I = 0; I < playable.GetInputCount(); I++)
                {
                    float inputWeight = playable.GetInputWeight(I);
                    blendedWeight += inputWeight;

                    ScriptPlayable<TypedControlBehaviour<float>> inputPlayable = (ScriptPlayable<TypedControlBehaviour<float>>)playable.GetInput(I);
                    TypedControlBehaviour<float> behaviour = inputPlayable.GetBehaviour();

                    // If not on on any clip at all yet then use the last clips endAt
                    if (blendedWeight == 0)
                    {
                        // Find out if we are after it
                        if (time >= behaviour.end)
                        {
                            // If told to hold then the value is this unless a clip, that is in play, comes along
                            if (behaviour.clipExtrapolation == ClipExtrapolation.Hold)
                                canvasGroup.alpha = GetValue(behaviour, 1.0f);
                            else if (behaviour.clipExtrapolation == ClipExtrapolation.Default)
                                canvasGroup.alpha = defaultValue;
                        }
                        // If we are before the first track then use default value
                        else if (I == 0)
                            canvasGroup.alpha = defaultValue;

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

                if (onATrack)
                    canvasGroup.alpha = blendedValue;
            }
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            firstFrameHappened = false;

            if (canvasGroup != null)
            {
                canvasGroup.alpha = defaultValue;
            }
        }

        private float GetValue(TypedControlBehaviour<float> behaviour, float normalizedTime)
        {
            return Mathf.Lerp(behaviour.startAt, behaviour.endAt, behaviour.curve.Evaluate(normalizedTime));
        }

        private void RecalculateAllClipsStartAndEnd(Playable playable)
        {
            var director = playable.GetGraph().GetResolver() as PlayableDirector;
            if (director != null && director.playableAsset is TimelineAsset timelineAsset)
            {
                foreach (var track in timelineAsset.GetOutputTracks())
                {
                    var trackBinding = director.GetGenericBinding(track) as CanvasGroup;
                    if (trackBinding == canvasGroup)
                    {
                        foreach (var clip in track.GetClips())
                        {
                            var asset = clip.asset as FloatClip;
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