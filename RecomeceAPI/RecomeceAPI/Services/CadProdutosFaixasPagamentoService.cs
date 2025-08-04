using RecomeceAPI.Interfaces;
using RecomeceAPI.Models;
using RecomeceAPI.Models.Common;
using RecomeceAPI.Server.Database;
using RecomeceAPI.Services.Common;
using System.Data;

namespace RecomeceAPI.Services
{
  public class CadProdutosFaixasPagamentoService : IRepository<CadProdutosFaixasPagamentoModel>
  {
    private readonly DbContext _dbContext;
    private readonly PaginationService _paginationService;
    private readonly GenericModelValidatorService _genericModelValidatorService;

    public CadProdutosFaixasPagamentoService(DbContext dbContext,
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
    public void AddEntityParameters(CadProdutosFaixasPagamentoModel entity)
    {
      AddIdParameter(entity.Id);
      _dbContext.Parametros.AddItem("@p_Descricao", entity.Descricao);
      _dbContext.Parametros.AddItem("@p_Fk_CadProdutosFaixas", entity.CadProdutosFaixas.Id);
      _dbContext.Parametros.AddItem("@p_Modo", entity.Modo);
      _dbContext.Parametros.AddItem("@p_Tipo", entity.Tipo);
      _dbContext.Parametros.AddItem("@p_ValorMaxParcelas", entity.ValorMaxParcelas);

    }
    public async Task<object> SaveAsync(CadProdutosFaixasPagamentoModel entity)
    {
      var validation = _genericModelValidatorService.GenericValidator(entity);
      if (validation.Count > 0)
        return validation;

      string cmd = @"Insert Into CadProdutosFaixasPagamento (Id, Descricao, Fk_CadProdutosFaixas, Modo, Tipo, ValorMaxParcelas) Select @p_Id, @p_Descricao, @p_Fk_CadProdutosFaixas, @p_Modo, @p_Tipo, @p_ValorMaxParcelas; Select LAST_INSERT_ID();";
      AddEntityParameters(entity);
      return await _dbContext.SaveAsync(cmd, 0);
    }
    public async Task<object> UpdateAsync(CadProdutosFaixasPagamentoModel entity)
    {
      var validation = _genericModelValidatorService.GenericValidator(entity);
      if (validation.Count > 0)
        return validation;

      if (await IsExists(entity.Id) == false)
        return NotificationService.NoExists();

      string cmd = @"Update CadProdutosFaixasPagamento
      Set 
      Id = @p_Id, 
      Descricao = @p_Descricao, 
      Modo = @p_Modo, 
      Tipo = @p_Tipo, 
      ValorMaxParcelas = @p_ValorMaxParcelas
      Where Id = @p_Id;";
      AddEntityParameters(entity);
      await _dbContext.ExecAsync(cmd, 0);
      return entity;
    }
    public async Task<CadProdutosFaixasPagamentoModel> GetByIdAsync(long id)
    {
      string cmd = @"Select * From CadProdutosFaixasPagamento Where Id = @p_Id;";
      AddIdParameter(id);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return SetByDataSet(result);
    }
    public async Task<List<CadProdutosFaixasPagamentoModel>> GetRangeByProduct(long cadProdutosFaixas)
    {
      string cmd = @"Select * From CadProdutosFaixasPagamento Where Fk_CadProdutosFaixas = @p_CadProdutosFaixas;";
      _dbContext.Parametros.AddItem("@p_CadProdutosFaixas", cadProdutosFaixas);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return GetListByDataSet(result);
    }
    public async Task<PaginationModel> GetAllAsync(int page, int itemsPerPage)
    {
      string cmd = @"Select * From CadProdutosFaixasPagamento Limit @p_Pag, @p_rows;";
      _dbContext.Parametros.AddItem("@p_Pag", page);
      _dbContext.Parametros.AddItem("@p_rows", itemsPerPage);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return _paginationService.GetPaginationData(result, page, itemsPerPage);
    }
    public async Task<object> DeleteAsync(long id)
    {
      if (await IsExists(id) == false)
        return NotificationService.NoExists();
      string cmd = @"Delete From CadProdutosFaixasPagamento Where Id = @p_Id;";
      AddIdParameter(id);
      await _dbContext.ExecAsync(cmd, 0);
      return NotificationService.Success();
    }
    public List<CadProdutosFaixasPagamentoModel> GetListByDataSet(DataSet ds)
    {
      var list = new List<CadProdutosFaixasPagamentoModel>();

      if (ds?.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        foreach (DataRow row in ds.Tables[0].Rows)
          list.Add(ConvertDataRowToObj(row));

      return list;
    }
    public CadProdutosFaixasPagamentoModel ConvertDataRowToObj(DataRow dr)
    {
      return new CadProdutosFaixasPagamentoModel
      {
        Id = dr.Field<long>("Id"),
        Descricao = dr.Field<string>("Descricao") ?? "",
        CadProdutosFaixas = new CadProdutosFaixasModel { Id = dr.Field<long>("Fk_CadProdutosFaixas") },
        Modo = ExtensionService.SafeConvert<byte>(dr["Modo"]),
        Tipo = ExtensionService.SafeConvert<byte>(dr["Tipo"]),
        ValorMaxParcelas = ExtensionService.SafeConvert<byte>(dr["ValorMaxParcelas"])
      };
    }
    public CadProdutosFaixasPagamentoModel SetByDataSet(DataSet ds)
    {
      if (ds?.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        return ConvertDataRowToObj(ds.Tables[0].Rows[0]);
      return new CadProdutosFaixasPagamentoModel();
    }

    public async Task<bool> IsExists(long id)
    {
      var item = await GetByIdAsync(id);
      return item.Id > 0;
    }
    public async Task ProcessNewRange(long rangeId, List<CadProdutosFaixasPagamentoModel> ranges)
    {
      foreach (var range in ranges)
      {
        range.CadProdutosFaixas.Id = rangeId;
        await SaveAsync(range);
      }
    }
  }
}