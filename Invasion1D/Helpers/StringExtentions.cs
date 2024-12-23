namespace Invasion1D.Helpers;

public static partial class StringExtentions
{
    public static string CustomToString(this TimeSpan timeSpan)
    {
        if (timeSpan.Hours > 0)
        {
            return timeSpan.ToString(@"hh\:mm\:ss\.f");
        }
        else if (timeSpan.Minutes > 0)
        {
            return timeSpan.ToString(@"mm\:ss\.f");
        }
        else
        {
            return timeSpan.ToString(@"ss\.f");
        }
    }

    public static string CleanElementTextContents(this string text)
    {
        text = text.Trim().Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ");
        return MultipleSpaces().Replace(text, " ");
    }

    [System.Text.RegularExpressions.GeneratedRegex(@"\s+")]
    private static partial System.Text.RegularExpressions.Regex MultipleSpaces();
}