using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using VisualInspector;
using HeaderAttribute = VisualInspector.HeaderAttribute;


namespace U9.Utils
{
    [CreateAssetMenu(fileName = "ToggleValidator", menuName = "UNIT9/Input Validators/Toggle Input Validator")]
    public class TglTickBoxErrorMessageValidator : BaseInputErrorMessageValidator
    {
        [HeaderAttribute("Formatting")]
        [SerializeField] private bool _mustBeSelected;
        [SerializeField] private LocalizedString _specialErrorMessage;


        public override char Validate(ref string text, ref int pos, char ch)
        {
            throw new System.NotImplementedException();
        }

        public override bool ValidateForErrorMessages(bool selected, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (!_mustBeSelected)
                return true;
            else
            {
                if (!selected)
                {
                    errorMessage = _specialErrorMessage.GetLocalizedString();
                    return false;
                }
                else
                    return true;
            }
           
        }
    }
}
