using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

namespace U9.Motion.Timeline
{
    public class ImageColorControlMixer : PlayableBehaviour
    {
        private Color defaultColor;
        private Color blendedColor;
        private Image image;
        private bool firstFrameHappened;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            image = playerData as Image;

            if (image != null)
            {
                if (!firstFrameHappened)
                {
                    defaultColor = image.color;

                    // Caluclate all the clips on this tracks, Start and End times
                    RecalculateAllClipsStartAndEnd(playable);

                    firstFrameHappened = true;
                }

                blendedColor = Color.clear;

                float blendedWeight = 0.0f;
                bool onATrack = false;

                double time = PlayableExtensions.GetTime(playable);

                for (int I = 0; I < playable.GetInputCount(); I++)
                {
                    float inputWeight = playable.GetInputWeight(I);
                    blendedWeight += inputWeight;

                    ScriptPlayable<TypedControlBehaviour<Color>> inputPlayable = (ScriptPlayable<TypedControlBehaviour<Color>>)playable.GetInput(I);
                    TypedControlBehaviour<Color> behaviour = inputPlayable.GetBehaviour();

                    // If not on on any clip at all yet then use the last clips endAt
                    if (blendedWeight == 0)
                    {
                        // Find out if we are after it
                        if (time >= behaviour.end)
                        {
                            // If told to hold then the value is this unless a clip, that is in play, comes along
                            if (behaviour.clipExtrapolation == ClipExtrapolation.Hold)
                                image.color = GetValue(behaviour, 1.0f);
                            else if (behaviour.clipExtrapolation == ClipExtrapolation.Default)
                                image.color = defaultColor;
                        }
                        // If we are before the first track then use default value
                        else if (I == 0)
                            image.color = defaultColor;

                        // Go to next input clip
                        continue;
                    }
                    else
                    {
                        blendedColor += GetValue(behaviour, (float)(inputPlayable.GetTime() / inputPlayable.GetDuration())) * inputWeight;

                        // We are on at least one clip, so we will use the blended value (in case there are multiple clips at this point on the track)
                        onATrack = true;
                    }
                }

                if (onATrack)
                    image.color = blendedColor;
            }
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            firstFrameHappened = false;

            if (image != null)
            {
                image.color = defaultColor;
            }
        }

        private Color GetValue(TypedControlBehaviour<Color> behaviour, float normalizedTime)
        {
            return Color.Lerp(behaviour.startAt, behaviour.endAt, behaviour.curve.Evaluate(normalizedTime));
        }

        private void RecalculateAllClipsStartAndEnd(Playable playable)
        {
            var director = playable.GetGraph().GetResolver() as PlayableDirector;
            if (director != null && director.playableAsset is TimelineAsset timelineAsset)
            {
                foreach (var track in timelineAsset.GetOutputTracks())
                {
                    var trackBinding = director.GetGenericBinding(track) as Image;
                    if (trackBinding == image)
                    {
                        foreach (var clip in track.GetClips())
                        {
                            var asset = clip.asset as ColorClip;
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
