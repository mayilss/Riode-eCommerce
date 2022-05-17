using System.Text.RegularExpressions;

namespace Riode.WebUI.AppCode.Extensions
{
    public static partial class Extension
    {
        static public string HtmlToPlain(this string html)
        {
            if (string.IsNullOrEmpty(html))
                return html;
            //return Regex.Replace(html, "<(.|\n)*?>", "");
            html = Regex.Replace(html, @"(<[^>]*>|\r\n|\r|\n)", "");
            html = Regex.Replace(html, @"\s+", " ");
            return html;
        }
        static public string ToEllipse(this string text, int length = 50)
        {
            if (string.IsNullOrEmpty(text) || length >= text.Length)
                return text;
            return $"{text.Substring(0,length)}...";
        }
    }
}
