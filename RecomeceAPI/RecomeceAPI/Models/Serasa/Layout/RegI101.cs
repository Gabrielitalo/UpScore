using RecomeceAPI.Services.Common;

namespace RecomeceAPI.Models.Serasa.Layout
{
  public class RegI101
  {
    public string TipoReg { get; set; } // Tipo de registro
    public string Subtipo { get; set; } // Subtipo de registro (fixo = "00")
    public string NomeMae { get; set; } // Nome da mãe ou Nome Fantasia (60 caracteres)

    public RegI101(string rawData)
    {
      if (rawData is null)
        return;
      TipoReg = ExtensionService.NovoSubstring(rawData, 0, 4).Trim();
      Subtipo = ExtensionService.NovoSubstring(rawData, 4, 2).Trim();
      NomeMae = ExtensionService.NovoSubstring(rawData, 6, 60).Trim();
    }
  }

}
