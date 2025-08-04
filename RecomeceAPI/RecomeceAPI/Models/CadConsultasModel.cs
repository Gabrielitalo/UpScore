namespace RecomeceAPI.Models
{
  using System;
  using System.ComponentModel.DataAnnotations;

  public class CadConsultasModel
  {
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    public long Id { get; set; }

    [MaxLength(60, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string Nome { get; set; }

    [MaxLength(300, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
    public string Descricao { get; set; }
    public string ProductURL { get; set; }

    public DateTime? DataInicial { get; set; }

    public DateTime? DataFinal { get; set; }

    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    public decimal Custo { get; set; }

    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    public decimal Venda { get; set; }
    public int Tipo { get; set; } = 1; // 1 Serasa, 2 Boa Vista

    public CadConsultasModel()
    {
      Nome = string.Empty;
      Descricao = string.Empty;
      ProductURL = string.Empty;
    }
  }

}
