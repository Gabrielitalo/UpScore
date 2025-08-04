using RecomeceAPI.Interfaces;
using RecomeceAPI.Models;
using RecomeceAPI.Models.Common;
using RecomeceAPI.Server.Database;
using RecomeceAPI.Services.Common;
using System.Data;

namespace RecomeceAPI.Services
{
    public class CadProdutosFaixasService : IRepository<CadProdutosFaixasModel>
  {
    private readonly DbContext _dbContext;
    private readonly PaginationService _paginationService;
    private readonly GenericModelValidatorService _genericModelValidatorService;

    public CadProdutosFaixasService(DbContext dbContext, 
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
    public void AddEntityParameters(CadProdutosFaixasModel entity)
    {
      AddIdParameter(entity.Id);
      _dbContext.Parametros.AddItem("@p_Fk_CadProdutos", entity.CadProdutos.Id); 
      _dbContext.Parametros.AddItem("@p_ValorDivida", entity.ValorDivida); 
      _dbContext.Parametros.AddItem("@p_ValorEntradaMinima", entity.ValorEntradaMinima); 
      _dbContext.Parametros.AddItem("@p_ValorServico", entity.ValorServico); 

    }
    public async Task<object> SaveAsync(CadProdutosFaixasModel entity)
    {
      var validation = _genericModelValidatorService.GenericValidator(entity);
      if(validation.Count > 0)
        return validation;

      string cmd = @"Insert Into CadProdutosFaixas (Id, Fk_CadProdutos, ValorDivida, ValorEntradaMinima, ValorServico) Select @p_Id, @p_Fk_CadProdutos, @p_ValorDivida, @p_ValorEntradaMinima, @p_ValorServico; Select LAST_INSERT_ID();";
      AddEntityParameters(entity);
      return await _dbContext.SaveAsync(cmd, 0);
    }
    public async Task<object> UpdateAsync(CadProdutosFaixasModel entity)
    {
      var validation = _genericModelValidatorService.GenericValidator(entity);
      if (validation.Count > 0)
        return validation;

      if (await IsExists(entity.Id) == false)
        return NotificationService.NoExists();

      string cmd = @"Update CadProdutosFaixas
      Set 
      Id = @p_Id, 
      ValorDivida = @p_ValorDivida, 
      ValorEntradaMinima = @p_ValorEntradaMinima, 
      ValorServico = @p_ValorServico
      Where Id = @p_Id;";
      AddEntityParameters(entity);
      await _dbContext.ExecAsync(cmd, 0);
      return entity;
    }
    public async Task<CadProdutosFaixasModel> GetByIdAsync(long id)
    {
      string cmd = @"Select * From CadProdutosFaixas Where Id = @p_Id;";
      AddIdParameter(id);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return SetByDataSet(result);
    }
    public async Task<CadProdutosFaixasModel> GetProductDebit(long productId, decimal value)
    {
      string cmd = @"Select *
      From CadProdutosFaixas
      Where Fk_CadProdutos = @p_CadProdutos
      AND ValorDivida >= @p_ValorDivida
      ORDER BY ValorDivida ASC
      LIMIT 1;";
      _dbContext.Parametros.AddItem("@p_CadProdutos", productId);
      _dbContext.Parametros.AddItem("@p_ValorDivida", value);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return SetByDataSet(result);
    }
    public async Task<object> GetProductRangeDebit(long productId, decimal value)
    {
      string cmd = @"SELECT 
      Cf.Id,
      Cf.Fk_CadProdutos,
      C.Descricao,
      Cf.ValorDivida, 
      Cf.ValorServico, 
      Cf.ValorEntradaMinima, 
      JSON_ARRAYAGG(
        JSON_OBJECT(
          'Id', Cfp.Id,
          'Tipo', Cfp.Tipo,
          'Modo', Cfp.Modo,
          'ValorMaxParcelas', Cfp.ValorMaxParcelas,
          'Descricao', Cfp.Descricao
        )
      ) AS FormasPagamento
    FROM CadProdutosFaixas Cf
    JOIN CadProdutos C ON C.Id = Cf.Fk_CadProdutos
    JOIN CadProdutosFaixasPagamento Cfp ON Cfp.Fk_CadProdutosFaixas = Cf.Id
    WHERE Cf.Fk_CadProdutos = @p_CadProdutos
    AND Cf.ValorDivida >= @p_ValorDivida
    GROUP BY 
      Cf.Id,
      Cf.Fk_CadProdutos,
      C.Descricao,
      Cf.ValorDivida, 
      Cf.ValorServico, 
      Cf.ValorEntradaMinima
      ORDER BY ValorDivida ASC
      LIMIT 1;";
      _dbContext.Parametros.AddItem("@p_CadProdutos", productId);
      _dbContext.Parametros.AddItem("@p_ValorDivida", value);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return result.Tables[0];
    }
    public async Task<PaginationModel> GetAllAsync(int page, int itemsPerPage)
    {
      string cmd = @"Select * From CadProdutosFaixas Limit @p_Pag, @p_rows;";
      _dbContext.Parametros.AddItem("@p_Pag", page);
      _dbContext.Parametros.AddItem("@p_rows", itemsPerPage);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return _paginationService.GetPaginationData(result, page, itemsPerPage);
    }
    public async Task<object> DeleteAsync(long id)
    {
      if(await IsExists(id) == false)
        return NotificationService.NoExists();
      string cmd = @"Delete From CadProdutosFaixas Where Id = @p_Id;";
      AddIdParameter(id);
      await _dbContext.ExecAsync(cmd, 0);
      return NotificationService.Success();
    }
    public List<CadProdutosFaixasModel> GetListByDataSet(DataSet ds)
    {
      var list = new List<CadProdutosFaixasModel>();

      if (ds?.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        foreach (DataRow row in ds.Tables[0].Rows)
          list.Add(ConvertDataRowToObj(row));

      return list;
    }
    public CadProdutosFaixasModel ConvertDataRowToObj(DataRow dr)
    {
      return new CadProdutosFaixasModel
      {
       Id = dr.Field<long>("Id"), 
       CadProdutos = new CadProdutosModel { Id = dr.Field<long>("Fk_CadProdutos")}, 
       ValorDivida = dr.Field<decimal>("ValorDivida"), 
       ValorEntradaMinima = dr.Field<decimal>("ValorEntradaMinima"), 
       ValorServico = dr.Field<decimal>("ValorServico")
      };
    }
    public CadProdutosFaixasModel SetByDataSet(DataSet ds)
    {
      if (ds?.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        return ConvertDataRowToObj(ds.Tables[0].Rows[0]);
      return new CadProdutosFaixasModel(); 
    }

    public async Task<bool> IsExists(long id)
    {
      var item = await GetByIdAsync(id);
      return item.Id > 0;
    }
  }
}