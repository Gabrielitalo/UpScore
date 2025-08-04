using RecomeceAPI.Models.Serasa.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecomeceAPI.Models.Serasa.Layout
{
  public sealed class ConcentreB49C : ISearasaLayout
  {
    public List<LayoutRule> Fields()
    {
      return new List<LayoutRule>
            {
                new LayoutRule(1, 1, 4, LayoutRuleDataType.Alphanumeric, Mnemonic.Filler, "B49C", "Preencher com \"B49C\""),
                new LayoutRule(2, 5, 6, LayoutRuleDataType.Alphanumeric, Mnemonic.Filler, "", "Preencher com o código da estação chamadora no cliente (identificação de terminal ou computador). Campo opcional."),
                new LayoutRule(3, 11, 15, LayoutRuleDataType.Numeric, Mnemonic.NumDoc, "", "Informar o número do CPF (com 11 dígitos) ou o número do CNPJ (com 15 dígitos) a ser consultado. Ajuste à direita e preenchido com zeros à esquerda."),
                new LayoutRule(4, 26, 1, LayoutRuleDataType.Alphanumeric, Mnemonic.TipoPessoa, "", "Informar \"F\" = pessoa física; \"J\" = pessoa jurídica."),
                new LayoutRule(5, 27, 6, LayoutRuleDataType.Alphanumeric, Mnemonic.BaseCons, "C", "Informar \"C\""),
                new LayoutRule(6, 33, 2, LayoutRuleDataType.Alphanumeric, Mnemonic.Modalidade, "FI", "Informar \"FI\""),
                new LayoutRule(7, 35, 7, LayoutRuleDataType.Numeric, Mnemonic.VlrConsul, "", "Uso da SERASA"),
                new LayoutRule(8, 42, 12, LayoutRuleDataType.Alphanumeric, Mnemonic.CentroCust, "", "Informar Centro de Custo Obs.: Como é opcional, informar brancos se não for usar esse dado."),
                new LayoutRule(9, 54, 1, LayoutRuleDataType.Alphanumeric, Mnemonic.Codificado, "S", "Informar \"S\""),
                new LayoutRule(10, 55, 2, LayoutRuleDataType.Numeric, Mnemonic.QtdReg, "99", "Indicar a quantidade de registros por transmissão de dados que deseja receber (até 99)"),
                new LayoutRule(11, 57, 1, LayoutRuleDataType.Alphanumeric, Mnemonic.Conversa, "S", "Informar se deseja diálogo de comunicação? (S/N)"),
                new LayoutRule(12, 58, 3, LayoutRuleDataType.Alphanumeric, Mnemonic.Funcao, "INI", "Usar as funções: INI, CON, ENC, ERR ou FIM"),
                new LayoutRule(13, 61, 1, LayoutRuleDataType.Alphanumeric, Mnemonic.TpConsulta, "A", "Informar \"A\""),
                new LayoutRule(14, 62, 1, LayoutRuleDataType.Alphanumeric, Mnemonic.Atualiza, "N", "Informar \"N\""),
                new LayoutRule(15, 63, 18, LayoutRuleDataType.Alphanumeric, Mnemonic.IdentTerm, "", "Uso da SERASA"),
                new LayoutRule(16, 81, 10, LayoutRuleDataType.Alphanumeric, Mnemonic.ResCli, "", "Uso da SERASA"),
                new LayoutRule(17, 91, 1, LayoutRuleDataType.Alphanumeric, Mnemonic.Delts, "", "Uso da SERASA"),
                new LayoutRule(18, 92, 1, LayoutRuleDataType.Alphanumeric, Mnemonic.Cobra, "", "Uso da SERASA"),
                new LayoutRule(19, 93, 1, LayoutRuleDataType.Alphanumeric, Mnemonic.Passa, "", "Informar \"D\", se a consulta é de distribuidores"),
                new LayoutRule(20, 94, 1, LayoutRuleDataType.Alphanumeric, Mnemonic.ConsCollec, "N", "Informar \"N\""),
                new LayoutRule(21, 95, 57, LayoutRuleDataType.Alphanumeric, Mnemonic.Filler, "", "FILLER"),
                new LayoutRule(22, 152, 15, LayoutRuleDataType.Numeric, Mnemonic.Consultante, "", "Informar o CNPJ da empresa consultante com 15 posições (com filial e dígito). Ex: 99.999.999/0001–99 -> informar 099999999000199"),
                new LayoutRule(23, 167, 234, LayoutRuleDataType.Alphanumeric, Mnemonic.Filler, "", "FILLER")
            };
    }
  }
}
