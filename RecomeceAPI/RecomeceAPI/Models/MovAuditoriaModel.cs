using System.ComponentModel.DataAnnotations;

namespace RecomeceAPI.Models
{
  public class MovAuditoriaModel
  {
    [Required(ErrorMessage = "O campo {0} é obrigatório.")] 
 public long Id{get;set;}
 
 public DateTime DataInicial{get;set;}
 
 public long IdResponsavel{get;set;}
 
 public long IdTabela{get;set;}
 [MaxLength(100, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")] 
 
 public string NomeTabela{get;set;}
 [MaxLength(800, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")] 
 
 public string Observacao{get;set;}
 [MaxLength(8000, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")] 
 
 public string Texto{get;set;}
 
 public byte TipoAcao{get;set;}
 
 public byte TipoResponsavel{get;set;}

public MovAuditoriaModel() 
{
NomeTabela = ""; 
Observacao = ""; 
Texto = ""; 

}
  }
}