using System.ComponentModel.DataAnnotations;

namespace RecomeceAPI.Models
{
  public class EstadosModel
  {
    [Required(ErrorMessage = "O campo {0} é obrigatório.")] 
 public long Id{get;set;}
 [MaxLength(300, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")] 
 
 public string Capital{get;set;}
 
 public int CodigoIBGE{get;set;}
 [MaxLength(300, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")] 
 
 public string Nome{get;set;}
 [MaxLength(2, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")] 
 
 public string UF{get;set;}

public EstadosModel() 
{
Capital = ""; 
Nome = ""; 
UF = ""; 

}
  }
}