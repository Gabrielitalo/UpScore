using RecomeceAPI.Models.Serasa.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecomeceAPI.Models.Serasa
{
  public sealed class LayoutRule
  {
    public int Seq { get; }
    public int Start { get; }
    public int Length { get; }
    public LayoutRuleDataType DataType { get; }
    public Mnemonic Mnemonic { get; }
    public string DefaultValue { get; }
    public string Description { get; }

    public LayoutRule(
        int seq,
        int start,
        int length,
        LayoutRuleDataType dataType,
        Mnemonic mnemonic,
        string defaultValue = "",
        string description = "")
    {
      Seq = seq;
      Start = start;
      Length = length;
      DataType = dataType;
      Mnemonic = mnemonic;
      DefaultValue = defaultValue;
      Description = description;
    }
  }

}
