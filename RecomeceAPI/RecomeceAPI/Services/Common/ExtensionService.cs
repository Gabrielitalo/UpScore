using System.Data;
using System.Globalization;
using System.Text.Json;
using System.Xml.Linq;

namespace RecomeceAPI.Services.Common
{
  public static class ExtensionService
  {
    public static bool isTableExists(this DataSet ds, int index)
    {
      if (ds.Tables.Count > index)
      {
        if (ds.Tables[index].Rows.Count > 0)
          return true;
        else
          return false;
      }
      else
        return false;
    }
    public static int ConvertStrToInt(this string str)
    {
      if (int.TryParse(str, out int result))
        return result;
      return 0;
    }
    public static decimal ConvertStrToDecimal(this string str)
    {
      if (decimal.TryParse(str, out decimal result))
        return result;
      return 0;
    }
    public static string ConvertBrlToInvariant(string valorBrl)
    {
      if (string.IsNullOrWhiteSpace(valorBrl))
        return "0";

      // Remove símbolo de moeda e espaços
      valorBrl = valorBrl.Replace("R$", "").Trim();

      // Converte para decimal usando pt-BR
      var ptBr = new CultureInfo("pt-BR");
      if (!decimal.TryParse(valorBrl, NumberStyles.Number | NumberStyles.AllowCurrencySymbol, ptBr, out var valorDecimal))
        throw new FormatException($"Valor inválido: {valorBrl}");

      // Retorna como string no formato invariant (ponto como separador decimal)
      return valorDecimal.ToString(CultureInfo.InvariantCulture);
    }
    public static DateTime ConvertStrToDate(this string str)
    {
      if (DateTime.TryParse(str, out DateTime result))
        return result;
      return DateTime.MinValue;
    }
    public static int ConvertDrToInt(this DataRow dr, string col)
    {
      if (DBNull.Value.Equals(dr[col]))
        return 0;
      else
        return Convert.ToInt32(dr[col]);
    }
    public static decimal ConvertDrToDecimal(this DataRow dr, string col)
    {
      if (DBNull.Value.Equals(dr[col]))
        return 0;
      else
        return Convert.ToDecimal(dr[col]);
    }
    public static DateTime ConvertDrToDateTime(this DataRow dr, string col)
    {
      if (DBNull.Value.Equals(dr[col]))
        return DateTime.MinValue;
      else
        return Convert.ToDateTime(dr[col]);
    }
    public static object IsDataTime(this DateTime? dt)
    {
      if (dt == null || dt == DateTime.MinValue)
        return DBNull.Value;
      else
        return dt;
    }
    public static string RemoveBase64Prefix(string base64String)
    {
      int index = base64String.IndexOf("base64,");
      if (index >= 0)
        return base64String.Substring(index + 7);
      return base64String;
    }

