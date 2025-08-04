using RecomeceAPI.Interfaces;
using RecomeceAPI.Models;
using RecomeceAPI.Models.Common;
using RecomeceAPI.Server.Database;
using RecomeceAPI.Services.Common;
using System.Data;

namespace RecomeceAPI.Services
{
  public class CidadesService : IRepository<CidadesModel>
  {
    private readonly DbContext _dbContext;
    private readonly PaginationService _paginationService;
    private readonly GenericModelValidatorService _genericModelValidatorService;

    public CidadesService(DbContext dbContext,
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
    public void AddEntityParameters(CidadesModel entity)
    {
      AddIdParameter(entity.Id);
      _dbContext.Parametros.AddItem("@p_CodigoIBGE", entity.CodigoIBGE);
      _dbContext.Parametros.AddItem("@p_Fk_Estados", entity.Estados.Id);
      _dbContext.Parametros.AddItem("@p_NomeCidade", entity.NomeCidade);

    }
    public async Task<object> SaveAsync(CidadesModel entity)
    {
      var validation = _genericModelValidatorService.GenericValidator(entity);
      if (validation.Count > 0)
        return validation;

      string cmd = @"Insert Into Cidades (Id, CodigoIBGE, Fk_Estados, NomeCidade) Select @p_Id, @p_CodigoIBGE, @p_Fk_Estados, @p_NomeCidade; Select LAST_INSERT_ID();";
      AddEntityParameters(entity);
      return await _dbContext.SaveAsync(cmd, 0);
    }
    public async Task<object> UpdateAsync(CidadesModel entity)
    {
      var validation = _genericModelValidatorService.GenericValidator(entity);
      if (validation.Count > 0)
        return validation;

      if (await IsExists(entity.Id) == false)
        return NotificationService.NoExists();

      string cmd = @"Update Cidades
      Set 
      Id = @p_Id, 
      CodigoIBGE = @p_CodigoIBGE, 
      Fk_Estados = @p_Fk_Estados, 
      NomeCidade = @p_NomeCidade
      Where Id = @p_Id;";
      AddEntityParameters(entity);
      await _dbContext.ExecAsync(cmd, 0);
      return entity;
    }
    public async Task<CidadesModel> GetByIdAsync(long id)
    {
      string cmd = @"Select * From Cidades Where Id = @p_Id;";
      AddIdParameter(id);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return SetByDataSet(result);
    }
    public async Task<CidadesModel> GetByIbgeAsync(string codigoIBGE)
    {
      string cmd = @"Select * From Cidades Where CodigoIBGE = @p_CodigoIBGE;";
      _dbContext.Parametros.AddItem("@p_CodigoIBGE", codigoIBGE);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return SetByDataSet(result);
    }
    public async Task<PaginationModel> GetAllAsync(int page, int itemsPerPage)
    {
      string cmd = @"Select * From Cidades Limit @p_Pag, @p_rows;";
      _dbContext.Parametros.AddItem("@p_Pag", ExtensionService.OffsetCalculator(page, itemsPerPage));
      _dbContext.Parametros.AddItem("@p_rows", itemsPerPage);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return _paginationService.GetPaginationData(result, page, itemsPerPage);
    }
    public async Task<object> DeleteAsync(long id)
    {
      if (await IsExists(id) == false)
        return NotificationService.NoExists();
      string cmd = @"Delete From Cidades Where Id = @p_Id;";
      AddIdParameter(id);
      await _dbContext.ExecAsync(cmd, 0);
      return NotificationService.Success();
    }
    public List<CidadesModel> GetListByDataSet(DataSet ds)
    {
      var list = new List<CidadesModel>();

      if (ds?.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        foreach (DataRow row in ds.Tables[0].Rows)
          list.Add(ConvertDataRowToObj(row));

      return list;
    }
    public CidadesModel ConvertDataRowToObj(DataRow dr)
    {
      return new CidadesModel
      {
        Id = dr.Field<long>("Id"),
        CodigoIBGE = dr.Field<int>("CodigoIBGE"),
        Estados = new EstadosModel { Id = dr.Field<long>("Fk_Estados") },
        NomeCidade = dr.Field<string>("NomeCidade") ?? ""
      };
    }
    public CidadesModel SetByDataSet(DataSet ds)
    {
      if (ds?.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        return ConvertDataRowToObj(ds.Tables[0].Rows[0]);
      return new CidadesModel();
    }

    public async Task<bool> IsExists(long id)
    {
      var item = await GetByIdAsync(id);
      return item.Id > 0;
    }
    public async Task<object> Autocomplete(string texto)
    {
      string cmd = @"Select C.Id, Concat(C.NomeCidade, ' - ', E.UF) Texto
      From Cidades C 
      Join Estados E on (E.Id = C.FK_Estados) Where C.NomeCidade like @p_texto Limit 10;";
      _dbContext.Parametros.AddItem("@p_texto", $"%{texto}%", 1);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return result.Tables[0];
    }
  }
}