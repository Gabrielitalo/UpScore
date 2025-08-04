using System.ComponentModel.DataAnnotations;

namespace RecomeceAPI.Models
{
  public class MovPropostasArquivosModel
  {
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    public long Id { get; set; }

    public byte[] ArquivoBytes { get; set; } = new byte[1];

    public DateTime DataConferencia { get; set; }

    public DateTime DataCriacao { get; set; }

    public MovPropostasBeneficiariosModel MovPropostasBeneficiarios { get; set; } = new MovPropostasBeneficiariosModel();
    public MovPropostasModel MovPropostas { get; set; } = new MovPropostasModel();
    [MaxLength(300, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]

    public string NomeArquivo { get; set; }
    [MaxLength(800, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]

    public string Observacao { get; set; }

    public byte Situacao { get; set; }

    public byte Tipo { get; set; }

    public MovPropostasArquivosModel()
    {
      NomeArquivo = "";
      Observacao = "";

    }
  }
}