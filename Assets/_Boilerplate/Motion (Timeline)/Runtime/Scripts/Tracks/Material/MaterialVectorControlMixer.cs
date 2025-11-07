using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace U9.Motion.Timeline
{
    public class MaterialVectorControlMixer : PlayableBehaviour
    {
        private Vector4 defaultVector;
        private Material material;
        private bool firstFrameHappened;
        private int parameterID;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            material = playerData as Material;

            if (material != null && material.HasProperty(parameterID))
            {
                if (!firstFrameHappened)
                {

                    defaultVector = material.GetVector(parameterID);

                    // Caluclate all the clips on this tracks, Start and End times
                    RecalculateAllClipsStartAndEnd(playable);

                    firstFrameHappened = true;
                }

                Vector4 blendedVector = Vector4.zero;

                float blendedWeight = 0.0f;
                bool onATrack = false;

                double time = PlayableExtensions.GetTime(playable);

                for (int I = 0; I < playable.GetInputCount(); I++)
                {
                    float inputWeight = playable.GetInputWeight(I);
                    blendedWeight += inputWeight;

                    ScriptPlayable<Vector4ControlBehaviour> inputPlayable = (ScriptPlayable<Vector4ControlBehaviour>)playable.GetInput(I);
                    Vector4ControlBehaviour behaviour = inputPlayable.GetBehaviour();

                    // If not on on any clip at all yet then use the last clips endAt
                    if (blendedWeight == 0)
                    {
                        // Find out if we are after it
                        if (time > behaviour.end)
                        {
                            // If told to hold then the value is this unless a clip, that is in play, comes along
                            if (behaviour.clipExtrapolation == ClipExtrapolation.Hold)
                                material.SetVector(parameterID, GetValue(behaviour, 1.0f));
                            else if (behaviour.clipExtrapolation == ClipExtrapolation.Default)
                                material.SetVector(parameterID, defaultVector);
                        }
                        // If we are before the first track then use default value
                        else if (I == 0)
                            material.SetVector(parameterID, defaultVector);

                        // Go to next input clip
                        continue;
                    }
                    else
                    {
                        blendedVector += GetValue(behaviour, (float)(inputPlayable.GetTime() / inputPlayable.GetDuration())) * inputWeight;

                        // We are on at least one clip, so we will use the blended value (in case there are multiple clips at this point on the track)
                        onATrack = true;
                    }
                }

                if (onATrack)
                    material.SetVector(parameterID, blendedVector);
            }
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            firstFrameHappened = false;

            if (material != null && material.HasProperty(parameterID))
            {
                material.SetVector(parameterID, defaultVector);
            }
        }

        public void SetParameterID(int parameterID)
        {
            this.parameterID = parameterID;
        }

        private Vector4 GetValue(Vector4ControlBehaviour behaviour, float normalizedTime)
        {
            return new Vector4(behaviour.xEnabled ? Mathf.Lerp(behaviour.startAt.x, behaviour.endAt.x, behaviour.curveX.Evaluate(normalizedTime)) : defaultVector.x,
                               behaviour.yEnabled ? Mathf.Lerp(behaviour.startAt.y, behaviour.endAt.y, behaviour.curveY.Evaluate(normalizedTime)) : defaultVector.y,
                               behaviour.zEnabled ? Mathf.Lerp(behaviour.startAt.z, behaviour.endAt.z, behaviour.curveZ.Evaluate(normalizedTime)) : defaultVector.z,
                               behaviour.wEnabled ? Mathf.Lerp(behaviour.startAt.w, behaviour.endAt.w, behaviour.curveW.Evaluate(normalizedTime)) : defaultVector.w);
        }

        private void RecalculateAllClipsStartAndEnd(Playable playable)
        {
            var director = playable.GetGraph().GetResolver() as PlayableDirector;
            if (director != null && director.playableAsset is TimelineAsset timelineAsset)
            {
                foreach (var track in timelineAsset.GetOutputTracks())
                {
                    var trackBinding = director.GetGenericBinding(track) as Material;
                    if (trackBinding == material)
                    {
                        foreach (var clip in track.GetClips())
                        {
                            var asset = clip.asset as Vector4Clip;
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
