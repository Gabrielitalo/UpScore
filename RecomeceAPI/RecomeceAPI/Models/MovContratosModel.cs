using System.ComponentModel.DataAnnotations;

namespace RecomeceAPI.Models
{
  public class MovContratosModel
  {
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    public long Id { get; set; }
    public MovPropostasModel MovPropostasModel = new MovPropostasModel();
    public byte Situacao { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime DataFechamento { get; set; }
  }
}
