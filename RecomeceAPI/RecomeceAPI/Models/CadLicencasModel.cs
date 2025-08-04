using System.ComponentModel.DataAnnotations;

namespace RecomeceAPI.Models
{
  public class CadLicencasModel
  {
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    public long Id { get; set; }

    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    public int Status { get; set; }

    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    public decimal Valor { get; set; }

    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    public int QtdUsuarios { get; set; }

    [MaxLength(255, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string IdExtPagto { get; set; }

    public DateTime DataCriacao { get; set; } = DateTime.Now;

    public DateTime DataPagamento { get; set; }

    public CadLicencasModel()
    {
      IdExtPagto = "";
    }
  }

}
