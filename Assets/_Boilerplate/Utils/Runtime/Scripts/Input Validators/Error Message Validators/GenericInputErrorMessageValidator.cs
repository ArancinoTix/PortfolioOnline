using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using VisualInspector;
using HeaderAttribute = VisualInspector.HeaderAttribute;

namespace U9.Utils
{
    [CreateAssetMenu(fileName = "GenericValidator", menuName = "UNIT9/Input Validators/Generic Input Validator")]

    public class GenericInputErrorMessageValidator : BaseInputErrorMessageValidator
    {
        [HeaderAttribute("Formatting")]
        [SerializeField] private bool _mustNotBeEmpty;
        [Separator][SerializeField] private LocalizedString _errorMessage;
        public override char Validate(ref string text, ref int pos, char ch)
        {
            throw new System.NotImplementedException();
        }
        public override bool ValidateForErrorMessages(string text, string textToMatch, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (_mustNotBeEmpty && text == "")
            {
                errorMessage = _errorMessage.GetLocalizedString();
                return false;
            }
            return true;
        }
    }
}
