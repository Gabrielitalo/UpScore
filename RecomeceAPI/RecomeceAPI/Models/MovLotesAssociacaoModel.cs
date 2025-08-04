using System.ComponentModel.DataAnnotations;

namespace RecomeceAPI.Models
{
  public class MovLotesAssociacaoModel
  {
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    public string Id { get; set; } = string.Empty;

    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    public CadEmpresasModel CadEmpresas { get; set; } = new CadEmpresasModel();

    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    public int Situacao { get; set; }

    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    public DateTime Data { get; set; }

    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    public int TotalBeneficiarios { get; set; }
    public decimal ValorTotal { get; set; }
    public int Tipo { get; set; } = 1; // 1 Envio, 2 Reenvio
    public int Serasa { get; set; }
    public int Boavista { get; set; }
    public int SPC { get; set; }
    public int CenprotSP { get; set; }
    public int CenprotNacional { get; set; }

  }
}
