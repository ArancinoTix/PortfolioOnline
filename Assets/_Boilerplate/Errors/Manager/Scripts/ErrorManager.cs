using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using U9.Errors.Codes;
using U9.Utils;
using System;
using U9.View;
using VisualInspector;

namespace U9.Errors
{
    public class ErrorManager : MonoSingleton<ErrorManager>
    {
        [SerializeField] private ErrorView _errorView;
        [SerializeField] private View.View _backgroundView;
        [SerializeField] private ResponseCodeOption _genericDetails;

        private ErrorSettings _currentErrorSettings;

        // Start is called before the first frame update
        void Awake()
        {
            Instance = this;
        }

        [DisableInEditMode][Button]
        private void TestError()
        {
            ShowError(-1,
                () => { Debug.Log("Default Choice Selected"); },
                () => { Debug.Log("Error Overridden"); });
        }

        /// <summary>
        /// Displays the given details
        /// </summary>
        /// <param name="details">Information about the error</param>
        /// <param name="onDefaultChoiceDismissed">If the default "close" button was selected</param>
        /// <param name="onOverridden">Triggered if another error forced this error to dismiss with no action</param>
        /// <param name="onCustomChoiceDismissed">Triggered if the view was dismissed with a custom choice</param>
        public void ShowError(ResponseCodeOption details, Action onDefaultChoiceDismissed, Action onOverridden, Action<int> onCustomChoiceDismissed = null)
        {
            ShowError(details.Code, details, onDefaultChoiceDismissed, onOverridden, onCustomChoiceDismissed);
        }

        /// <summary>
        /// Displays a generic error for the given code
        /// </summary>
        /// <param name="errorCode">Code for the generic error</param>
        /// <param name="onDefaultChoiceDismissed">If the default "close" button was selected</param>
        /// <param name="onOverridden">Triggered if another error forced this error to dismiss with no action</param>
        public void ShowError(int errorCode, Action onDefaultChoiceDismissed, Action onOverridden)
        {
            ShowError(errorCode, _genericDetails, onDefaultChoiceDismissed, onOverridden, null);
        }

        private void ShowError(int errorCode, ResponseCodeOption details, Action onDefaultChoiceDismissed, Action onOverridden, Action<int> onCustomChoiceDismissed)
        {
            //If we have an existing error, override it
            if (_currentErrorSettings != null)
                _currentErrorSettings.Overridden();

            _currentErrorSettings = new ErrorSettings(details, onDefaultChoiceDismissed, onOverridden, onCustomChoiceDismissed);

            _errorView.Hide(0, 0, true); //Hide immediately incase it was mid display for a previous error
            _errorView.Configure(errorCode, details);
            _errorView.Display();

            //If background is not shown, show
            if (!_backgroundView.IsDisplaying)
                _backgroundView.Display();

            _errorView.onChoiceSelected = ChoiceMade;
        }

        private void ChoiceMade(int choiceIndex)
        {
            _errorView.onChoiceSelected = null;

            //Hide the views
            _backgroundView.Hide();
            _errorView.Hide();

            //We move it to a temp incase another error is triggered immediately via the events. Failure to do so could cause a false onOverriden callback
            var tmpSetting = _currentErrorSettings;
            _currentErrorSettings = null;

            //Trigger the events.
            if (tmpSetting != null)
                tmpSetting.ChoiceMade(choiceIndex);
        }

        private class ErrorSettings
        {
            private ResponseCodeOption _details;
            private Action _onDefaultChoiceDismissed;
            private Action _onOverridden;
            private Action<int> _onCustomChoiceDismissed;

            public ErrorSettings(ResponseCodeOption details, Action onDefaultChoiceDismissed, Action onOverridden, Action<int> onCustomChoiceDismissed)
            {
                _details = details;
                _onDefaultChoiceDismissed = onDefaultChoiceDismissed;
                _onOverridden = onOverridden;
                _onCustomChoiceDismissed = onCustomChoiceDismissed;
            }

            public ResponseCodeOption Details { get => _details; }

            public void ChoiceMade(int index)
            {
                if (index == -1)
                    _onDefaultChoiceDismissed?.Invoke();
                else
                    _onCustomChoiceDismissed?.Invoke(index);
            }

            public void Overridden()
            {
                _onOverridden?.Invoke();
            }
        }
    }
}
