using System.Globalization;

namespace Launcher.UI;

/// <summary>
///     Checks if two strings are equal in a natural way.
/// </summary>
public class NaturalStringComparer : IComparer<string>
{
    /// <inheritdoc />
    public int Compare(string? s1, string? s2)
    {
        if (s1 == s2)
            return 0;
        if (s1 == null)
            return -1;
        if (s2 == null)
            return 1;

        int i = 0, j = 0;
        int len1 = s1.Length, len2 = s2.Length;

        while (i < len1 && j < len2)
        {
            int result = CompareCharacters(s1, s2, ref i, ref j);
            if (result != 0)
                return result;
        }

        // If one string has remaining characters, it is greater
        return (len1 - i).CompareTo(len2 - j);
    }

    private static int CompareCharacters(string s1, string s2, ref int i, ref int j)
    {
        char c1 = s1[i];
        char c2 = s2[j];

        // If both characters are digits, compare as numbers
        if (char.IsDigit(c1) && char.IsDigit(c2))
        {
            int start1 = i;
            while (i < s1.Length && char.IsDigit(s1[i]))
                i++;

            int start2 = j;
            while (j < s2.Length && char.IsDigit(s2[j]))
                j++;

            string numStr1 = s1.Substring(start1, i - start1);
            string numStr2 = s2.Substring(start2, j - start2);

            // Parse numbers ignoring leading zeros
            int num1 = int.Parse(numStr1, CultureInfo.InvariantCulture);
            int num2 = int.Parse(numStr2, CultureInfo.InvariantCulture);

            if (num1 != num2)
                return num1.CompareTo(num2);

            return 0;
        }

        // Compare characters case-insensitively
        int result = char.ToUpperInvariant(c1).CompareTo(char.ToUpperInvariant(c2));
        if (result != 0)
            return result;

        i++;
        j++;

        return 0;
    }
}
