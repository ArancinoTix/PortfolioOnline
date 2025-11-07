using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "AutoFormatIPValidator", menuName = "UNIT9/Input Validators/Auto Format IP")]
public class TMP_IPInputValidator : TMP_InputValidator
{
    public override char Validate(ref string text, ref int pos, char ch)
    {
        // Only allow digits and dots
        if (!char.IsDigit(ch) && ch != '.')
            return '\0';

        // Count how many dots exist
        int dotCount = CountDots(text);

        // Handle dot
        if (ch == '.')
        {
            if (dotCount >= 3)
                return '\0';

            // If cursor is before a .0 (user pressed dot twice), move cursor and continue
            if (text.Length > pos && text[pos-1] == '.' && text[pos] == '0')
            {
                pos += 1; // Skip past ".0"

                // Recount dots after skipping
                dotCount = CountDots(text);

                if (dotCount >= 3)
                    return '\0'; // Double check dot count after moving
            }

            // After skip (or not), insert ".0"
            text = text.Insert(pos, ".0");
            pos += 1; // Cursor between . and 0
            return '\0';
        }

        // Handle digit
        if (char.IsDigit(ch))
        {
            // Replace auto-inserted 0 after .
            if (pos > 0 && text.Length > pos && text[pos] == '0' && text[pos - 1] == '.')
            {
                text = text.Remove(pos, 1);
                text = text.Insert(pos, ch.ToString());
                pos++;
                return '\0';
            }

            // Count digits in current octet
            int digitsInOctet = 0;
            for (int i = pos - 1; i >= 0 && text[i] != '.'; i--)
            {
                if (char.IsDigit(text[i])) digitsInOctet++;
                else break;
            }

            text = text.Insert(pos, ch.ToString());
            pos++;

            // After inserting this digit, re-check count
            digitsInOctet++;
            if (digitsInOctet == 3 && dotCount < 3)
            {
                text = text.Insert(pos, ".0");
                pos++; // Jump after the .
            }

            return '\0'; // All inserted manually
        }

        return '\0';
    }

    private int CountDots(string str)
    {
        int count = 0;
        foreach (char c in str)
            if (c == '.') count++;
        return count;
    }
}