using System.Text.RegularExpressions;

namespace Terminal;

public class Warnings
{
  private string _fileName;

  public bool IsFirstSpace;
  public bool IsOnik;
  public bool IsRetroClicks;
  public bool IsMultySpace;
  public bool IsMultyLF;
  public bool IsPUA;
  public bool IsBadQuotes;
  public bool IsSoftHyphen;
  public bool IsControlSymbol;
  public bool IsGarbageHyphens;

  public Warnings(string fileName)
  {
    _fileName = fileName;
  }

  /// <summary>
  /// Есть ли предупреждения?
  /// </summary>
  /// <returns>Да/нет</returns>
  public bool Warn() =>
    IsFirstSpace
    | IsOnik
    | IsRetroClicks
    | IsMultySpace
    | IsMultyLF
    | IsPUA
    | IsBadQuotes
    | IsSoftHyphen
    | IsControlSymbol
    | IsGarbageHyphens;

  /// <summary>
  /// Проверка строки на предупреждения
  /// </summary>
  /// <param name="s">Строка</param>
  /// <param name="warn">Предупреждения</param>
  public void Inspection(string s)
  {
    // Пробелы в начале строки
    if (Regex.IsMatch(s, @"\A\s") || Regex.IsMatch(s, @"\n\s"))
      this.IsFirstSpace = true;
    // Несколько пробелов подряд
    if (Regex.IsMatch(s, @"\s\s"))
      this.IsMultySpace = true;
    // Оник (Ѹ, ѹ) одним символом
    if (Regex.IsMatch(s, "Ѹ", RegexOptions.IgnoreCase))
      this.IsOnik = true;
    // Есть двойной восклицательный знак (‼). Ретрофлексивный щелчок?
    if (s.Contains("‼"))
      this.IsRetroClicks = true;
    // Проверка наличия нескольких концов строки подряд
    if (s.Contains("\n\n"))
      this.IsMultyLF = true;
    // Есть ли символы из ПУА
    if (Regex.IsMatch(s, @"[\ue000-\uf8ff]"))
      this.IsPUA = true;
    // Есть кавычки “”
    if (s.Contains("”") || s.Contains("“"))
      this.IsBadQuotes = true;
    // Есть мягкий перенос
    if (s.Contains("\u00ad"))
      this.IsSoftHyphen = true;
    // Есть контрольные символы
    if (Regex.IsMatch(s, @"[\x00-\x08\x0b-\x1f\x7f-\x9f]"))
      this.IsControlSymbol = true;
    // Мусорные переносы
    if (s.Contains("-\n"))
      this.IsGarbageHyphens = true;
  }

  /// <summary>
  /// Вывод имеющихся предупреждений для файла
  /// </summary>
  /// <param name="warn">Предупреждения</param>
  /// <param name="file">Имя файла</param>
  public void Result()
  {
    if (this.Warn())
      return;

    Console.WriteLine($"В файле \"{_fileName}\" есть:");
    if (this.IsFirstSpace)
      Console.WriteLine("\tпробелы в начале строки");
    if (this.IsOnik)
      Console.WriteLine("\tОник (Оу оу) одним символом");
    if (this.IsRetroClicks)
      Console.WriteLine("\tдвойной восклицательный знак (!!). Ретрофлексивный щелчок?");
    if (this.IsMultySpace)
      Console.WriteLine("\tнесколько пробелов подряд");
    if (this.IsMultyLF)
      Console.WriteLine("\tнесколько концов строки подряд");
    if (this.IsPUA)
      Console.WriteLine("\tсимволы из ПУА");
    if (this.IsBadQuotes)
      Console.WriteLine("\tплохие кавычки ``''");
    if (this.IsSoftHyphen)
      Console.WriteLine("\tмягкий перенос -");
    if (this.IsControlSymbol)
      Console.WriteLine("\tслужебные символы");
    if (this.IsGarbageHyphens)
      Console.WriteLine("\tмусорные переносы");
    Console.WriteLine("");
  }
}