using RecomeceAPI.Interfaces;
using RecomeceAPI.Models.Common;
using System.Data;

namespace RecomeceAPI.Services.Common
{

    public class PaginationService : IPaginationService
  {
    public int CalculatePage(int page, int itemsPerPage)
    {
      throw new NotImplementedException();
    }

    // Calcula o total de páginas com base no total de itens e itens por página
    public int CalculateTotalPages(int totalItems, int itemsPerPage)
    {
      if (itemsPerPage <= 0)
      {
        return 0; // Caso a divisão por zero ocorra
      }

      return (int)Math.Ceiling((double)totalItems / itemsPerPage);
    }

    // Retorna os dados paginados dentro da model Paginacao
    public PaginationModel GetPaginationData(DataSet allItems, int page, int itemsPerPage)
    {
      // Garantir que o número da página seja válido
      if (page < 1)
        page = 1;

      if (itemsPerPage < 1)
        itemsPerPage = 10; // Valor padrão de itens por página

      // Calcula o total de itens
      int totalItens = 0;
      try
      {
        if (allItems.isTableExists(1))
          totalItens = Convert.ToInt32(allItems.Tables[1].Rows[0]["totalItens"]);
        else
          totalItens = allItems.isTableExists(0) ? allItems.Tables[0].Rows.Count : 0;
      }
      catch
      {
        totalItens = 0;
      }

      // Calcula o total de páginas
      var pageCount = CalculateTotalPages(totalItens, itemsPerPage);

      // Verifica se a página solicitada não ultrapassa o número total de páginas
      if (page > pageCount)
        page = pageCount;

      // Calcula o índice inicial para a página solicitada
      var startIndex = (page - 1) * itemsPerPage;

      // Seleciona os itens para a página atual
      var paginatedItems = allItems.Tables[0];

      object totals = allItems.isTableExists(2) ? allItems.Tables[2] : new object();

      // Retorna o objeto de paginação com os dados preenchidos
      return new PaginationModel
      {
        PageCount = pageCount,
        TotalItens = totalItens,
        CurrentPage = page,
        ItensPerPage = itemsPerPage,
        Itens = paginatedItems, // Os itens paginados
        Totals = totals // Totalizadores do contexto
      };
    }
  }
}
