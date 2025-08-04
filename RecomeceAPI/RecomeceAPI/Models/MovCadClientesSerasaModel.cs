using RecomeceAPI.Models.Serasa.Layout;
using System.ComponentModel.DataAnnotations;

namespace RecomeceAPI.Models
{
  public class MovCadClientesSerasaModel
  {
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    public long Id { get; set; }

    public decimal AcoesJudiciais { get; set; }

    public decimal AnotacoesSPC { get; set; }
    [MaxLength(2000, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]

    public string ArquivoEnvio { get; set; }

    public string ArquivoRetorno { get; set; }

    public decimal ChequesSemFundos { get; set; }

    public decimal Concordatas { get; set; }

    public DateTime DataHoraConsulta { get; set; }
    public string MarkID { get; set; }

    public decimal DividasVencidas { get; set; }
    public CadClientesModel CadClientes { get; set; } = new CadClientesModel();
    public CadEquipeModel CadEquipe { get; set; } = new CadEquipeModel();

    public decimal PartFalencias { get; set; }

    public decimal PendenciasBancariasRefin { get; set; }

    public decimal PendenciasComerciaisPefin { get; set; }

    public decimal Protestos { get; set; }

    public int Score { get; set; }
    public int StatusConsulta { get; set; }

    public byte Situacao { get; set; }

    public byte Tipo { get; set; }

    public decimal TotalNegativacao { get; set; }
    public Concentre Concentre = new Concentre();

    public MovCadClientesSerasaModel()
    {
      ArquivoEnvio = "";

    }
  }
}