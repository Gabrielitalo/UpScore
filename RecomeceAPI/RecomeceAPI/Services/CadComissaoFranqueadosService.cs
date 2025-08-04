using RecomeceAPI.Interfaces;
using RecomeceAPI.Models;
using RecomeceAPI.Models.Common;
using RecomeceAPI.Server.Database;
using RecomeceAPI.Services.Common;
using System.Data;

namespace RecomeceAPI.Services
{
    public class CadComissaoFranqueadosService : IRepository<CadComissaoFranqueadosModel>
  {
    private readonly DbContext _dbContext;
    private readonly PaginationService _paginationService;
    private readonly GenericModelValidatorService _genericModelValidatorService;

    public CadComissaoFranqueadosService(DbContext dbContext, 
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
    public void AddEntityParameters(CadComissaoFranqueadosModel entity)
    {
      AddIdParameter(entity.Id);
      _dbContext.Parametros.AddItem("@p_Comissao", entity.Comissao); 
      _dbContext.Parametros.AddItem("@p_Modo", entity.Modo); 
      _dbContext.Parametros.AddItem("@p_Nome", entity.Nome); 
      _dbContext.Parametros.AddItem("@p_Tipo", entity.Tipo); 
      _dbContext.Parametros.AddItem("@p_Valor", entity.Valor); 

    }
    public async Task<object> SaveAsync(CadComissaoFranqueadosModel entity)
    {
      var validation = _genericModelValidatorService.GenericValidator(entity);
      if(validation.Count > 0)
        return validation;

      string cmd = @"Insert Into CadComissaoFranqueados (Id, Comissao, Modo, Nome, Tipo, Valor) Select @p_Id, @p_Comissao, @p_Modo, @p_Nome, @p_Tipo, @p_Valor; Select LAST_INSERT_ID();";
      AddEntityParameters(entity);
      return await _dbContext.SaveAsync(cmd, 0);
    }
    public async Task<object> UpdateAsync(CadComissaoFranqueadosModel entity)
    {
      var validation = _genericModelValidatorService.GenericValidator(entity);
      if (validation.Count > 0)
        return validation;

      if (await IsExists(entity.Id) == false)
        return NotificationService.NoExists();

      string cmd = @"Update CadComissaoFranqueados
      Set 
      Id = @p_Id, 
      Comissao = @p_Comissao, 
      Modo = @p_Modo, 
      Nome = @p_Nome, 
      Tipo = @p_Tipo, 
      Valor = @p_Valor
      Where Id = @p_Id;";
      AddEntityParameters(entity);
      await _dbContext.ExecAsync(cmd, 0);
      return entity;
    }
    public async Task<CadComissaoFranqueadosModel> GetByIdAsync(long id)
    {
      string cmd = @"Select * From CadComissaoFranqueados Where Id = @p_Id;";
      AddIdParameter(id);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return SetByDataSet(result);
    }
    public async Task<PaginationModel> GetAllAsync(int page, int itemsPerPage)
    {
      string cmd = @"Select * From CadComissaoFranqueados Limit @p_Pag, @p_rows;";
      _dbContext.Parametros.AddItem("@p_Pag", page);
      _dbContext.Parametros.AddItem("@p_rows", itemsPerPage);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return _paginationService.GetPaginationData(result, page, itemsPerPage);
    }
    public async Task<object> DeleteAsync(long id)
    {
      if(await IsExists(id) == false)
        return NotificationService.NoExists();
      string cmd = @"Delete From CadComissaoFranqueados Where Id = @p_Id;";
      AddIdParameter(id);
      await _dbContext.ExecAsync(cmd, 0);
      return NotificationService.Success();
    }
    public List<CadComissaoFranqueadosModel> GetListByDataSet(DataSet ds)
    {
      var list = new List<CadComissaoFranqueadosModel>();

      if (ds?.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        foreach (DataRow row in ds.Tables[0].Rows)
          list.Add(ConvertDataRowToObj(row));

      return list;
    }
    public CadComissaoFranqueadosModel ConvertDataRowToObj(DataRow dr)
    {
      return new CadComissaoFranqueadosModel
      {
       Id = dr.Field<long>("Id"), 
       Comissao = dr.Field<decimal>("Comissao"), 
       Modo = dr.Field<byte>("Modo"), 
       Nome = dr.Field<string>("Nome") ?? "", 
       Tipo = dr.Field<byte>("Tipo"), 
       Valor = dr.Field<decimal>("Valor")
      };
    }
    public CadComissaoFranqueadosModel SetByDataSet(DataSet ds)
    {
      if (ds?.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        return ConvertDataRowToObj(ds.Tables[0].Rows[0]);
      return new CadComissaoFranqueadosModel(); 
    }

    public async Task<bool> IsExists(long id)
    {
      var item = await GetByIdAsync(id);
      return item.Id > 0;
    }
  }
}