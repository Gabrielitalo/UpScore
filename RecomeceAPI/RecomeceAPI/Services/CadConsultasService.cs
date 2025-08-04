using RecomeceAPI.Interfaces;
using RecomeceAPI.Models;
using RecomeceAPI.Models.Common;
using RecomeceAPI.Server.Database;
using RecomeceAPI.Services.Common;
using System.Data;

namespace RecomeceAPI.Services
{
  public class CadConsultasService : IRepository<CadConsultasModel>
  {
    private readonly DbContext _dbContext;
    private readonly PaginationService _paginationService;
    private readonly GenericModelValidatorService _genericModelValidatorService;

    public CadConsultasService(DbContext dbContext,
      PaginationService paginationService,
      GenericModelValidatorService genericModelValidatorService)
    {
      _dbContext = dbContext;
      _paginationService = paginationService;
      _genericModelValidatorService = genericModelValidatorService;
    }

    public void AddIdParameter(long id)
    {
      _dbContext.Parametros.AddItem("@p_Id", id);
    }

    public void AddEntityParameters(CadConsultasModel entity)
    {
      AddIdParameter(entity.Id);
      _dbContext.Parametros.AddItem("@p_Nome", entity.Nome);
      _dbContext.Parametros.AddItem("@p_Descricao", entity.Descricao);
      _dbContext.Parametros.AddItem("@p_DataInicial", entity.DataInicial);
      _dbContext.Parametros.AddItem("@p_DataFinal", entity.DataFinal);
      _dbContext.Parametros.AddItem("@p_Custo", entity.Custo);
      _dbContext.Parametros.AddItem("@p_Venda", entity.Venda);
      _dbContext.Parametros.AddItem("@p_ProductURL", entity.ProductURL);
      _dbContext.Parametros.AddItem("@p_Tipo", entity.Tipo);
    }

    public async Task<object> SaveAsync(CadConsultasModel entity)
    {
      var validation = _genericModelValidatorService.GenericValidator(entity);
      if (validation.Count > 0)
        return validation;

      string cmd = @"INSERT INTO CadConsultas (Id, Nome, Descricao, DataInicial, DataFinal, Custo, Venda, ProductURL, Tipo)
                     SELECT @p_Id, @p_Nome, @p_Descricao, @p_DataInicial, @p_DataFinal, @p_Custo, @p_Venda, @p_ProductURL, @p_Tipo;
                     SELECT LAST_INSERT_ID();";
      AddEntityParameters(entity);
      return await _dbContext.SaveAsync(cmd, 0);
    }

    public async Task<object> UpdateAsync(CadConsultasModel entity)
    {
      var validation = _genericModelValidatorService.GenericValidator(entity);
      if (validation.Count > 0)
        return validation;

      if (await IsExists(entity.Id) == false)
        return NotificationService.NoExists();

      string cmd = @"UPDATE CadConsultas
                     SET Nome = @p_Nome,
                         Descricao = @p_Descricao,
                         DataInicial = @p_DataInicial,
                         DataFinal = @p_DataFinal,
                         Custo = @p_Custo,
                         Venda = @p_Venda, 
                         ProductURL = @p_ProductURL,
                         Tipo = @p_Tipo
                     WHERE Id = @p_Id;";
      AddEntityParameters(entity);
      await _dbContext.ExecAsync(cmd, 0);
      return entity;
    }

    public async Task<CadConsultasModel> GetByIdAsync(long id)
    {
      string cmd = @"SELECT * FROM CadConsultas WHERE Id = @p_Id;";
      AddIdParameter(id);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return SetByDataSet(result);
    }

    public async Task<PaginationModel> GetAllAsync(int page, int itemsPerPage)
    {
      string cmd = @"SELECT * FROM CadConsultas Where DataFinal is null Order by Nome LIMIT @p_Pag, @p_rows;";
      _dbContext.Parametros.AddItem("@p_Pag", page);
      _dbContext.Parametros.AddItem("@p_rows", itemsPerPage);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return _paginationService.GetPaginationData(result, page, itemsPerPage);
    }

    public async Task<object> DeleteAsync(long id)
    {
      if (await IsExists(id) == false)
        return NotificationService.NoExists();

      string cmd = @"DELETE FROM CadConsultas WHERE Id = @p_Id;";
      AddIdParameter(id);
      await _dbContext.ExecAsync(cmd, 0);
      return NotificationService.Success();
    }

    public List<CadConsultasModel> GetListByDataSet(DataSet ds)
    {
      var list = new List<CadConsultasModel>();

      if (ds?.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        foreach (DataRow row in ds.Tables[0].Rows)
          list.Add(ConvertDataRowToObj(row));

      return list;
    }

    public CadConsultasModel ConvertDataRowToObj(DataRow dr)
    {
      return new CadConsultasModel
      {
        Id = dr.Field<long>("Id"),
        Nome = dr.Field<string>("Nome") ?? "",
        ProductURL = dr.Field<string>("ProductURL") ?? "",
        Descricao = dr.Field<string>("Descricao") ?? "",
        DataInicial = dr.Field<DateTime?>("DataInicial"),
        DataFinal = dr.Field<DateTime?>("DataFinal"),
        Custo = dr.Field<decimal>("Custo"),
        Venda = dr.Field<decimal>("Venda"),
        Tipo = dr.Field<int>("Tipo")
      };
    }

    public CadConsultasModel SetByDataSet(DataSet ds)
    {
      if (ds?.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        return ConvertDataRowToObj(ds.Tables[0].Rows[0]);
      return new CadConsultasModel();
    }

    public async Task<bool> IsExists(long id)
    {
      var item = await GetByIdAsync(id);
      return item.Id > 0;
    }
  }
}
