using System.Collections.Generic;
using System.IO;

namespace Terminal;

public static class Script
{
  static Dictionary<string, string> SuperScripts = new();

  static Dictionary<string, string> SubScripts = new();

  /// <summary>
  /// Загрузить словарь
  /// </summary>
  /// <param name="fileName">Имя файла</param>
  /// <param name="dictionary">Словарь</param>
  static void LoadDictionary(string fileName, Dictionary<string, string> dictionary)
  {
    var temp = File.ReadAllLines(fileName);
    foreach (var x in temp)
    {
      var cells = x.Split(new char[] { '\t' });
      dictionary.Add(cells[0], cells[1]);
    }
  }

  /// <summary>
  /// Проверить входят ли все символы
  /// </summary>
  /// <param name="s">Строка</param>
  /// <param name="dictionary">Словарь</param>
  /// <returns>Да/нет</returns>
  static bool CheckForScript(string s, Dictionary<string, string> dictionary)
  {
    int stringLength = s.Length;
    for (int i = 0; i < stringLength; i++)
    {
      if (Char.IsHighSurrogate(s[i]) && i + 1 < stringLength && Char.IsLowSurrogate(s[i + 1]))
      {
        if (!dictionary.Keys.Any(key => key.Contains($"{s[i]}{s[i + 1]}")))
          return false;
        i++;
        continue;
      }
      if (!dictionary.Keys.Any(key => key.Contains(s[i])))
        return false;
    }
    return true;
  }

  /// <summary>
  /// Конвертировать строку в скрипт
  /// </summary>
  /// <param name="s">Строка</param>
  /// <param name="dictionary">Словарь</param>
  /// <returns>Результат</returns>
  static string ConvertToScripts(string s, Dictionary<string, string> dictionary)
  {
    int stringLength = s.Length;
    string result = "";
    for (int i = 0; i < s.Length; i++)
    {
      if (Char.IsHighSurrogate(s[i]) && i + 1 < stringLength && Char.IsLowSurrogate(s[i + 1]))
      {
        result = $"{result}{dictionary[new string(new char[] { s[i], s[i + 1] })]}";
        i++;
        continue;
      }
      result = $"{result}{dictionary[new string(new char[] { s[i] })]}";
    }

    return result;
  }

  /// <summary>
  /// Найти один скрипт
  /// </summary>
  /// <param name="s"Строка</param>
  /// <param name="index">Начало поиска</param>
  /// <param name="scr1">Открывающий тег</param>
  /// <param name="scr2">Закрывающий тег</param>
  /// <returns>Да/нет</returns>
  static int[]? FindOneScript(string s, int index, string scr1, string scr2)
  {
    int firstLength = scr1.Length;
    int secondLength = scr2.Length;

    int one = s.IndexOf(scr1, index);
    if (one == -1)
      return null;

    int three = s.IndexOf(scr2, index);
    if (three == -1)
      return null;

    return new int[] { one, one + firstLength, three, three + secondLength };
  }

  /// <summary>
  /// Заменяем все указанные теги в цикле
  /// </summary>
  /// <param name="s">Строка</param>
  /// <param name="beginTag">Открывающий тег</param>
  /// <param name="endTag">Закрывающий тег</param>
  /// <param name="dic">Словарь</param>
  /// <returns>Результат</returns>
  static string TagCycle(string s, string beginTag, string endTag, Dictionary<string, string> dic)
  {
    int firstLength = beginTag.Length;
    int secondLength = endTag.Length;
    int begin = 0;
    int[] preresult = null;
    do
    {
      preresult = FindOneScript(s, begin, beginTag, endTag);
      if (preresult != null)
      {
        var fortest = s.Substring(
          preresult[1],
          preresult[2] - (preresult[0] + firstLength)
        );
        if (CheckForScript(fortest, dic))
        {
          string result = ConvertToScripts(fortest, dic);
          s = String.Format(
            "{0}{1}{2}",
            s.Substring(0, preresult[0]),
            result,
            s.Substring(preresult[3])
          );
        }
        begin = preresult[3];
      }
    } while (preresult != null);
    return s;
  }

  /// <summary>
  /// Проверка строки на скрипты
  /// </summary>
  /// <param name="s">Проверяемая строка</param>
  /// <returns>Результат</returns>
  public static string Checker(string s)
  {
    LoadDictionary(Path.Combine(BeforeAll.PATH_LOCALE_SHARE, "sub.tab"), SubScripts);
    LoadDictionary(Path.Combine(BeforeAll.PATH_LOCALE_SHARE, "super.tab"), SuperScripts);

    s = TagCycle(s, "<sup>", "</sup>", SuperScripts);
    s = TagCycle(s, "<sub>", "</sub>", SubScripts);

    return s;
  }
}