using System.ComponentModel.DataAnnotations;

namespace RecomeceAPI.Models
{
  public class MovPropostasBeneficiariosModel
  {
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    public long Id { get; set; }
    public CadClientesModel CadClientes { get; set; } = new CadClientesModel();
    public MovPropostasModel MovPropostas { get; set; } = new MovPropostasModel();
    [MaxLength(800, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]

    public string Observacao { get; set; }

    public int Score { get; set; }

    public byte Tipo { get; set; }

    public decimal ValorContrato { get; set; }
    public string MarkID { get; set; }
    public decimal ValorDivida { get; set; }

    public MovPropostasBeneficiariosModel()
    {
      MarkID = "";
      Observacao = "";
    }
  }
}