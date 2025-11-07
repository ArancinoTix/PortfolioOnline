using UnityEngine;
using UnityEngine.Localization;
using VisualInspector;
using HeaderAttribute = VisualInspector.HeaderAttribute;

namespace U9.Utils
{ 
    [CreateAssetMenu(fileName = "PasswordValidator", menuName = "UNIT9/Input Validators/Password Input Validator")]
    public class PasswordInputErrorMessageValidator : BaseInputErrorMessageValidator
    {

        [HeaderAttribute("Formatting")]
        [SerializeField] private bool _mustContainUpperCaseCharacter;
        [Separator][SerializeField] private LocalizedString _uppercaseErrorMessage;
        
        [SerializeField] private bool _mustContainLowerCaseCharacter;
        [Separator][SerializeField] private LocalizedString _lowercaseErrorMessage;
      
        [SerializeField] private bool _mustContainSpecialCharacter;
        [SerializeField] private LocalizedString _specialErrorMessage;
        [Separator][SerializeField] private string _validSpecialCharacters;
       
        [SerializeField] private bool _mustContainNumberCharacter;
        [Separator][SerializeField] private LocalizedString _numberErrorMessage;
        [HeaderAttribute("Length")]
        
        [SerializeField] private int _minLength = 8;
        [SerializeField] private int _maxLength = 20;
        [Separator][SerializeField] private LocalizedString _lengthErrorMessage;
       
        [HeaderAttribute("Stength")]
        [SerializeField] private bool _mustNotBeWeak = true;
        [SerializeField] private LocalizedString _notStrongErrorMessage;
        [MessageBox("The text file should be a new line seperated file of invalid passwords.")]
        [Separator][SerializeField] private TextAsset _weakPasswordFile;

        

        /// <summary>
        /// Check if the password is valid.
        /// </summary>
        /// <param name="text">The text to validate</param>
        /// <param name="errorMessage">Error messages</param>
        /// <returns>is the text valid and acceptable</returns>
        public override bool ValidateForErrorMessages(string text, string textToMatch, out string errorMessage)
        {
            bool isValid = base.ValidateForErrorMessages(text, textToMatch, out errorMessage);
            int errorCount = 0;

            if(!isValid)
            {
                if (ReturnAllErrors)
                    return false;
                else
                    errorCount = 1;
            }

            //Do we need to have UPPER, lower and or special characters
            if (_mustContainUpperCaseCharacter && !ContainsUpperCase(text))
            {
                AppendNewLines(errorCount, ref errorMessage);
                errorMessage += _uppercaseErrorMessage.GetLocalizedString();
                errorCount++;

                if (!ReturnAllErrors)
                    return false;
            }

            if(_mustContainLowerCaseCharacter && !ContainsLowerCase(text))
            {
                AppendNewLines(errorCount, ref errorMessage);
                errorMessage += _lowercaseErrorMessage.GetLocalizedString();
                errorCount++;

                if (!ReturnAllErrors)
                    return false;
            }
              
            if(_mustContainSpecialCharacter && !ContainsSpecial(text))
            {
                AppendNewLines(errorCount, ref errorMessage);
                errorMessage += _specialErrorMessage.GetLocalizedString();
                errorCount++;

                if (!ReturnAllErrors)
                    return false;
            }

            if (_mustContainNumberCharacter && !ContainsNumber(text))
            {
                AppendNewLines(errorCount, ref errorMessage);
                errorMessage += _numberErrorMessage.GetLocalizedString();
                errorCount++;

                if (!ReturnAllErrors)
                    return false;
            }

            //Do you need a specific length
            int tLength = text.Length;
            if((_minLength >=0 && tLength < _minLength)
                ||
                (_maxLength >=0 && tLength >_maxLength))
            {
                AppendNewLines(errorCount, ref errorMessage);
                errorMessage += _lengthErrorMessage.GetLocalizedString(_minLength,_maxLength);
                errorCount++;

                if (!ReturnAllErrors)
                    return false;
            }

            //Is the password weak, strong etc
            if (_mustNotBeWeak && IsWeak(text))
            {
                AppendNewLines(errorCount, ref errorMessage);
                errorMessage += _notStrongErrorMessage.GetLocalizedString();
                errorCount++;

                if (!ReturnAllErrors)
                    return false;
            }

            return errorCount == 0;
        }

        private bool IsWeak(string text)
        {
            if (_weakPasswordFile == null)
                return false;
            else
            {
                string[] weakPasswords = _weakPasswordFile.text.Split(System.Environment.NewLine);
                text = text.ToLower();

                foreach (var weakPassword in weakPasswords)
                {
                    if (weakPassword.ToLower().Equals(text))
                        return true;
                }

                return false;
            }
        }

        private bool ContainsUpperCase(string text)
        {
            foreach(char c in text)
            {
                if (char.IsUpper(c))
                    return true; 
            }
            return false;
        }

        private bool ContainsLowerCase(string text)
        {
            foreach (char c in text)
            {
                if (!char.IsUpper(c))
                    return true;
            }
            return false;
        }

        private bool ContainsSpecial(string text)
        {
            foreach (char c in text)
            {
                if (_validSpecialCharacters.Contains(c))
                    return true;
            }
            return false;
        }

        private bool ContainsNumber(string text)
        {
            foreach (char c in text)
            {
                if (char.IsNumber(c))
                    return true;
            }
            return false;
        }

        public override char Validate(ref string text, ref int pos, char ch)
        {
            //TODO
#if UNITY_EDITOR
            text = text.Insert(pos, ch.ToString());
#endif
            pos++;
            return ch;
        }
    }
}