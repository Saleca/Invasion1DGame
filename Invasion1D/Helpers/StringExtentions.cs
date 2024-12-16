namespace Invasion1D.Helpers;

public static class StringExtentions
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
}