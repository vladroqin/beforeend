using System.Text.RegularExpressions;

namespace Terminal;

public static class Html
{
  /// <summary>
  /// Для HTML
  /// </summary>
  /// <param name="s">Строка</param>
  /// <returns>результат</returns>
  public static string ForHtml(string s)
  {
    s = Regex.Replace(s, @"\s*<br>", "<br>", RegexOptions.IgnoreCase);
    s = Regex.Replace(s, @"<p>\s*", "<p>", RegexOptions.IgnoreCase);
    s = Regex.Replace(s, @"\s*</p>", "", RegexOptions.IgnoreCase);
    s = Regex.Replace(s, @"<div>\s*", "<div>", RegexOptions.IgnoreCase);
    s = Regex.Replace(s, @"\s*</div>", "</div>", RegexOptions.IgnoreCase);
    s = Regex.Replace(s, @"\s*</tr>", "", RegexOptions.IgnoreCase);
    s = Regex.Replace(s, @"\s*</td>", "", RegexOptions.IgnoreCase);
    s = Regex.Replace(s, @"\s*</th>", "", RegexOptions.IgnoreCase);
    s = Regex.Replace(s, @"\s*</li>", "", RegexOptions.IgnoreCase);
    s = Regex.Replace(s, @"\s*</dt>", "", RegexOptions.IgnoreCase);
    s = Regex.Replace(s, @"\s*</dd>", "", RegexOptions.IgnoreCase);
    s = Regex.Replace(s, @"[ \t]+", " ");
    s = Regex.Replace(s, @"\n ", "\n");
    s = Regex.Replace(s, @"\n+", "\n");
    s = Regex.Replace(s, @"^\n", "");
    return s;
  }
}