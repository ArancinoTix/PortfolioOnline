using UnityEngine;
using UnityEngine.Playables;

namespace U9.Motion.Timeline
{
    public class MaterialTextureControlMixer : PlayableBehaviour
    {
        private Texture defaultTexture;
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

                    defaultTexture = material.GetTexture(parameterID);
                    firstFrameHappened = true;
                }

                int inputCount = playable.GetInputCount();

                for (int I = 0; I < inputCount; I++)
                {
                    ScriptPlayable<SpriteControlBehaviour> inputPlayable = (ScriptPlayable<SpriteControlBehaviour>)playable.GetInput(I);

                    SpriteControlBehaviour behaviour = inputPlayable.GetBehaviour();

                    if (behaviour.sprites != null)
                    {
                        // Only one clip can play at a time on a single track
                        if (inputPlayable.GetPlayState() == PlayState.Playing)
                        {
                            double playTime = inputPlayable.GetTime();
                            double playDuration = inputPlayable.GetDuration();
                            float normalizedTime = Mathf.Clamp01((float)(playTime / playDuration));

                            // TODO: Currently adding 0.01 to normalized time so that final frame is shown (This won't always work, find better solution)
                            material.SetTexture(parameterID, behaviour.sprites[(int)Mathf.Lerp(0, behaviour.sprites.Length - 1, normalizedTime + 0.01f)].texture);
                        }
                    }
                }
            }
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            firstFrameHappened = false;

            if (material != null && material.HasProperty(parameterID))
            {
                material.SetTexture(parameterID, defaultTexture);
            }
        }
        public void SetParameterID(int parameterID)
        {
            this.parameterID = parameterID;
        }
    }
}
