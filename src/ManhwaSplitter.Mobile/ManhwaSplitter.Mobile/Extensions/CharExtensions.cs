namespace ManhwaSplitter.Mobile.Extensions;

public static class CharExtensions
{
    public static bool IsEnglishLetter(this char c)
    {
        return c is >= 'a' and <= 'z'
            or >= 'A' and <= 'Z';
    }

    public static bool IsDigitOrEnglishLetter(this char c)
    {
        return char.IsDigit(c) || c.IsEnglishLetter();
    }
}