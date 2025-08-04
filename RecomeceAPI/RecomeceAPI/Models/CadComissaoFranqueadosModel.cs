using System.ComponentModel.DataAnnotations;

namespace RecomeceAPI.Models
{
  public class CadComissaoFranqueadosModel
  {
    [Required(ErrorMessage = "O campo {0} é obrigatório.")] 
 public long Id{get;set;}
 
 public decimal Comissao{get;set;}
 
 public byte Modo{get;set;}
[Required(ErrorMessage = "O campo {0} é obrigatório.")] [MaxLength(300, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")] 
 
 public string Nome{get;set;}
 
 public byte Tipo{get;set;}
 
 public decimal Valor{get;set;}

public CadComissaoFranqueadosModel() 
{
Nome = ""; 

}
  }
}