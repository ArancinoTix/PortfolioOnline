using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Localization;

namespace U9.Utils
{
    public class InputFieldWithErrorMessage : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private TMP_InputField _inputFieldToMatch;
        [SerializeField] private TextMeshProUGUI _errorLabel;
        [SerializeField] private BaseInputErrorMessageValidator _errorMessageValidator;
        [SerializeField] private float _noErrorUIHeight = 180;
        protected float _previousHeight = 0;
        private Dictionary<string, string> _customErrors = new Dictionary<string, string>();

        /// <summary>
        /// Notifies any listeners when a new error is added.
        /// Can be used for example to focus the view on a specific field after submitting to the backend.
        /// </summary>
        public Action<InputFieldWithErrorMessage> onErrorTriggered;
        public Action<float> onComponentResized;
        public Action onValueChanged;

        private void Awake()
        {
            _inputField.onEndEdit.AddListener(ValueChanged);
            ClearErrors();
        }

        /// <summary>
        /// Reset the error messages
        /// </summary>
        public void ClearErrors()
        {
            if (_errorLabel)
                _errorLabel.gameObject.SetActive(false);

            _customErrors.Clear();
            SetUIHeight(_noErrorUIHeight);
        }

        /// <summary>
        /// Sets the input field base value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="notifyEvents"></param>
        public void SetValue(string value, bool notifyEvents)
        {
            if (!notifyEvents)
                _inputField.SetTextWithoutNotify(value);
            else
            {
                _inputField.text = value;
                ValueChanged(value);
            }
        }

        /// <summary>
        /// Gets the current inputted value
        /// </summary>
        /// <returns></returns>
        public string GetValue()
        {
            return _inputField.text;
        }

        public BaseInputErrorMessageValidator GetErrorValidator()
        {
            return _errorMessageValidator;
        }

        /// <summary>
        /// Returns the main input field in case you need to assign listeners directly.
        /// </summary>
        /// <returns></returns>
        public TMP_InputField GetInputField()
        {
            return _inputField;
        }

        /// <summary>
        /// Is the inputted text valid for the assigned validator?
        /// </summary>
        /// <returns>Is valid</returns>
        public bool IsValid()
        {
            if (_errorMessageValidator != null)
                return _errorMessageValidator.ValidateForErrorMessages(
                    _inputField.text,
                    GetStringToMatch(),
                    out string errorMessage);
            else
                return true;
        }


        /// <summary>
        /// Refreshes the error message
        /// </summary>
        private void RefreshErrorMessage(bool notifyErrorEvent)
        {
            ProcessString(_inputField.text, notifyErrorEvent);
        }

        /// <summary>
        /// Called when we refresh or finish editing
        /// </summary>
        /// <param name="value"></param>
        private void ValueChanged(string value)
        {
            ProcessString(value, true);
            onValueChanged?.Invoke();
        }

        private void ProcessString(string value, bool notifyErrorEvent)
        {
            //Get he custom error summary
            string fullErrorMessage = GetCustomErrorMessage();

            //If the error message is empty, we are valid
            bool isValid = string.IsNullOrEmpty(fullErrorMessage);

            //Do we need to check further?
            if (_errorMessageValidator != null
                && 
                (isValid || _errorMessageValidator.ReturnAllErrors)) //Check only if we need to
            {
                //Get the status of the text from the assigned validator
                bool isValidForValidator = _errorMessageValidator.ValidateForErrorMessages(
                    value,
                    GetStringToMatch(),
                    out string errorMessage);

                //If we had custom errors and assigned ones, combine them. 
                if (!isValidForValidator && !isValid)
                    fullErrorMessage += System.Environment.NewLine + errorMessage;
               
                //Otherwise replace it
                else 
                    fullErrorMessage = errorMessage;

                //Only override the value if we didn't have custom errors
                if (isValid)
                    isValid = isValidForValidator;       
            }

            //Enable/disable the error if needed
            if(_errorLabel != null)
            {
                _errorLabel.gameObject.SetActive(!isValid);
                _errorLabel.text = fullErrorMessage;
            }

            //Adjust the UI height to accomodate the error length. It's possible for us to get multiple errors.
            float h = _noErrorUIHeight;

            //Update the UI height if we have an error
            if (!isValid)
                h += _errorLabel.preferredHeight;

            //Apply it
            SetUIHeight(h);

            if (!isValid)
                onErrorTriggered?.Invoke(this);
        }

        /// <summary>
        /// Add a custom error with a unique id. For example the backend rejected it
        /// </summary>
        /// <param name="id">Id of the custom message, for example "usernameUnavailable"</param>
        /// <param name="message">Message to show the user, for example "Username is taken".</param>
        public void AddError(string id, string message, bool notifyErrorEvent)
        {
            if (!_customErrors.ContainsKey(id))
            {
                _customErrors.Add(id, message);
                RefreshErrorMessage(notifyErrorEvent);
            }
        }

        /// <summary>
        /// Removes a custom error
        /// </summary>
        /// <param name="id">Id of the custom message, for example "usernameUnavailable"</param>
        public void RemoveError(string id)
        {
            if (_customErrors.ContainsKey(id))
            {
                _customErrors.Remove(id);
                RefreshErrorMessage(false);
            }
        }

        public string GetError()
        {
            return _errorMessageValidator.GetErroMessage();
        }

        /// <summary>
        /// Gets the custom error message
        /// </summary>
        /// <returns></returns>
        private string GetCustomErrorMessage()
        {
            string errorMessage = string.Empty;
            int errorCount = 0;

            if(_customErrors.Count >0)
            {
                foreach(var customError in _customErrors)
                {
                    //If we have more than one, add a new line
                    if (errorCount > 0)
                        errorMessage += System.Environment.NewLine;

                    //Add the error
                    errorMessage += customError.Value;
                    errorCount++;

                    //If we only display one message, stop
                    if (_errorMessageValidator != null && !_errorMessageValidator.ReturnAllErrors)
                        break;
                }
            }

            return errorMessage;
        }

        public void ClearInputField()
        {
            _inputField.text = string.Empty;
        }

        /// <summary>
        /// Applies the given height to the UI
        /// </summary>
        /// <param name="height"></param>
        protected void SetUIHeight(float height)
        {
            RectTransform rt = (RectTransform)transform;

            var sDelta = rt.sizeDelta;
            sDelta.y = height;

            rt.sizeDelta = sDelta;

            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform.parent);

            onComponentResized?.Invoke(_previousHeight - height);
            _previousHeight = height;
        }

        /// <summary>
        /// Returns the text of another input field that we need to match against if it exists, for example the first password field.
        /// </summary>
        /// <returns></returns>
        private string GetStringToMatch()
        {

            if (_inputFieldToMatch != null)
                return _inputFieldToMatch.text;
            else
                return _inputField.text;
        }
    }
}