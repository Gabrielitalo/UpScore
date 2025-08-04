using RecomeceAPI.Interfaces;
using RecomeceAPI.Models;
using RecomeceAPI.Models.Common;
using RecomeceAPI.Server.Database;
using RecomeceAPI.Services.Common;
using System.Data;

namespace RecomeceAPI.Services
{
    public class CadComissaoVendedoresService : IRepository<CadComissaoVendedoresModel>
  {
    private readonly DbContext _dbContext;
    private readonly PaginationService _paginationService;
    private readonly GenericModelValidatorService _genericModelValidatorService;

    public CadComissaoVendedoresService(DbContext dbContext, 
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
    public void AddEntityParameters(CadComissaoVendedoresModel entity)
    {
      AddIdParameter(entity.Id);
      _dbContext.Parametros.AddItem("@p_Ativo", entity.Ativo); 
      _dbContext.Parametros.AddItem("@p_Fk_CadComissao", entity.CadComissao.Id); 
      _dbContext.Parametros.AddItem("@p_Fk_CadEquipe", entity.CadEquipe.Id); 

    }
    public async Task<object> SaveAsync(CadComissaoVendedoresModel entity)
    {
      var validation = _genericModelValidatorService.GenericValidator(entity);
      if(validation.Count > 0)
        return validation;

      string cmd = @"Insert Into CadComissaoVendedores (Id, Ativo, Fk_CadComissao, Fk_CadEquipe) Select @p_Id, @p_Ativo, @p_Fk_CadComissao, @p_Fk_CadEquipe; Select LAST_INSERT_ID();";
      AddEntityParameters(entity);
      return await _dbContext.SaveAsync(cmd, 0);
    }
    public async Task<object> UpdateAsync(CadComissaoVendedoresModel entity)
    {
      var validation = _genericModelValidatorService.GenericValidator(entity);
      if (validation.Count > 0)
        return validation;

      if (await IsExists(entity.Id) == false)
        return NotificationService.NoExists();

      string cmd = @"Update CadComissaoVendedores
      Set 
      Id = @p_Id, 
      Ativo = @p_Ativo, 
      Fk_CadComissao = @p_Fk_CadComissao, 
      Fk_CadEquipe = @p_Fk_CadEquipe
      Where Id = @p_Id;";
      AddEntityParameters(entity);
      await _dbContext.ExecAsync(cmd, 0);
      return entity;
    }
    public async Task<CadComissaoVendedoresModel> GetByIdAsync(long id)
    {
      string cmd = @"Select * From CadComissaoVendedores Where Id = @p_Id;";
      AddIdParameter(id);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return SetByDataSet(result);
    }
    public async Task<PaginationModel> GetAllAsync(int page, int itemsPerPage)
    {
      string cmd = @"Select * From CadComissaoVendedores Limit @p_Pag, @p_rows;";
      _dbContext.Parametros.AddItem("@p_Pag", page);
      _dbContext.Parametros.AddItem("@p_rows", itemsPerPage);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return _paginationService.GetPaginationData(result, page, itemsPerPage);
    }
    public async Task<object> DeleteAsync(long id)
    {
      if(await IsExists(id) == false)
        return NotificationService.NoExists();
      string cmd = @"Delete From CadComissaoVendedores Where Id = @p_Id;";
      AddIdParameter(id);
      await _dbContext.ExecAsync(cmd, 0);
      return NotificationService.Success();
    }
    public List<CadComissaoVendedoresModel> GetListByDataSet(DataSet ds)
    {
      var list = new List<CadComissaoVendedoresModel>();

      if (ds?.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        foreach (DataRow row in ds.Tables[0].Rows)
          list.Add(ConvertDataRowToObj(row));

      return list;
    }
    public CadComissaoVendedoresModel ConvertDataRowToObj(DataRow dr)
    {
      return new CadComissaoVendedoresModel
      {
       Id = dr.Field<long>("Id"), 
       Ativo = dr.Field<byte>("Ativo"), 
       CadComissao = new CadComissaoModel { Id = dr.Field<long>("Fk_CadComissao")}, 
       CadEquipe = new CadEquipeModel { Id = dr.Field<long>("Fk_CadEquipe")}
      };
    }
    public CadComissaoVendedoresModel SetByDataSet(DataSet ds)
    {
      if (ds?.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        return ConvertDataRowToObj(ds.Tables[0].Rows[0]);
      return new CadComissaoVendedoresModel(); 
    }

    public async Task<bool> IsExists(long id)
    {
      var item = await GetByIdAsync(id);
      return item.Id > 0;
    }
  }
}