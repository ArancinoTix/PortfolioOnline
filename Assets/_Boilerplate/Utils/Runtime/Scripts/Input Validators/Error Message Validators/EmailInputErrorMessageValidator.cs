using UnityEngine;
using UnityEngine.Localization;
using VisualInspector;
using HeaderAttribute = VisualInspector.HeaderAttribute;

namespace U9.Utils
{
    [CreateAssetMenu(fileName = "EmailValidator", menuName = "UNIT9/Input Validators/Email Input Validator")]

    public class EmailInputErrorMessageValidator : BaseInputErrorMessageValidator
    {
        //[HeaderAttribute("Formatting")]
        [SerializeField] private bool _mustContainSpecialDotCharacter;
        [Separator][SerializeField] private LocalizedString _missingDotMessage;

        [SerializeField] private bool _mustContainSpecialAtCharacter;
        [Separator][SerializeField] private LocalizedString _missingAtMessage;

        [SerializeField] private bool _mustBeNotEmpty;
        [SerializeField] private LocalizedString _emptyStringMessage;


        public override char Validate(ref string text, ref int pos, char ch)
        {
            //TODO
#if UNITY_EDITOR
            text = text.Insert(pos, ch.ToString());
#endif
            pos++;
            return ch;
        }

        public override bool ValidateForErrorMessages(string text, string textToMatch, out string errorMessage)
        {
            if(text == string.Empty && _mustBeNotEmpty)
            {
                errorMessage = _emptyStringMessage.GetLocalizedString();
                return false;
            }
            if (!text.Contains("@") && _mustContainSpecialAtCharacter )
            {
                errorMessage = _missingAtMessage.GetLocalizedString();
                return false;
            }
            if (!text.Contains(".") && _mustContainSpecialDotCharacter)
            {
                errorMessage = _missingDotMessage.GetLocalizedString();
                return false;
            }
            else
            {
                errorMessage = null;
                return true;
            }

        }


    }
}
