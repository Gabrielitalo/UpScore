using System.ComponentModel.DataAnnotations;

namespace RecomeceAPI.Models
{
  public class CadClientesModel
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
    [MaxLength(100, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]

    public string EstadoCivil { get; set; }
    public CadEquipeModel CadEquipe { get; set; } = new CadEquipeModel();
    [MaxLength(45, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]

    public string Inscricao { get; set; }
    [MaxLength(300, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]

    public string Logradouro { get; set; }
    [MaxLength(100, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]

    public string Nacionalidade { get; set; }
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [MaxLength(300, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]

    public string Nome { get; set; }
    [MaxLength(30, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]

    public string Numero { get; set; }
    [MaxLength(120, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]

    public string Profissao { get; set; }

    [MaxLength(800, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string Senha { get; set; }
    [MaxLength(30, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string Telefone { get; set; }
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    public byte Tipo { get; set; }
    public CidadesModel Cidades { get; set; } = new CidadesModel();

    public CadClientesModel()
    {
      Bairro = "";
      Cep = "";
      Complemento = "";
      Email = "";
      EstadoCivil = "";
      Inscricao = "";
      Logradouro = "";
      Nacionalidade = "";
      Nome = "";
      Numero = "";
      Profissao = "";
      Senha = "";
      Telefone = "";

    }
  }
}