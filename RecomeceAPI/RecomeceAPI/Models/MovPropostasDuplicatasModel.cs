using System.ComponentModel.DataAnnotations;

namespace RecomeceAPI.Models
{
  public class MovPropostasDuplicatasModel
  {
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    public string Id { get; set; }

    public byte CondPagamento { get; set; }

    public DateTime DataVencimento { get; set; }
    [MaxLength(1000, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]

    public string Descricao { get; set; }
    public MovPropostasModel MovPropostas { get; set; } = new MovPropostasModel();

    public byte Indice { get; set; }

    public decimal Valor { get; set; }

    public MovPropostasDuplicatasModel()
    {
      Descricao = "";
      Id = Guid.NewGuid().ToString(); 
    }
  }
}