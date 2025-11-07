using System;
using System.Collections;
using UnityEngine;
using U9.Utils;
using U9.Ease;
using VisualInspector;
using HeaderAttribute = VisualInspector.HeaderAttribute;
using RangeAttribute = UnityEngine.RangeAttribute;

namespace U9.ProgressTransition
{
    [ExecuteInEditMode]
    public abstract class BaseProgressTransition : MonoBehaviour
    {
#if UNITY_EDITOR
        [HeaderAttribute("Editor")]
        [SerializeField] [TextArea(3,3)] private string _description;
        public string Description { get => _description; set => _description = value; }
#endif

        [HeaderAttribute("Progress")]
        [Separator][SerializeField] [Range(0, 1)] private float _progress = 1;

        [HeaderAttribute("Easing Settings")]
        [SerializeField] private float _duration = 0.35f;
        [SerializeField] private EaseType _easeType = EaseType.Linear;
        [Separator][SerializeField] private bool _useTimeScale = false;


        private float _currentProgress = 0;
        private Coroutine _activeCoroutine = null;
        private IBaseEase _easeFunction = null;

        public float Duration {
            get => _duration; 
            protected set => _duration = value; 
        }

        private IBaseEase EaseFunction
        {
            get
            {
                if (_easeFunction == null || _easeFunction.EaseType() != _easeType)
                    _easeFunction = EaseFormula.GetEaseFunction(_easeType);
                return _easeFunction;
            }
        }

        public float Progress { get => _progress; }

        protected float GetEasedProgress(float x)
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
#endif
                return EaseFunction.Calculate(x);
#if UNITY_EDITOR
            else
                return EaseFormula.GetEasedValue(_easeType, x);
#endif
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (!Application.isPlaying)
            {
                if (_currentProgress != _progress)
                    PrepCalculationParams();
                UpdateProgress(false);
            }
        }
#endif

        public virtual void PrepCalculationParams()
        {
            //Override if there are values you need before the tween begins
        }


        [DisableInEditMode][Button]
        public void Play(bool incrementProgress, float delay, Action onStart, Action onComplete)
        {
            Stop();
            _activeCoroutine = CoroutineHandler.GetInstance(true).StartCoroutine(ProgressRoutine(incrementProgress, delay, onStart, onComplete));
        }


        [DisableInEditMode][Button]
        public void Stop()
        {
            if (_activeCoroutine != null)
                CoroutineHandler.GetInstance(false)?.StopCoroutine(_activeCoroutine);
            _activeCoroutine = null;
        }

        private IEnumerator ProgressRoutine(bool increment, float delay, Action onStart, Action onComplete)
        {
            PrepCalculationParams();

            if(delay >0)
                yield return new WaitForSeconds(delay);

            onStart?.Invoke();

            //Where are we coming from and going to?
            float origin = _currentProgress;
            float target = increment ? 1 : 0;

            //How much farther do we need to go?
            float remainingProgress = target - origin;
            if (remainingProgress < 0)
                remainingProgress = -remainingProgress;

            //If we have no progress to apply, skip
            if (remainingProgress <= 0)
            {
                yield return null;
            }
            //Otherwise we prep the loop
            else
            {
                //How much time we need have left based on the progress?
                float timeRemaining = _duration * remainingProgress;

                //Calculate our speed per frame
                float speed = 1f / timeRemaining;
                float lerp = 0;

                while (lerp < 1)
                {
                    //Apply the delta and lerp the progress to the target
                    float delta = speed * (_useTimeScale ? Time.deltaTime : Time.unscaledDeltaTime);
                    lerp += delta;
                    _progress = Mathf.Lerp(origin, target, lerp);

                    //Apply
                    UpdateProgress(false);
                    yield return null;
                }
            }

            //Inform of completion
            onComplete?.Invoke();
        }

        public void SetProgress(float progress, bool forceUpdate =false)
        {
            _progress = progress;
            UpdateProgress(forceUpdate);
        }

        private void UpdateProgress(bool forceUpdate)
        {
            if (_progress != _currentProgress || forceUpdate)
            {
                _currentProgress = _progress;
                ApplyProgress(GetEasedProgress(_currentProgress));
            }
        }

        protected abstract void ApplyProgress(float progress);


        [DisableInPlayMode][Button]
        private void SetFromValues()
        {
            if(!Application.isPlaying)
                SetFromValuesInternal();
        }

        [DisableInPlayMode][Button]
        private void SetToValues()
        {
            if (!Application.isPlaying)
                SetToValuesInternal();
        }

        protected abstract void SetFromValuesInternal();
        protected abstract void SetToValuesInternal();
        protected virtual void Reset()
        {
            SetFromValuesInternal();
            SetToValuesInternal();
        }
    }
}