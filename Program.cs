using System.Configuration;
using System.Text;
using System.Text.RegularExpressions;

namespace Terminal;

public class BeforeAll
{
  private const string SPACES = " \t\u00a0\u1680\u2000\u2001\u2002" +
    "\u2003\u2004\u2005\u2006\u2007\u2008\u2009\u200a" +
    "\u202f\u205f\u3000\u180e\u200b\u200c\u200d\u2060" +
    "\ufeff\u00ad";  //Последние символы, особенно последний, возможно зря
  #region Help
  private const string HELP = "Использование: BeforeAll [ПАРАМЕТРЫ] [ФАЙЛ]\n" +
  "Убирает мелкий мусор из текстовых файлов и делает небольшую обработку.\n" +
  " -h\tОбработка HTML\n" +
  " -m\tОбработка популярных мультиграфов\n" +
  " -s\tОбработка верхних и нижних индексов в HTML\n\n" +
  " -w\tТекст в кодировке cp1251\n" +
  " -W\tТекст в кодировке cp1252\n" +
  " -d\tТекст в кодировке cp866\n" +
  " -D\tТекст в кодировке cp437\n" +
  " -k\tТекст в кодировке КОИ-8\n\n" +
  "(Необходимо задать параметр PATH_LOCALE_SHARE директории файлов *.tab!)";
  #endregion
  private readonly static Encoding _utf8woBom = new UTF8Encoding(false);
  private static bool _isHtml = default;
  private static bool _isSuperSubScript = default;
  private static bool _isMultyGraphs = default;
  public static string PATH_LOCALE_SHARE = "";

  private static void Main(string[] args)
  {
    if (args.Length == 0 || String.IsNullOrWhiteSpace(args[0]) ||
      (args.Length == 2 && args[0][0] != '-'))
    {
      Console.WriteLine(HELP);
      return;
    }
    if (args.Length == 1 && !File.Exists(args[0]))
    {
      Console.WriteLine($"Файл \"{args[0]}\" не существует!");
      return;
    }
    if (args.Length != 1 && !File.Exists(args[1]))
    {
      Console.WriteLine($"Файл \"{args[1]}\" не существует!");
      return;
    }

    try
    {
      PATH_LOCALE_SHARE = ConfigurationManager.AppSettings["PATH_LOCALE_SHARE"];
    }
    catch (Exception)
    {
      return;
    }

    string file;
    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    var enc = Encoding.UTF8;

    if (args.Length == 2)
    {
      file = args[1];
      if (args[0].Contains('h'))
        _isHtml = true;
      if (args[0].Contains('m'))
        _isMultyGraphs = true;
      if (args[0].Contains('s'))
        _isSuperSubScript = true;

      if (args[0].Contains('W'))
        enc = Encoding.GetEncoding(1252);
      if (args[0].Contains('w'))
        enc = Encoding.GetEncoding(1251);
      if (args[0].Contains('D'))
        enc = Encoding.GetEncoding(437);
      if (args[0].Contains('d'))
        enc = Encoding.GetEncoding(866);
      if (args[0].Contains('k'))
        enc = Encoding.GetEncoding(20866);
    }
    else
      file = args[0];

    var filename = Path.GetFileNameWithoutExtension(file);
    var ext = Path.GetExtension(file);
    var newfile = $"{filename}-NEW{ext}";
    var warn = new Warnings(file);
    var text = File.ReadAllText(file, enc);

    if (Regex.IsMatch(text, "<pre", RegexOptions.IgnoreCase))
    {
      _isHtml = false;
      _isMultyGraphs = false;
      _isSuperSubScript = false;
      Console.WriteLine("Здесь есть <pre>!");
    }

    text = Work(text, warn);

    File.WriteAllText(newfile, text, _utf8woBom);
    File.SetLastWriteTime(newfile, File.GetLastWriteTime(file));
    warn.Result();
  }

  /// <summary>
  /// Основная обработка текста
  /// </summary>
  /// <param name="text">Текст</param>
  /// <param name="warn">Предупреждения</param>
  /// <returns>Результат обработки</returns>
  private static string Work(string text, Warnings warn)
  {
    text = BeforeNormalization(text);
    text = text.Normalize(NormalizationForm.FormC);
    text = LineEndsNormailization(text);
    text = LineEndsSpaces(text);
    text = AfterNormalization(text);

    if (_isHtml)
      text = Html.ForHtml(text);

    if (_isMultyGraphs)
      text = MultyGraph.FormMultygraphs(text);

    if (_isSuperSubScript)
      text = Script.Checker(text);

    warn.Inspection(text);
    text = LineEndsSpaces(text);
    return EndLines(text);
  }

  /// <summary>
  /// Убрать пробелы в конце строки
  /// </summary>
  /// <param name="s">Строка</param>
  /// <returns>Результат</returns>
  private static string LineEndsSpaces(string s) =>
    Regex.Replace(Regex.Replace(s, $"[{SPACES}]*\\n", "\n"), @"\s+\Z", "");

  /// <summary>
  /// Сделать правильные концы строки
  /// </summary>
  /// <param name="s">Сама строка</param>
  /// <returns>Результат</returns>
  private static string LineEndsNormailization(string s)
  {
    return s.Replace("\r\n", "\n")  // ДОС
      .Replace("\r\n", "\n")        // для перестраховки, один раз встрачалось
      .Replace("\r", "\n")          // кл. Мак
      .Replace("\v", "\n")
      .Replace("\f", "\n")
      .Replace("\x85", "\n")        // на всякий случай
      .Replace("\u2028", "\n")
      .Replace("\u2029", "\n");
  }

  /// <summary>
  /// Действия перед нормализацией
  /// </summary>
  /// <param name="s">Строка</param>
  /// <returns>Улучшенная строка</returns>
  private static string BeforeNormalization(string s) =>
    s.Replace("〈", "⟨").Replace("〉", "⟩").Replace("ι", " \u0345");

  /// <summary>
  /// Действия после нормализации
  /// </summary>
  /// <param name="s">Строка</param>
  /// <returns>Результат</returns>
  private static string AfterNormalization(string s)
  {
    var sb = new StringBuilder(s);
    sb = TabReplace(sb, Path.Combine(PATH_LOCALE_SHARE, "chars0.tab"));
    sb = TabReplace(sb, Path.Combine(PATH_LOCALE_SHARE, "chars2.tab"));
    //sb = TabReplace(sb, Path.Combine(PATH_LOCALE_SHARE, "chars3.tab"));
    return sb.ToString();
  }

  /// <summary>
  /// Заменяем символы
  /// </summary>
  /// <param name="sb">Где надо менять</param>
  /// <param name="file">Символы</param>
  /// <returns>Изменённый стрингбилдер</returns>
  private static StringBuilder TabReplace(StringBuilder sb, string file)
  {
    var temp = File.ReadAllLines(file);
    foreach (var t in temp)
    {
      var x = t.Split(new char[] { '\t' });
      sb.Replace(x[0], x[1]);
    }
    return sb;
  }

  /// <summary>
  /// Убираем концы строк в самом конце файла
  /// </summary>
  /// <param name="s">Строка</param>
  /// <returns>Результат</returns>
  private static string EndLines(string s)
  {
    int i = s.Length;
    if ((s[i - 1] == '\n' || s[i - 1] == 0x1a) && i >= 0)
    {
      i = i - 1;
      while ((s[i - 1] == '\n' || s[i - 1] == 0x1a) && i >= 0)
        i = i - 1;
    }
    else
      return s;

    return s.Substring(0, i);
  }
}