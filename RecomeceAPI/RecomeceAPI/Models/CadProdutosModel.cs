using System.ComponentModel.DataAnnotations;

namespace RecomeceAPI.Models
{
  public class CadProdutosModel
  {
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    public long Id { get; set; }
    [MaxLength(13, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]

    public string Codigo { get; set; }
    public CadEmpresasModel CadEmpresas { get; set; } = new CadEmpresasModel();
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    public int LimpaNome { get; set; }
    public decimal Custo { get; set; }

    public DateTime DataFinal { get; set; }

    public DateTime DataInicial { get; set; }
    [MaxLength(60, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]

    public string Descricao { get; set; }

    public byte Situacao { get; set; }
    public int TipoConsulta { get; set; } = 1;

    public CadProdutosModel()
    {
      Codigo = "";
      Descricao = "";

    }
  }
}