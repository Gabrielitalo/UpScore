using System.ComponentModel.DataAnnotations;

namespace RecomeceAPI.Models
{
  public class CadEmpresasLicencasModel
  {
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    public long Id { get; set; }

    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    public CadEmpresasModel CadEmpresas { get; set; } = new CadEmpresasModel();

    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    public CadLicencasModel CadLicencas { get; set; } = new CadLicencasModel();

    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    public int Ativo { get; set; } = 1;

    public DateTime DataInicio { get; set; } = DateTime.Now;

    public DateTime DataFim { get; set; }
  }

}
