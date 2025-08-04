namespace RecomeceAPI.Models
{
  using System;
  using System.ComponentModel.DataAnnotations;

  public class LogConsultasModel
  {
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    public string Id { get; set; }
    public long? Fk_CadEquipe { get; set; }
    public long? Fk_CadConsultas { get; set; }

    [MaxLength(30, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string Inscricao { get; set; }

    [MaxLength(36, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string MarkID { get; set; }

    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    public int Situacao { get; set; } = 1; // 1 Sucesso, 2 Falha
    public DateTime? DataHoraConsulta { get; set; }
    public decimal? Score { get; set; }
    public decimal? TotalNegativacao { get; set; }

    [MaxLength(2000, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string ArquivoEnvio { get; set; }
    public string ArquivoRetorno { get; set; }

    public LogConsultasModel()
    {
      //Id = Guid.NewGuid().ToString();
      Inscricao = string.Empty;
      MarkID = string.Empty;
      ArquivoEnvio = string.Empty;
      ArquivoRetorno = string.Empty;
    }
  }

}
