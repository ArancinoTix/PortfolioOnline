using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "CaptialNameValidator", menuName = "UNIT9/Input Validators/Capital Name Input Validator")]
public class TMP_CapitalNameInputValidator : TMP_InputValidator
{
    public override char Validate(ref string text, ref int pos, char ch)
    {
        if(text.Length > 12)
            return '\0';

        // Allow spaces
        if (ch == ' ')
        {
            text = text.Insert(pos, " ");
            pos++;
            return ' ';
        }

        // Allow letters and convert to uppercase
        if (char.IsLetter(ch))
        {
            char upperChar = char.ToUpper(ch);
            text = text.Insert(pos, upperChar.ToString());
            pos++;
            return upperChar;
        }

        // Reject all other characters
        return '\0';
    }
}