using System.Collections.Generic;
using UnityEngine;
using VisualInspector;
using HeaderAttribute = VisualInspector.HeaderAttribute;

namespace U9.ProgressTransition
{      
    public class SequenceProgressTransition : BaseProgressTransition
    {
        protected enum SequenceType
        {
            Parallel,
            Staggered,
            Sequential
        }

        [MessageBox("This transition combines multiple transitions into one." +
            "\nDuration will be overriden by the sum of the sequences transitions." +
            "\n\n- [Parallel] will cause the transitions to play side by side." +
            "\n- [Staggered] will cause the transitions to play at a defined stagger offset from one another." +
            "\n- [Sequential] will have the transitions play one after the other.")]
        [HeaderAttribute("State Settings")] 
        [SerializeField] private SequenceType _sequenceType = SequenceType.Parallel;
        [SerializeField] private float _staggerAmount = 0.1f;
        [Separator] [SerializeField] private BaseProgressTransition[] _transitionsToSequence;

        public BaseProgressTransition[] TransitionsToSequence { get => _transitionsToSequence; }

        public override void PrepCalculationParams()
        {
            //Duration is calculated sum of components
            Duration = 0;

            if(_sequenceType == SequenceType.Parallel)
            {
                //The duration will be the duration of the longest transition in the sequence
                foreach (var f in _transitionsToSequence)
                {
                    if (f != null)
                    {
                        f.PrepCalculationParams();

                        if(f.Duration > Duration)
                            Duration = f.Duration;
                    }
                }
            }
            else if(_sequenceType == SequenceType.Staggered)
            {
                //The duration will be the sum of all transitions in the sequence,
                //with the total stagger subtracted

                float totalStagger = _staggerAmount * (_transitionsToSequence.Length - 1);

                foreach (var f in _transitionsToSequence)
                {
                    if (f != null)
                    {
                        f.PrepCalculationParams();

                        if (f.Duration > Duration)
                            Duration = f.Duration;
                    }
                }

                Duration += totalStagger;
            }
            else
            {
                //The duration will be the sum of all transitions in the sequence
                foreach (var f in _transitionsToSequence)
                {
                    if (f != null)
                    {
                        f.PrepCalculationParams();

                        Duration += f.Duration;
                    }
                }
            }
        }

        protected override void ApplyProgress(float progress)
        {
            if(_sequenceType == SequenceType.Parallel)
            {
                foreach (var f in _transitionsToSequence)
                {
                    var share = Mathf.Clamp01(progress / f.Duration* Duration);
                    if (f != null)
                        f.SetProgress(share, true);
                }
            }
            else if (_sequenceType == SequenceType.Staggered)
            {
                float staggerDelta = _staggerAmount/Duration;

                float staggerOffset = 0;
                float totalStagger = staggerDelta * (_transitionsToSequence.Length - 1);

                foreach (var f in _transitionsToSequence)
                {
                    if (f != null)
                    {
                        float localProgress = (progress - staggerOffset) / (1 - totalStagger);
                        localProgress = Mathf.Clamp01(localProgress);
                        f.SetProgress(localProgress, true);

                        staggerOffset += staggerDelta;
                    }
                }
            }
            else
            {
                float staggerOffset = 0;

                foreach (var f in _transitionsToSequence)
                {
                    if (f != null)
                    {
                        float progressShare = f.Duration / Duration;

                        float localProgress = (progress - staggerOffset) / progressShare;
                        localProgress = Mathf.Clamp01(localProgress);
                        f.SetProgress(localProgress, true);

                        staggerOffset += progressShare;
                    }
                }
            }
        }

        protected override void SetFromValuesInternal()
        {
        }

        protected override void SetToValuesInternal()
        {
        }

        protected override void Reset()
        {
            List<BaseProgressTransition> potentialTransitions = new List<BaseProgressTransition>(GetComponents<BaseProgressTransition>());
            for (int i = 0, ni = potentialTransitions.Count; i < ni; i++)
            {
                if (potentialTransitions[i] == this)
                {
                    potentialTransitions.RemoveAt(i);
                    i--;
                    ni--;
                }
            }
                
            _transitionsToSequence = potentialTransitions.ToArray();
        }
    }
}
