using DocumentFormat.OpenXml.Office2010.Excel;

namespace RecomeceAPI.Models
{
  public class MovLotesEmpresasBeneficiariosModel
  {
    public string Id { get; set; } = string.Empty;
    public MovLotesEmpresasModel MovLotesEmpresas { get; set; } = new MovLotesEmpresasModel();
    public string Inscricao { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public int Situacao { get; set; } = 0;
  }
}
