namespace RecomeceAPI.Models
{
  using System;
  using System.ComponentModel.DataAnnotations;

  public class MovContaCorrenteModel
  {
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    public string Id { get; set; }

    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    public CadEmpresasModel CadEmpresas { get; set; } = new CadEmpresasModel();

    [MaxLength(36, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string Fk_Origem { get; set; }

    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    public int OrigemTipo { get; set; } // 1 Consultas

    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    public int Situacao { get; set; } = 0; // 0 Gerado, 1 Pago

    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    public int Tipo { get; set; } // 1 Débito, 2 Crédito

    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    public decimal Valor { get; set; }

    [MaxLength(255, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string IdExtPagto { get; set; }
    public DateTime DataCriacao { get; set; } = DateTime.Now;
    public DateTime? DataPagamento { get; set; }

    [MaxLength(500, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string Historico { get; set; }

    public MovContaCorrenteModel()
    {
      Id = Guid.NewGuid().ToString();
      Fk_Origem = string.Empty;
      IdExtPagto = string.Empty;
      Historico = string.Empty;
    }
  }

}
