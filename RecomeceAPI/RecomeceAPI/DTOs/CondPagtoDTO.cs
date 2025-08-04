namespace RecomeceAPI.DTOs
{
  public class CondPagtoDTO
  {
    public int CondPagtoEntrada { get; set; }
    public DateTime DataEntrada { get; set; }
    public decimal ValorEntrada { get; set; }
    public int CondPagtoParcela { get; set; }
    public DateTime DataPrimeiraParcela { get; set; }
    public int NumeroParcela { get; set; }
  }
}