    public static string BuscaPropriedadeJson(JsonElement root, string key)
    {
      if (root.TryGetProperty(key, out JsonElement idElement))
        return idElement.GetString() ?? "";
      else
        return "";
    }
    public static TimeSpan DatesDiffs(DateTime start, DateTime end)
    {
      return end - start;
    }
    public static double GetMinutesDiff(DateTime start, DateTime end)
    {
      return DatesDiffs(start, end).TotalMinutes;
    }
    public static double GetHourDiff(DateTime start, DateTime end)
    {
      return DatesDiffs(start, end).TotalHours;
    }
    public static double GetDaysDiff(DateTime start, DateTime end)
    {
      return DatesDiffs(start, end).TotalDays;
    }
    public static DateTime CurrentDateTime()
    {
      return DateTime.Now;
    }
    public static DateTime FirstDayOfMonth(DateTime dt)
    {
      return new DateTime(dt.Year, dt.Month, 1);
    }
    public static DateTime LastDayOfMonth(DateTime dt)
    {
      return new DateTime(dt.Year, dt.Month, DateTime.DaysInMonth(dt.Year, dt.Month));
    }
    public static string CleanSpecialChars(string text)
    {
      if (string.IsNullOrEmpty(text))
        return "";
      else
        return text.Replace("-", "").Replace(".", "").Replace("/", "").Replace("_", "").Replace("(", "").Replace(")", "").Replace(".", "").Replace("-", "").Replace("+", "").Trim();
    }
    public static Dictionary<string, string> GetMimeTypes()
    {
      return new Dictionary<string, string>
      {
        {".txt", "text/plain" },
        {".pdf", "application/pdf" },
        {".doc", "application/vnd.ms-word" },
        {".docx", "application/vnd.ms-word" },
        {".xls", "application/vnd.ms-excel" },
        {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
        {".png", "image/png" },
        {".jpg", "image/jpeg" },
        {".jpeg", "image/jpeg" },
        {".gif", "image/gif" },
        {".csv", "text/csv" },
        {".ico", "text/ico" },
        {".zip", "application/zip" },
        {".rar", "application/rar" },
      };
    }
    public static T SafeConvert<T>(object value, T defaultValue = default)
    {
      try
      {
        if (value == null || value == DBNull.Value)
          return defaultValue;

        return (T)Convert.ChangeType(value, typeof(T));
      }
      catch
      {
        return defaultValue;
      }
    }
    public static int OffsetCalculator(int paginaAtual, int itensPorPagina)
    {
      if (paginaAtual < 1 || paginaAtual == 0) paginaAtual = 1;
      if (itensPorPagina < 1) itensPorPagina = 10;

      int offset = (paginaAtual - 1) * itensPorPagina;
      return offset;
    }
    public static string NovoSubstring(string texto, int posIni, int posFin)
    {
      if (!string.IsNullOrEmpty(texto))
      {
        if (texto.Length >= posFin)
        {
          return texto.Substring(posIni, posFin);
        }
      }
      return texto;
    }
    public static decimal SubstringBuscaDecimal(string texto, int posIni, int posFin)
    {
      try
      {
        string valor = NovoSubstring(texto, posIni, posFin);
        if (valor.Length > 0)
        {
          string inteiro = NovoSubstring(valor, 0, 13);
          string dec = NovoSubstring(valor, 13, 2);
          return Convert.ToDecimal($"{inteiro}.{dec}");
        }
        return 0;
      }
      catch
      {
        return 0;
      }
    }
    public static decimal SubstringParseDecimal(string texto, int posIni, int posFin)
    {
      try
      {
        string valor = NovoSubstring(texto, posIni, posFin);
        if (valor.Length > 0)
        {
          decimal v = Convert.ToDecimal(valor);
          return v / 100;
        }
        return 0;
      }
      catch
      {
        return 0;
      }
    }
    public static string AplicarMascaraMoeda(decimal valor)
    {
      return valor.ToString("C", new System.Globalization.CultureInfo("pt-BR"));
    }
    public static string ConverterParaDataBr(string dataRaw)
    {
      if (DateTime.TryParseExact(dataRaw, "yyyy-MM-dd",
          System.Globalization.CultureInfo.InvariantCulture,
          System.Globalization.DateTimeStyles.None, out DateTime data))
      {
        return data.ToString("dd/MM/yyyy");
      }

      return "Data inválida";
    }
    public static string AplicarMascaraCpfCnpj(string documento)
    {
      if (string.IsNullOrWhiteSpace(documento))
        return string.Empty;

      // Remove qualquer caractere que não seja número
      string numeros = new string(documento.Where(char.IsDigit).ToArray());

      if (numeros.Length == 11)
      {
        // CPF: 000.000.000-00
        return Convert.ToUInt64(numeros).ToString(@"000\.000\.000\-00");
      }
      else if (numeros.Length == 14)
      {
        // CNPJ: 00.000.000/0000-00
        return Convert.ToUInt64(numeros).ToString(@"00\.000\.000\/0000\-00");
      }

      return documento; // retorna como veio se não for 11 ou 14 dígitos
    }
    public static string? GetXmlValueByPath(string xml, string ns, string path)
    {
      try
      {
        var doc = XDocument.Parse(xml);
        XNamespace name = ns;

        // Quebra o caminho em partes
        var tags = path.Split('/', StringSplitOptions.RemoveEmptyEntries);

        XElement? current = doc.Root;

        foreach (var tag in tags)
        {
          if (current == null) break;

          current = current.Element(name + tag);
        }

        return current?.Value.Trim();
      }
      catch (Exception ex)
      {
        System.Diagnostics.Debug.WriteLine(ex.ToString());
        return "";
      }
    }

  }
}
