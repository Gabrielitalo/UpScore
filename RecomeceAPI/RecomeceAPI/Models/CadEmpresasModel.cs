using System.ComponentModel.DataAnnotations;

namespace RecomeceAPI.Models
{
  public class CadEmpresasModel
  {
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    public long Id { get; set; }
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    public byte Ativo { get; set; }
    [MaxLength(150, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]

    public string Bairro { get; set; }

    public byte BancoDados { get; set; }
    [MaxLength(20, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]

    public string Cep { get; set; }
    [MaxLength(800, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]

    public string ChaveAPIAsaas { get; set; }
    [MaxLength(60, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]

    public string Complemento { get; set; }
    [MaxLength(25, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]

    public string CpfResponsavel { get; set; }
    [MaxLength(250, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]

    public string EmailResponsavel { get; set; }
    public CidadesModel Cidades { get; set; } = new CidadesModel();
    [MaxLength(800, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]

    public string IdAsaas { get; set; }

    public long IdEmpresaResponsavel { get; set; }
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [MaxLength(25, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]

    public string Inscricao { get; set; }
    [MaxLength(300, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]

    public string Logradouro { get; set; }
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [MaxLength(300, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]

    public string Nome { get; set; }
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [MaxLength(300, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]

    public string NomeResponsavel { get; set; }
    [MaxLength(30, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string Numero { get; set; }
    public byte Servidor { get; set; }
    [MaxLength(30, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string TelefoneResponsavel { get; set; }
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    public byte Tipo { get; set; }
    public WhiteLabelConfigModel WhiteLabelConfig { get; set; } = new WhiteLabelConfigModel();

    public CadEmpresasModel()
    {
      Bairro = "";
      Cep = "";
      ChaveAPIAsaas = "";
      Complemento = "";
      CpfResponsavel = "";
      EmailResponsavel = "";
      IdAsaas = "";
      Inscricao = "";
      Logradouro = "";
      Nome = "";
      NomeResponsavel = "";
      Numero = "";
      TelefoneResponsavel = "";
    }
  }
}