using System.ComponentModel.DataAnnotations;

namespace RecomeceAPI.Models
{
  public class CadProdutosFaixasPagamentoModel
  {
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    public long Id { get; set; }
    [MaxLength(300, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]

    public string Descricao { get; set; }
    public CadProdutosFaixasModel CadProdutosFaixas { get; set; } = new CadProdutosFaixasModel();

    public byte Modo { get; set; }

    public byte Tipo { get; set; }

    public byte ValorMaxParcelas { get; set; }

    public CadProdutosFaixasPagamentoModel()
    {
      Descricao = "";

    }
  }
}