using System.ComponentModel.DataAnnotations;

namespace RecomeceAPI.Models
{
  public class MovConsultasCompradasModel
  {
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    public long Id { get; set; }

    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    public CadEmpresasModel CadEmpresas { get; set; }

    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    public byte Ativo { get; set; } // 0 = Inativo, 1 = Ativo

    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    public int Situacao { get; set; } // 0 = Gerado, 1 = Pago

    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    public int QtdConsultas { get; set; }

    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    public decimal Valor { get; set; }

    [MaxLength(255, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string IdExtPagto { get; set; }

    public DateTime DataCriacao { get; set; }

    public MovConsultasCompradasModel()
    {
      IdExtPagto = "";
      DataCriacao = DateTime.Now;
      CadEmpresas = new CadEmpresasModel();
    }
  }
}
