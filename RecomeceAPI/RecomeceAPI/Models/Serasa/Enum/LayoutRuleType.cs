using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecomeceAPI.Models.Serasa.Enum
{
  public enum LayoutRuleType
  {
    [Description("IN")]
    Input,

    [Description("OUT")]
    Output
  }

  // Método auxiliar para obter o valor associado
  public static class LayoutRuleTypeExtensions
  {
    public static string GetDescription(this LayoutRuleType value)
    {
      var fieldInfo = value.GetType().GetField(value.ToString());
      var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

      return attributes.Length > 0 ? attributes[0].Description : value.ToString();
    }
  }
}
