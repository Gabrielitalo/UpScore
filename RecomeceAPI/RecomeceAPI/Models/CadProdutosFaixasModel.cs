using System.ComponentModel.DataAnnotations;

namespace RecomeceAPI.Models
{
  public class CadProdutosFaixasModel
  {
    [Required(ErrorMessage = "O campo {0} é obrigatório.")] 
 public long Id{get;set;}
public CadProdutosModel CadProdutos { get; set; } = new CadProdutosModel();
 
 public decimal ValorDivida{get;set;}
 
 public decimal ValorEntradaMinima{get;set;}
 
 public decimal ValorServico{get;set;}

public CadProdutosFaixasModel() 
{

}
  }
}