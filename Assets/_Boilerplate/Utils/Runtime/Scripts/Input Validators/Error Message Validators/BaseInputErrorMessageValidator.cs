using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using VisualInspector;
using HeaderAttribute = VisualInspector.HeaderAttribute;

namespace U9.Utils
{
    //[CreateAssetMenu(fileName = "PhoneNumberInputValidator", menuName = "UNIT9/Input Validators With ErrorMessage/Phone Number Input Validator")]
    public abstract class BaseInputErrorMessageValidator : TMP_InputValidator
    {
        [HeaderAttribute("Formatting")]
        [SerializeField] private bool _mustMatch;
        [Separator][SerializeField] private LocalizedString _mismatchErrorMessage;

        [HeaderAttribute("General")]
        [SerializeField] private bool returnAllErrors = false;

        public bool ReturnAllErrors { get => returnAllErrors; }

        public virtual bool ValidateForErrorMessages(string text, string textToMatch, out string errorMessage)
        {
            errorMessage = string.Empty;

            if(_mustMatch && !text.Equals(textToMatch))
            {
                errorMessage = _mismatchErrorMessage.GetLocalizedString();
                return false;
            }
            return true;
        }

        public virtual bool ValidateForErrorMessages(bool selected, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (!selected)
            {
                errorMessage = _mismatchErrorMessage.GetLocalizedString();
                return false;
            }
            return true; 
        }

        public virtual bool ValidateForErrorMessages(int index, out string errorMessage)
        {
            errorMessage = string.Empty;
            return false;
        }

        public string GetErroMessage()
        {
            return _mismatchErrorMessage.GetLocalizedString();
        }

        protected void AppendNewLines(int errorCount, ref string errorMessage)
        {
            if (errorCount > 0)
                errorMessage += System.Environment.NewLine;
        }

        
    }
}