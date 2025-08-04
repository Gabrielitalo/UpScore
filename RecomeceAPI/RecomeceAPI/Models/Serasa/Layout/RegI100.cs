using RecomeceAPI.Services.Common;

namespace RecomeceAPI.Models.Serasa.Layout
{
  public class RegI100
  {
    public decimal TotalConsolidado { get; set; } // Calculado 
    public string TipoReg { get; set; } // Tipo de registro
    public string Subtipo { get; set; } // Subtipo de registro (fixo = "00")
    public string Nome { get; set; } // Nome obtido no Cadastro Base de CPF/CNPJ
    public string DataConfirm { get; set; } // Data do nascimento ou data de criação (DDMMAAAA)
    public int CodSitCad { get; set; } // Código da situação cadastral
    public string DataSit { get; set; } // Data da Situação do CPF/CNPJ (DDMMAAAA)
    public string CodRetorno { get; set; } // Código de retorno
    public string CCFIndisp { get; set; } // Indicador de CCF indisponível (uso da SERASA)
    public string Inscricao { get; set; } = string.Empty;

    public RegI100(string rawData)
    {
      if (rawData is null)
        return;
      TipoReg = ExtensionService.NovoSubstring(rawData, 0, 4).Trim();
      Subtipo = ExtensionService.NovoSubstring(rawData, 4, 2).Trim();
      Nome = ExtensionService.NovoSubstring(rawData, 6, 70).Trim();
      DataConfirm = ExtensionService.NovoSubstring(rawData, 76, 8).Trim();
      CodSitCad = int.Parse(ExtensionService.NovoSubstring(rawData, 84, 1).Trim());
      DataSit = ExtensionService.NovoSubstring(rawData, 85, 8).Trim();
      CodRetorno = ExtensionService.NovoSubstring(rawData, 93, 2).Trim();
      CCFIndisp = ExtensionService.NovoSubstring(rawData, 95, 1).Trim();
    }
  }

}
