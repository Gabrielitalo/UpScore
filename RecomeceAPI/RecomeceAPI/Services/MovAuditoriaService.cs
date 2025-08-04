using RecomeceAPI.Interfaces;
using RecomeceAPI.Models;
using RecomeceAPI.Models.Common;
using RecomeceAPI.Server.Auth;
using RecomeceAPI.Server.Database;
using RecomeceAPI.Services.Common;
using System.Data;

namespace RecomeceAPI.Services
{
  public class MovAuditoriaService : IRepository<MovAuditoriaModel>
  {
    private readonly DbContext _dbContext;
    private readonly PaginationService _paginationService;
    private readonly GenericModelValidatorService _genericModelValidatorService;

    public MovAuditoriaService(DbContext dbContext,
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
    public void AddEntityParameters(MovAuditoriaModel entity)
    {
      AddIdParameter(entity.Id);
      _dbContext.Parametros.AddItem("@p_DataInicial", entity.DataInicial);
      _dbContext.Parametros.AddItem("@p_IdResponsavel", entity.IdResponsavel);
      _dbContext.Parametros.AddItem("@p_IdTabela", entity.IdTabela);
      _dbContext.Parametros.AddItem("@p_NomeTabela", entity.NomeTabela);
      _dbContext.Parametros.AddItem("@p_Observacao", entity.Observacao);
      _dbContext.Parametros.AddItem("@p_Texto", entity.Texto);
      _dbContext.Parametros.AddItem("@p_TipoAcao", entity.TipoAcao);
      _dbContext.Parametros.AddItem("@p_TipoResponsavel", entity.TipoResponsavel);

    }
    public async Task<object> SaveAsync(MovAuditoriaModel entity)
    {
      var validation = _genericModelValidatorService.GenericValidator(entity);
      if (validation.Count > 0)
        return validation;

      string cmd = @"Insert Into MovAuditoria (Id, DataInicial, IdResponsavel, IdTabela, NomeTabela, Observacao, Texto, TipoAcao, TipoResponsavel) Select @p_Id, @p_DataInicial, @p_IdResponsavel, @p_IdTabela, @p_NomeTabela, @p_Observacao, @p_Texto, @p_TipoAcao, @p_TipoResponsavel; Select LAST_INSERT_ID();";
      AddEntityParameters(entity);
      return await _dbContext.SaveAsync(cmd, 0);
    }
    public async Task<object> UpdateAsync(MovAuditoriaModel entity)
    {
      var validation = _genericModelValidatorService.GenericValidator(entity);
      if (validation.Count > 0)
        return validation;

      if (await IsExists(entity.Id) == false)
        return NotificationService.NoExists();

      string cmd = @"Update MovAuditoria
      Set 
      Id = @p_Id, 
      DataInicial = @p_DataInicial, 
      IdResponsavel = @p_IdResponsavel, 
      IdTabela = @p_IdTabela, 
      NomeTabela = @p_NomeTabela, 
      Observacao = @p_Observacao, 
      Texto = @p_Texto, 
      TipoAcao = @p_TipoAcao, 
      TipoResponsavel = @p_TipoResponsavel
      Where Id = @p_Id;";
      AddEntityParameters(entity);
      await _dbContext.ExecAsync(cmd, 0);
      return entity;
    }
    public async Task<MovAuditoriaModel> GetByIdAsync(long id)
    {
      string cmd = @"Select * From MovAuditoria Where Id = @p_Id;";
      AddIdParameter(id);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return SetByDataSet(result);
    }
    public async Task<PaginationModel> GetAllAsync(int page, int itemsPerPage)
    {
      string cmd = @"Select * From MovAuditoria Limit @p_Pag, @p_rows;";
      _dbContext.Parametros.AddItem("@p_Pag", page);
      _dbContext.Parametros.AddItem("@p_rows", itemsPerPage);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return _paginationService.GetPaginationData(result, page, itemsPerPage);
    }
    public async Task<object> DeleteAsync(long id)
    {
      if (await IsExists(id) == false)
        return NotificationService.NoExists();
      string cmd = @"Delete From MovAuditoria Where Id = @p_Id;";
      AddIdParameter(id);
      await _dbContext.ExecAsync(cmd, 0);
      return NotificationService.Success();
    }
    public List<MovAuditoriaModel> GetListByDataSet(DataSet ds)
    {
      var list = new List<MovAuditoriaModel>();

      if (ds?.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        foreach (DataRow row in ds.Tables[0].Rows)
          list.Add(ConvertDataRowToObj(row));

      return list;
    }
    public MovAuditoriaModel ConvertDataRowToObj(DataRow dr)
    {
      return new MovAuditoriaModel
      {
        Id = dr.Field<long>("Id"),
        DataInicial = dr.Field<DateTime>("DataInicial"),
        IdResponsavel = dr.Field<long>("IdResponsavel"),
        IdTabela = dr.Field<long>("IdTabela"),
        NomeTabela = dr.Field<string>("NomeTabela") ?? "",
        Observacao = dr.Field<string>("Observacao") ?? "",
        Texto = dr.Field<string>("Texto") ?? "",
        TipoAcao = dr.Field<byte>("TipoAcao"),
        TipoResponsavel = dr.Field<byte>("TipoResponsavel")
      };
    }
    public MovAuditoriaModel SetByDataSet(DataSet ds)
    {
      if (ds?.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        return ConvertDataRowToObj(ds.Tables[0].Rows[0]);
      return new MovAuditoriaModel();
    }

    public async Task<bool> IsExists(long id)
    {
      var item = await GetByIdAsync(id);
      return item.Id > 0;
    }
    public async Task PrepareSave(byte action, string tableName, long tableId, string text)
    {
      var audit = new MovAuditoriaModel
      {
        Id = 0,
        DataInicial = DateTime.Now,
        IdResponsavel = Convert.ToInt64(AppIdentity.GetClaimValue("IdUser")),
        TipoResponsavel = Convert.ToByte(AppIdentity.GetClaimValue("IdType")),
        IdTabela = tableId,
        NomeTabela = tableName,
        Observacao = "",
        Texto = text,
        TipoAcao = action,
      };

      await SaveAsync(audit); 
    }
  }
}