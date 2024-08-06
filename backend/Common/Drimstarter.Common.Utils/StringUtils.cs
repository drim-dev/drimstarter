using System.Globalization;

namespace Drimstarter.Common.Utils;

// TODO: write unit tests
public static class StringUtils
{
    public static string CapitalizeWords(this string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return str;
        }

        var textInfo = CultureInfo.CurrentCulture.TextInfo;
        return textInfo.ToTitleCase(str.ToLower());
    }
}
