using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

namespace U9.Motion.Timeline
{
    public class ImageSequenceControlMixer : PlayableBehaviour
    {
        private Sprite blendedValue;
        private Sprite defaultSprite;
        private Image image;
        private bool firstFrameHappened;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            image = playerData as Image;

            if (image != null)
            {
                if (!firstFrameHappened)
                {
                    defaultSprite = image.sprite;

                    // Caluclate all the clips on this tracks, Start and End times
                    RecalculateAllClipsStartAndEnd(playable);

                    firstFrameHappened = true;
                }

                bool onATrack = false;

                double time = PlayableExtensions.GetTime(playable);

                for (int I = 0; I < playable.GetInputCount(); I++)
                {
                    float inputWeight = playable.GetInputWeight(I);

                    ScriptPlayable<SpriteControlBehaviour> inputPlayable = (ScriptPlayable<SpriteControlBehaviour>)playable.GetInput(I);
                    SpriteControlBehaviour behaviour = inputPlayable.GetBehaviour();

                    // If not on a clip at all yet then use the last clips final value
                    if (inputWeight == 0)
                    {
                        // Find out if we are after it
                        if (time > behaviour.end)
                        {
                            // If told to hold then the value is this unless a clip, that is in play, comes along
                            if (behaviour.clipExtrapolation == ClipExtrapolation.Hold)
                                image.sprite = GetValue(behaviour, 1.0f);
                            else if (behaviour.clipExtrapolation == ClipExtrapolation.Default)
                                image.sprite = defaultSprite;
                        }
                        // If we are before the first track then use default value
                        else if (I == 0)
                            image.sprite = defaultSprite;

                        // Go to next input clip
                        continue;
                    }
                    else
                    {
                        blendedValue = GetValue(behaviour, (float)(inputPlayable.GetTime() / inputPlayable.GetDuration()));

                        // We are on at least one clip, so we will use the last, as mixing doesnt make sense here!
                        onATrack = true;
                    }
                }

                if (onATrack)
                    image.sprite = blendedValue;
            }
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            firstFrameHappened = false;

            if (image != null)
            {
                image.sprite = defaultSprite;
            }
        }

        private Sprite GetValue(SpriteControlBehaviour behaviour, float normalizedTime)
        {
            return behaviour.sprites[(int)Mathf.Lerp(0, behaviour.sprites.Length - 1, normalizedTime + 0.01f)];
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
                            var asset = clip.asset as SpriteClip;
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
