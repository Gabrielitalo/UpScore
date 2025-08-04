using RecomeceAPI.Models.Common;
using System.Data;

namespace RecomeceAPI.Interfaces
{
    public interface IPaginationService
  {
    int CalculatePage(int page, int itemsPerPage); // Calcula o índice inicial da página.
    int CalculateTotalPages(int totalItems, int itemsPerPage); // Calcula o número total de páginas.
    PaginationModel GetPaginationData(DataSet allItems, int page, int itemsPerPage);
  }
}
