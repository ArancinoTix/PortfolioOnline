using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace U9.Motion.Timeline
{
    public class RotateControlMixer : PlayableBehaviour
    {
        private Vector3 defaultRotation;
        private Vector3 defaultLocalRotation;
        private Vector3 blendedRotation;
        private Transform transform;
        private bool firstFrameHappened;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            transform = playerData as Transform;

            if (transform != null)
            {
                if (!firstFrameHappened)
                {
                    defaultRotation = transform.rotation.eulerAngles;
                    defaultLocalRotation = transform.localRotation.eulerAngles;

                    // Caluclate all the clips on this tracks, Start and End times
                    RecalculateAllClipsStartAndEnd(playable);

                    firstFrameHappened = true;
                }

                blendedRotation = Vector3.zero;

                float blendedWeight = 0.0f;
                bool onATrack = false;
                bool useWorldSpace = true;

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
                                AssignValue(behaviour.useWorldSpace, GetValue(behaviour, 1.0f));
                            else if (behaviour.clipExtrapolation == ClipExtrapolation.Default)
                            {
                                transform.rotation = Quaternion.Euler(defaultRotation);
                                transform.localRotation = Quaternion.Euler(defaultLocalRotation);
                            }
                        }
                        // If we are before the first track then use default value
                        else if (I == 0)
                        {
                            transform.rotation = Quaternion.Euler(defaultRotation);
                            transform.localRotation = Quaternion.Euler(defaultLocalRotation);
                        }

                        // Go to next input clip
                        continue;
                    }
                    else
                    {
                        blendedRotation += GetValue(behaviour, (float)(inputPlayable.GetTime() / inputPlayable.GetDuration())) * inputWeight;

                        // We are on at least one clip, so we will use the blended value (in case there are multiple clips at this point on the track)
                        onATrack = true;

                        // The last blended clip will determine the space used
                        useWorldSpace = behaviour.useWorldSpace;
                    }
                }

                if (onATrack)
                    AssignValue(useWorldSpace, blendedRotation);
            }
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            firstFrameHappened = false;

            if (transform != null)
            {
                transform.rotation = Quaternion.Euler(defaultRotation);
                transform.localRotation = Quaternion.Euler(defaultLocalRotation);
            }
        }

        private Vector3 GetValue(Vector3ControlBehaviour behaviour, float normalizedTime)
        {
            if(behaviour.useRelative)
                return new Vector3(behaviour.xEnabled ? Mathf.Lerp(behaviour.startAt.x, behaviour.endAt.x, behaviour.curveX.Evaluate(normalizedTime)) + defaultRotation.x : defaultRotation.x,
                                   behaviour.yEnabled ? Mathf.Lerp(behaviour.startAt.y, behaviour.endAt.y, behaviour.curveY.Evaluate(normalizedTime)) + defaultRotation.y : defaultRotation.y,
                                   behaviour.zEnabled ? Mathf.Lerp(behaviour.startAt.z, behaviour.endAt.z, behaviour.curveZ.Evaluate(normalizedTime)) + defaultRotation.z : defaultRotation.z);

            return new Vector3(behaviour.xEnabled ? Mathf.Lerp(behaviour.startAt.x, behaviour.endAt.x, behaviour.curveX.Evaluate(normalizedTime)) : defaultRotation.x,
                               behaviour.yEnabled ? Mathf.Lerp(behaviour.startAt.y, behaviour.endAt.y, behaviour.curveY.Evaluate(normalizedTime)) : defaultRotation.y,
                               behaviour.zEnabled ? Mathf.Lerp(behaviour.startAt.z, behaviour.endAt.z, behaviour.curveZ.Evaluate(normalizedTime)) : defaultRotation.z);
        }

        private void AssignValue(bool useWorldSpace, Vector3 value)
        {
            if(useWorldSpace)
                transform.rotation = Quaternion.Euler(value);
            else
                transform.localRotation = Quaternion.Euler(value);
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