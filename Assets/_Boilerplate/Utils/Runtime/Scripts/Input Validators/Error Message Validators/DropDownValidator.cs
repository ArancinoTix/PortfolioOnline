using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

namespace U9.Utils
{
    [CreateAssetMenu(fileName = "DropDownValidator", menuName = "UNIT9/Input Validators/Drop Down Validator")]
    public class DropDownValidator : BaseInputErrorMessageValidator
    {
        [HeaderAttribute("Formatting")]
        [SerializeField] private bool _MustBeDifferentFrom;
        [SerializeField] private int _index;
        [SerializeField] private LocalizedString _specialErrorMessage;

        public override char Validate(ref string text, ref int pos, char ch)
        {
            throw new System.NotImplementedException();
        }

        public override bool ValidateForErrorMessages(int currentIndex, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (!_MustBeDifferentFrom)
                return true;
            else
            {
                if (currentIndex == _index)
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
