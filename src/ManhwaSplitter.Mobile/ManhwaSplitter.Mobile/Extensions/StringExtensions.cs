namespace ManhwaSplitter.Mobile.Extensions;

public static class StringExtensions
{
    public static bool CharsAreDigitsOrEnglishLetters(this string str, int start, int end)
    {
        for (int i = start; i <= end; i++)
        {
            if (!str[i].IsDigitOrEnglishLetter())
                return false;
        }

        return true;
    }
}