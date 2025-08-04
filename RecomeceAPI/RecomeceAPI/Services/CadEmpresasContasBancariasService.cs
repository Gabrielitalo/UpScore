using RecomeceAPI.Interfaces;
using RecomeceAPI.Models;
using RecomeceAPI.Models.Common;
using RecomeceAPI.Server.Database;
using RecomeceAPI.Services.Common;
using System.Data;

namespace RecomeceAPI.Services
{
    public class CadEmpresasContasBancariasService : IRepository<CadEmpresasContasBancariasModel>
  {
    private readonly DbContext _dbContext;
    private readonly PaginationService _paginationService;
    private readonly GenericModelValidatorService _genericModelValidatorService;

    public CadEmpresasContasBancariasService(DbContext dbContext, 
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
    public void AddEntityParameters(CadEmpresasContasBancariasModel entity)
    {
      AddIdParameter(entity.Id);
      _dbContext.Parametros.AddItem("@p_Agencia", entity.Agencia); 
      _dbContext.Parametros.AddItem("@p_Ativo", entity.Ativo); 
      _dbContext.Parametros.AddItem("@p_ChavePix", entity.ChavePix); 
      _dbContext.Parametros.AddItem("@p_CodBanco", entity.CodBanco); 
      _dbContext.Parametros.AddItem("@p_Conta", entity.Conta); 
      _dbContext.Parametros.AddItem("@p_Fk_CadEmpresas", entity.CadEmpresas.Id); 
      _dbContext.Parametros.AddItem("@p_Padrao", entity.Padrao); 

    }
    public async Task<object> SaveAsync(CadEmpresasContasBancariasModel entity)
    {
      var validation = _genericModelValidatorService.GenericValidator(entity);
      if(validation.Count > 0)
        return validation;

      string cmd = @"Insert Into CadEmpresasContasBancarias (Id, Agencia, Ativo, ChavePix, CodBanco, Conta, Fk_CadEmpresas, Padrao) Select @p_Id, @p_Agencia, @p_Ativo, @p_ChavePix, @p_CodBanco, @p_Conta, @p_Fk_CadEmpresas, @p_Padrao; Select LAST_INSERT_ID();";
      AddEntityParameters(entity);
      return await _dbContext.SaveAsync(cmd, 0);
    }
    public async Task<object> UpdateAsync(CadEmpresasContasBancariasModel entity)
    {
      var validation = _genericModelValidatorService.GenericValidator(entity);
      if (validation.Count > 0)
        return validation;

      if (await IsExists(entity.Id) == false)
        return NotificationService.NoExists();

      string cmd = @"Update CadEmpresasContasBancarias
      Set 
      Agencia = @p_Agencia, 
      Ativo = @p_Ativo, 
      ChavePix = @p_ChavePix, 
      CodBanco = @p_CodBanco, 
      Conta = @p_Conta, 
      Fk_CadEmpresas = @p_Fk_CadEmpresas, 
      Padrao = @p_Padrao
      Where Id = @p_Id;";
      AddEntityParameters(entity);
      await _dbContext.ExecAsync(cmd, 0);
      return entity;
    }
    public async Task<CadEmpresasContasBancariasModel> GetByIdAsync(long id)
    {
      string cmd = @"Select * From CadEmpresasContasBancarias Where Id = @p_Id;";
      AddIdParameter(id);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return SetByDataSet(result);
    }
    public async Task<object> GetById(long id)
    {
      string cmd = @"Select * From CadEmpresasContasBancarias Where Id = @p_Id;";
      AddIdParameter(id);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return result.Tables[0];
    }
    public async Task<PaginationModel> GetAllAsync(int page, int itemsPerPage)
    {
      string cmd = @"Select * From CadEmpresasContasBancarias Limit @p_Pag, @p_rows;";
      _dbContext.Parametros.AddItem("@p_Pag", page);
      _dbContext.Parametros.AddItem("@p_rows", itemsPerPage);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return _paginationService.GetPaginationData(result, page, itemsPerPage);
    }
    public async Task<object> GetAllAsync(long empresaId)
    {
      string cmd = @"Select * From CadEmpresasContasBancarias Where Fk_CadEmpresas = @p_CadEmpresas";
      _dbContext.Parametros.AddItem("@p_CadEmpresas", empresaId);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return result.Tables[0];
    }
    public async Task<object> DeleteAsync(long id)
    {
      if(await IsExists(id) == false)
        return NotificationService.NoExists();
      string cmd = @"Delete From CadEmpresasContasBancarias Where Id = @p_Id;";
      AddIdParameter(id);
      await _dbContext.ExecAsync(cmd, 0);
      return NotificationService.Success();
    }
    public List<CadEmpresasContasBancariasModel> GetListByDataSet(DataSet ds)
    {
      var list = new List<CadEmpresasContasBancariasModel>();

      if (ds?.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        foreach (DataRow row in ds.Tables[0].Rows)
          list.Add(ConvertDataRowToObj(row));

      return list;
    }
    public CadEmpresasContasBancariasModel ConvertDataRowToObj(DataRow dr)
    {
      return new CadEmpresasContasBancariasModel
      {
       Id = dr.Field<long>("Id"), 
       Agencia = dr.Field<string>("Agencia") ?? "", 
       //Ativo = dr.Field<byte>("Ativo"), 
       ChavePix = dr.Field<string>("ChavePix") ?? "", 
       CodBanco = dr.Field<string>("CodBanco") ?? "", 
       Conta = dr.Field<string>("Conta") ?? "", 
       CadEmpresas = new CadEmpresasModel { Id = dr.Field<long>("Fk_CadEmpresas")}, 
       //Padrao = dr.Field<byte>("Padrao")
      };
    }
    public CadEmpresasContasBancariasModel SetByDataSet(DataSet ds)
    {
      if (ds?.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        return ConvertDataRowToObj(ds.Tables[0].Rows[0]);
      return new CadEmpresasContasBancariasModel(); 
    }

    public async Task<bool> IsExists(long id)
    {
      var item = await GetByIdAsync(id);
      return item.Id > 0;
    }
  }
}