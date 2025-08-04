using System.ComponentModel.DataAnnotations;

namespace RecomeceAPI.Models
{
  public class CadEmpresasContasBancariasModel
  {
    [Required(ErrorMessage = "O campo {0} é obrigatório.")] 
 public long Id{get;set;}
 [MaxLength(20, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")] 
 
 public string Agencia{get;set;}
[Required(ErrorMessage = "O campo {0} é obrigatório.")] 
 public byte Ativo{get;set;}
 [MaxLength(1000, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")] 
 
 public string ChavePix{get;set;}
 [MaxLength(10, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")] 
 
 public string CodBanco{get;set;}
 [MaxLength(30, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")] 
 
 public string Conta{get;set;}
public CadEmpresasModel CadEmpresas { get; set; } = new CadEmpresasModel();
[Required(ErrorMessage = "O campo {0} é obrigatório.")] 
 public byte Padrao{get;set;}

public CadEmpresasContasBancariasModel() 
{
Agencia = ""; 
ChavePix = ""; 
CodBanco = ""; 
Conta = ""; 

}
  }
}