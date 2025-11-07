using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace U9.Utils
{
    public class DropDownWithErrorMessage : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown _dropDown;
        [SerializeField] private TextMeshProUGUI _errorLabel;
        [SerializeField] private BaseInputErrorMessageValidator _errorMessageValidator;
        [SerializeField] private float _noErrorUIHeight = 180;
        private Dictionary<string, string> _customErrors = new Dictionary<string, string>();

        public TMP_Dropdown DropDown { get => _dropDown; }

        private void Awake()
        {
            _dropDown.onValueChanged.AddListener(CheckSelection);
        }

        public void ClearErrors()
        {
            if (_errorLabel != null)
                _errorLabel.gameObject.SetActive(false);

            _customErrors.Clear();
            SetUIHeight(_noErrorUIHeight);
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
            _errorMessageValidator.ValidateForErrorMessages(false, out fullErrorMessage);
            _errorLabel.gameObject.SetActive(true);
            _errorLabel.text = fullErrorMessage;

            //Adjust the UI height to accomodate the error length. It's possible for us to get multiple errors.
            float h = _noErrorUIHeight;

            //Update the UI height if we have an error
            h += _errorLabel.preferredHeight;

            //Apply it
            SetUIHeight(h);
        }

        private void CheckSelection(int index)
        {
            if(_errorMessageValidator != null)
            {
                string fullErrorMessage = string.Empty;
                if (!_errorMessageValidator.ValidateForErrorMessages(index, out fullErrorMessage))
                {
                    if (_errorLabel != null)
                    {
                        _errorLabel.gameObject.SetActive(true);
                        _errorLabel.text = fullErrorMessage;

                        //Adjust the UI height to accomodate the error length. It's possible for us to get multiple errors.
                        float h = _noErrorUIHeight;

                        //Update the UI height if we have an error
                        h += _errorLabel.preferredHeight;

                        //Apply it
                        SetUIHeight(h);
                    }
                    
                }
                else
                {
                    if (_errorLabel != null && _errorLabel.gameObject.activeInHierarchy)
                        ClearErrors();
                }
            }
        }

        public bool IsValid()
        {
            CheckSelection(_dropDown.value);
            if (_errorLabel.gameObject.activeInHierarchy)
                return false;
            else
                return true;
        }
    }
}
