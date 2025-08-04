using RecomeceAPI.Services.Common;

namespace RecomeceAPI.Models.Serasa.Layout
{
  public class RegA900
  {
    public string TipoReg { get; set; }      // Tipo de Registro
    public string Codigo { get; set; }      // Subtipo de Registro
    public string MensagemReduzida { get; set; }     // Mensagem informativa
    public string MensagemCompleta { get; set; }     // Mensagem informativa
    public string Filler { get; set; }       // Uso da Serasa

    public RegA900(string rawData)
    {
      if (rawData is null)
        return;

      TipoReg = ExtensionService.NovoSubstring(rawData, 0, 4).Trim();
      Codigo = ExtensionService.NovoSubstring(rawData, 4, 6).Trim();
      MensagemReduzida = ExtensionService.NovoSubstring(rawData, 10, 32).Trim();
      MensagemReduzida = ExtensionService.NovoSubstring(rawData, 42, 70).Trim();
      Filler = ExtensionService.NovoSubstring(rawData, 112, 3).Trim();
    }
  }
}
