namespace Terminal;

public static class MultyGraph
{
  /// <summary>
  /// Меняем мультиграфы
  /// </summary>
  /// <param name="s">Строка</param>
  /// <returns>Результат</returns>
  public static string FormMultygraphs(string s) =>
    s.Replace("&lt;->", "↔")
      .Replace("<-&gt;", "↔")
      .Replace("&lt;=>", "⇔")
      .Replace("<=&gt;", "⇔")
      .Replace("<-", "←")
      /*.Replace("->", "→")*/
      .Replace("<->", "↔")
      .Replace("<=", "⇐")
      .Replace("=>", "⇒")
      .Replace("<=>", "⇔")
      .Replace("&lt;-", "←")
      .Replace("-&gt;", "→")
      .Replace("&lt;-&gt;", "↔")
      .Replace("&lt;=", "⇐")
      .Replace("=&gt;", "⇒")
      .Replace("&lt;=&gt;", "⇔");
}