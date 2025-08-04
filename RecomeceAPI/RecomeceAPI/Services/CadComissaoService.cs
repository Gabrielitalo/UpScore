using RecomeceAPI.Interfaces;
using RecomeceAPI.Models;
using RecomeceAPI.Models.Common;
using RecomeceAPI.Server.Database;
using RecomeceAPI.Services.Common;
using System.Data;

namespace RecomeceAPI.Services
{
    public class CadComissaoService : IRepository<CadComissaoModel>
  {
    private readonly DbContext _dbContext;
    private readonly PaginationService _paginationService;
    private readonly GenericModelValidatorService _genericModelValidatorService;

    public CadComissaoService(DbContext dbContext, 
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
    public void AddEntityParameters(CadComissaoModel entity)
    {
      AddIdParameter(entity.Id);
      _dbContext.Parametros.AddItem("@p_Ativo", entity.Ativo); 
      _dbContext.Parametros.AddItem("@p_Comissao", entity.Comissao); 
      _dbContext.Parametros.AddItem("@p_Modo", entity.Modo); 
      _dbContext.Parametros.AddItem("@p_Nome", entity.Nome); 
      _dbContext.Parametros.AddItem("@p_Valor", entity.Valor); 

    }
    public async Task<object> SaveAsync(CadComissaoModel entity)
    {
      var validation = _genericModelValidatorService.GenericValidator(entity);
      if(validation.Count > 0)
        return validation;

      string cmd = @"Insert Into CadComissao (Id, Ativo, Comissao, Modo, Nome, Valor) Select @p_Id, @p_Ativo, @p_Comissao, @p_Modo, @p_Nome, @p_Valor; Select LAST_INSERT_ID();";
      AddEntityParameters(entity);
      return await _dbContext.SaveAsync(cmd, 0);
    }
    public async Task<object> UpdateAsync(CadComissaoModel entity)
    {
      var validation = _genericModelValidatorService.GenericValidator(entity);
      if (validation.Count > 0)
        return validation;

      if (await IsExists(entity.Id) == false)
        return NotificationService.NoExists();

      string cmd = @"Update CadComissao
      Set 
      Id = @p_Id, 
      Ativo = @p_Ativo, 
      Comissao = @p_Comissao, 
      Modo = @p_Modo, 
      Nome = @p_Nome, 
      Valor = @p_Valor
      Where Id = @p_Id;";
      AddEntityParameters(entity);
      await _dbContext.ExecAsync(cmd, 0);
      return entity;
    }
    public async Task<CadComissaoModel> GetByIdAsync(long id)
    {
      string cmd = @"Select * From CadComissao Where Id = @p_Id;";
      AddIdParameter(id);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return SetByDataSet(result);
    }
    public async Task<PaginationModel> GetAllAsync(int page, int itemsPerPage)
    {
      string cmd = @"Select * From CadComissao Limit @p_Pag, @p_rows;";
      _dbContext.Parametros.AddItem("@p_Pag", page);
      _dbContext.Parametros.AddItem("@p_rows", itemsPerPage);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return _paginationService.GetPaginationData(result, page, itemsPerPage);
    }
    public async Task<object> DeleteAsync(long id)
    {
      if(await IsExists(id) == false)
        return NotificationService.NoExists();
      string cmd = @"Delete From CadComissao Where Id = @p_Id;";
      AddIdParameter(id);
      await _dbContext.ExecAsync(cmd, 0);
      return NotificationService.Success();
    }
    public List<CadComissaoModel> GetListByDataSet(DataSet ds)
    {
      var list = new List<CadComissaoModel>();

      if (ds?.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        foreach (DataRow row in ds.Tables[0].Rows)
          list.Add(ConvertDataRowToObj(row));

      return list;
    }
    public CadComissaoModel ConvertDataRowToObj(DataRow dr)
    {
      return new CadComissaoModel
      {
       Id = dr.Field<long>("Id"), 
       Ativo = dr.Field<byte>("Ativo"), 
       Comissao = dr.Field<decimal>("Comissao"), 
       Modo = dr.Field<byte>("Modo"), 
       Nome = dr.Field<string>("Nome") ?? "", 
       Valor = dr.Field<decimal>("Valor")
      };
    }
    public CadComissaoModel SetByDataSet(DataSet ds)
    {
      if (ds?.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        return ConvertDataRowToObj(ds.Tables[0].Rows[0]);
      return new CadComissaoModel(); 
    }

    public async Task<bool> IsExists(long id)
    {
      var item = await GetByIdAsync(id);
      return item.Id > 0;
    }
  }
}