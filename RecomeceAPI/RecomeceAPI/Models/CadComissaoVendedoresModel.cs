using System.ComponentModel.DataAnnotations;

namespace RecomeceAPI.Models
{
  public class CadComissaoVendedoresModel
  {
    [Required(ErrorMessage = "O campo {0} é obrigatório.")] 
 public long Id{get;set;}
[Required(ErrorMessage = "O campo {0} é obrigatório.")] 
 public byte Ativo{get;set;}
public CadComissaoModel CadComissao { get; set; } = new CadComissaoModel();
public CadEquipeModel CadEquipe { get; set; } = new CadEquipeModel();

public CadComissaoVendedoresModel() 
{

}
  }
}