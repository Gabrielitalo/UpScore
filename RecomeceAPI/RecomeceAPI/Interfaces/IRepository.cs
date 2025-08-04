using RecomeceAPI.Models.Common;
using System.Data;

namespace RecomeceAPI.Interfaces
{
    public interface IRepository<T>
  {
    Task<object> SaveAsync(T entity);
    Task<object> UpdateAsync(T entity);
    Task<object> DeleteAsync(long id);
    Task<T> GetByIdAsync(long id);
    Task<bool> IsExists(long id);
    Task<PaginationModel> GetAllAsync(int page, int itensPerPage);
    T SetByDataSet(DataSet ds);
    T ConvertDataRowToObj(DataRow dr);
    List<T> GetListByDataSet(DataSet ds);
    void AddIdParameter(long id);  // Prepara parâmetros com somente o ID
    void AddEntityParameters(T entity);  // Prepara todos os parâmetros para a entidade
  }
}
