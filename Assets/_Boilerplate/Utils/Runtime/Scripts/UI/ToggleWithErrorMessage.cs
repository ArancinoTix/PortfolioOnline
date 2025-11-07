using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace U9.Utils
{
    public class ToggleWithErrorMessage : MonoBehaviour
    {
        [SerializeField] private Toggle _toggle;
        [SerializeField] private TextMeshProUGUI _errorLabel;
        [SerializeField] private BaseInputErrorMessageValidator _errorMessageValidator;
        [SerializeField] private float _noErrorUIHeight = 180;
        private Dictionary<string, string> _customErrors = new Dictionary<string, string>();
        /// <summary>
        /// Notifies any listeners when a new error is added.
        /// Can be used for example to focus the view on a specific field after submitting to the backend.
        /// </summary>
        
        //public Action<InputFieldWithErrorMessage> onErrorTriggered;
        //public Action<float> onComponentResized;
        private void Awake()
        {
            _toggle.isOn = false;
            ClearErrors();
            _toggle.onValueChanged.AddListener((bool a)=>IsToggleSelected());
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

        public void ResetToggle()
        {
            _toggle.isOn = false;
            ClearErrors();
        }

        private void SetUIHeight(float height)
        {
            RectTransform rt = (RectTransform)transform;

            var sDelta = rt.sizeDelta;
            sDelta.y = height;

            rt.sizeDelta = sDelta;

            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform.parent);
        }

        private void ShowError()
        {
             string fullErrorMessage = string.Empty;
            _errorMessageValidator.ValidateForErrorMessages(false,out fullErrorMessage);
            _errorLabel.gameObject.SetActive(true);
            _errorLabel.text =  fullErrorMessage;

            //Adjust the UI height to accomodate the error length. It's possible for us to get multiple errors.
            float h = _noErrorUIHeight;

            //Update the UI height if we have an error
            h += _errorLabel.preferredHeight;

            //Apply it
            SetUIHeight(h);
        }

        private string GetCustomErrorMessage()
        {
            string errorMessage = string.Empty;
            int errorCount = 0;

            if (_customErrors.Count > 0)
            {
                foreach (var customError in _customErrors)
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

        public bool IsToggleSelected()
        {
            if (_toggle.isOn)
            {
                ClearErrors();
                return true;
            }
            else
            {
                ShowError();
                return false;
            }
        }
    }
}
