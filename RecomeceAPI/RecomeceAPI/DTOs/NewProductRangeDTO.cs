using RecomeceAPI.Models;

namespace RecomeceAPI.DTOs
{
  public class NewProductRangeDTO
  {
    public CadProdutosFaixasModel CadProdutosFaixas = new CadProdutosFaixasModel();
    public List<CadProdutosFaixasPagamentoModel> CadProdutosFaixasPagamento = new List<CadProdutosFaixasPagamentoModel>();
  }
}
