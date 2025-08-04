using RecomeceAPI.Services.Common;

namespace RecomeceAPI.Models.Serasa.Layout
{
  public class RegI102
  {
    public string TipoReg { get; set; } // Tipo de registro
    public string Subtipo { get; set; } // Subtipo de registro (fixo = "00")
    public string UF { get; set; } // Unidade Federativa (2 caracteres)
    public string Municipio { get; set; } // Nome do município (50 caracteres)

    public RegI102(string rawData)
    {
      if (rawData is null)
        return;
      TipoReg = ExtensionService.NovoSubstring(rawData, 0, 4).Trim();
      Subtipo = ExtensionService.NovoSubstring(rawData, 4, 2).Trim();
      UF = ExtensionService.NovoSubstring(rawData, 6, 2).Trim();
      Municipio = ExtensionService.NovoSubstring(rawData, 8, 50).Trim();
    }
  }

}
