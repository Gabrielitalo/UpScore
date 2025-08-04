using System.ComponentModel.DataAnnotations;

namespace RecomeceAPI.Models
{
  public class CadEquipeModel
  {
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    public long Id { get; set; }
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    public byte Ativo { get; set; }
    [MaxLength(150, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]

    public string Bairro { get; set; }
    [MaxLength(20, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]

    public string Cep { get; set; }
    [MaxLength(100, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]

    public string Complemento { get; set; }
    [MaxLength(250, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]

    public string Email { get; set; }
    public CadEmpresasModel CadEmpresas { get; set; } = new CadEmpresasModel();
    [MaxLength(25, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]

    public string Inscricao { get; set; }
    [MaxLength(300, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]

    public string Logradouro { get; set; }
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [MaxLength(300, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]

    public string Nome { get; set; }
    [MaxLength(30, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]

    public string Numero { get; set; }

    [MaxLength(800, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string Senha { get; set; }
    [MaxLength(30, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]

    public string Telefone { get; set; }
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    public byte Tipo { get; set; }

    public CadEquipeModel()
    {
      Bairro = "";
      Cep = "";
      Complemento = "";
      Email = "";
      Inscricao = "";
      Logradouro = "";
      Nome = "";
      Numero = "";
      Senha = "";
      Telefone = "";

    }
  }
}