using System.ComponentModel.DataAnnotations;

namespace RecomeceAPI.Models
{
  public class MovPropostasModel
  {
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    public long Id { get; set; }

    public DateTime DataHoraCadastro { get; set; }

    public DateTime DataHoraFechamento { get; set; }
    public CadClientesModel CadClientes { get; set; } = new CadClientesModel();
    public CadEquipeModel CadEquipe { get; set; } = new CadEquipeModel();
    public CadProdutosModel CadProdutos { get; set; } = new CadProdutosModel();
    [MaxLength(60, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]

    public string NumeroContrato { get; set; }
    [MaxLength(800, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]

    public string Observacao { get; set; }

    public decimal PercDesconto { get; set; }
    public decimal ValorAvulso { get; set; }

    public byte Situacao { get; set; }

    public int Termometro { get; set; }

    public decimal ValorAprovado { get; set; }

    public decimal ValorContrato { get; set; }

    public decimal ValorDivida { get; set; }

    public decimal ValorEntrada { get; set; }

    public MovPropostasModel()
    {
      NumeroContrato = "";
      Observacao = "";

    }
  }
}