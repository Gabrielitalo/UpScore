using System.ComponentModel.DataAnnotations;

namespace RecomeceAPI.Models
{
  public class CidadesModel
  {
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    public long Id { get; set; }

    public int CodigoIBGE { get; set; }
    public EstadosModel Estados { get; set; } = new EstadosModel();
    [MaxLength(500, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]

    public string NomeCidade { get; set; }

    public CidadesModel()
    {
      NomeCidade = "";
    }
  }
}