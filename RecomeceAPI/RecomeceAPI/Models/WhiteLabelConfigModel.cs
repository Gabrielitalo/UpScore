namespace RecomeceAPI.Models
{
  public class WhiteLabelConfigModel
  {
    public long EmpresaId { get; set; }
    public decimal ValorNomeLote { get; set; }
    public int Tipo{ get; set; } // 0 Default, 1 Custom
    public string Nome { get; set; } = string.Empty;
    public string Logo { get; set; } = "default-logo.png";
    public string ModeloProposta { get; set; } = "default_proposal";
    public string ModeloContrato { get; set; } = "default_contract";

  }
}
