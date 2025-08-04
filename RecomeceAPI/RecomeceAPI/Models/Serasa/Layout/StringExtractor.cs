using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecomeceAPI.Models.Serasa.Layout
{
  public class StringExtractor
  {
    public Dictionary<string, string> ExtractData(string input, List<LayoutRule> rules)
    {
      var extractedData = new Dictionary<string, string>();

      foreach (var rule in rules)
      {
        // Ajusta para zero-based index (C# usa zero-based index)
        int startIndex = rule.Start - 1;
        int length = rule.Length;

        // Garante que a posição e o comprimento sejam válidos
        if (startIndex >= 0 && startIndex + length <= input.Length)
        {
          string value = input.Substring(startIndex, length).Trim();
          extractedData[rule.Mnemonic.ToString()] = value;
        }
        else
        {
          extractedData[rule.Mnemonic.ToString()] = "INVALID POSITION";
        }
      }

      return extractedData;
    }
  }
}
