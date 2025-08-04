using System.ComponentModel.DataAnnotations;

namespace RecomeceAPI.Models
{
  public class MovLotesEmpresasModel
  {
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    public string Id { get; set; } = string.Empty;  

    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    public CadEmpresasModel CadEmpresas { get; set; } = new CadEmpresasModel();

    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    public byte Situacao { get; set; }

    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    public DateTime Data { get; set; }

    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    public int TotalBeneficiarios { get; set; }
    public decimal Valor { get; set; }
    public string IdExtPagto {  get; set; } = string.Empty;
    public int Tipo { get; set; } = 1; // 1 Envio, 2 Reenvio
    public int Serasa { get; set; }
    public int Boavista { get; set; }
    public int SPC { get; set; }
    public int CenprotSP { get; set; }
    public int CenprotNacional { get; set; }

  }
}
