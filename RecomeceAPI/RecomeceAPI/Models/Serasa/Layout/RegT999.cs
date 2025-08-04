using RecomeceAPI.Services.Common;

namespace RecomeceAPI.Models.Serasa.Layout
{
  public class RegT999
  {
    public string TipoReg { get; set; }      // Tipo de Registro
    public string Codigo { get; set; }      // Subtipo de Registro
    public string Mensagem { get; set; }     // Mensagem de encerramento
    public string Filler { get; set; }       // Uso da Serasa

    public RegT999(string rawData)
    {
      if (rawData.Length < 115)
        throw new ArgumentException("Tamanho da string inválido para o registro T999");

      TipoReg = ExtensionService.NovoSubstring(rawData, 0, 4).Trim();
      Codigo = ExtensionService.NovoSubstring(rawData, 4, 3).Trim();
      Mensagem = ExtensionService.NovoSubstring(rawData, 7, 70).Trim();
      Filler = ExtensionService.NovoSubstring(rawData, 77, 38).Trim();
    }
  }
}
