using DocumentFormat.OpenXml.Office2010.Excel;
using RecomeceAPI.Interfaces;
using RecomeceAPI.Models;
using RecomeceAPI.Models.Common;
using RecomeceAPI.Server.Auth;
using RecomeceAPI.Server.Database;
using RecomeceAPI.Services.Common;
using System.Data;

namespace RecomeceAPI.Services
{
  public class CadProdutosService : IRepository<CadProdutosModel>
  {
    private readonly DbContext _dbContext;
    private readonly PaginationService _paginationService;
    private readonly GenericModelValidatorService _genericModelValidatorService;

    public CadProdutosService(DbContext dbContext,
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
    public void AddEntityParameters(CadProdutosModel entity)
    {
      AddIdParameter(entity.Id);
      _dbContext.Parametros.AddItem("@p_Codigo", entity.Codigo);
      _dbContext.Parametros.AddItem("@p_Custo", entity.Custo);
      _dbContext.Parametros.AddItem("@p_DataFinal", entity.DataFinal);
      _dbContext.Parametros.AddItem("@p_DataInicial", entity.DataInicial);
      _dbContext.Parametros.AddItem("@p_Descricao", entity.Descricao);
      _dbContext.Parametros.AddItem("@p_Situacao", entity.Situacao);
      _dbContext.Parametros.AddItem("@p_LimpaNome", entity.LimpaNome);
      _dbContext.Parametros.AddItem("@p_CadEmpresas", entity.CadEmpresas.Id);
      _dbContext.Parametros.AddItem("@p_TipoConsulta", entity.TipoConsulta);

    }
    public async Task<object> SaveAsync(CadProdutosModel entity)
    {
      entity.CadEmpresas = new CadEmpresasModel();
      entity.CadEmpresas.Id = Convert.ToInt64(AppIdentity.GetClaimValue("IdCompany"));
      var validation = _genericModelValidatorService.GenericValidator(entity);
      if (validation.Count > 0)
        return validation;

      string cmd = @"Insert Into CadProdutos (Id, Codigo, Custo, DataFinal, DataInicial, Descricao, Situacao, Fk_CadEmpresas, LimpaNome, TipoConsulta) 
      Select @p_Id, @p_Codigo, @p_Custo, @p_DataFinal, @p_DataInicial, @p_Descricao, @p_Situacao, @p_CadEmpresas, @p_LimpaNome, @p_TipoConsulta; 
      Select LAST_INSERT_ID();";
      AddEntityParameters(entity);
      return await _dbContext.SaveAsync(cmd, 0);
    }
    public async Task<object> UpdateAsync(CadProdutosModel entity)
    {
      var validation = _genericModelValidatorService.GenericValidator(entity);
      if (validation.Count > 0)
        return validation;

      if (await IsExists(entity.Id) == false)
        return NotificationService.NoExists();

      string cmd = @"Update CadProdutos
      Set 
      Id = @p_Id, 
      Codigo = @p_Codigo, 
      LimpaNome = @p_LimpaNome,
      Custo = @p_Custo, 
      DataFinal = @p_DataFinal, 
      DataInicial = @p_DataInicial, 
      Descricao = @p_Descricao, 
      Situacao = @p_Situacao,
      TipoConsulta = @p_TipoConsulta
      Where Id = @p_Id;";
      AddEntityParameters(entity);
      await _dbContext.ExecAsync(cmd, 0);
      return entity;
    }
    public async Task<CadProdutosModel> GetByIdAsync(long id)
    {
      string cmd = @"Select * From CadProdutos Where Id = @p_Id;";
      AddIdParameter(id);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return SetByDataSet(result);
    }
    public async Task<object> GelAllActive()
    {
      long idCompany = Convert.ToInt64(AppIdentity.GetClaimValue("IdCompany"));
      string cmd = @"Select Id, Codigo, Descricao, LimpaNome From CadProdutos Where Situacao = 1 and Fk_CadEmpresas = @p_CadEmpresas;";
      _dbContext.Parametros.AddItem("@p_CadEmpresas", idCompany);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return result.Tables[0];
    }
    public async Task<object> GelAllRanges(long id)
    {
      string cmd = @"SELECT 
                      JSON_OBJECT(
                          'id', cp.Id,
                          'situacao', cp.Situacao,
                          'dataInicial', cp.DataInicial,
                          'dataFinal', cp.DataFinal,
                          'codigo', cp.Codigo,
                          'descricao', cp.Descricao,
                          'custo', cp.Custo,
                          'fkCadEmpresas', cp.Fk_CadEmpresas,
                          'limpaNome', cp.LimpaNome,
                          'tipoConsulta', cp.TipoConsulta,
                          'faixas', (
                              SELECT JSON_ARRAYAGG(
                                  JSON_OBJECT(
                                      'id', cpf.Id,
                                      'valorDivida', cpf.ValorDivida,
                                      'valorServico', cpf.ValorServico,
                                      'valorEntradaMinima', cpf.ValorEntradaMinima,
                                      'pagamentos', (
                                          SELECT JSON_ARRAYAGG(
                                              JSON_OBJECT(
                                                  'id', cpfp.Id,
                                                  'tipo', cpfp.Tipo,
                                                  'modo', cpfp.Modo,
                                                  'valorMaxParcelas', cpfp.ValorMaxParcelas,
                                                  'descricao', cpfp.Descricao
                                              )
                                          )
                                          FROM CadProdutosFaixasPagamento cpfp
                                          WHERE cpfp.Fk_CadProdutosFaixas = cpf.Id
                                      )
                                  )
                              )
                              FROM CadProdutosFaixas cpf
                              WHERE cpf.Fk_CadProdutos = cp.Id
                          )
                      ) AS produtoCompleto
                  FROM CadProdutos cp
                  WHERE cp.Id = @p_Id;
";
      _dbContext.Parametros.AddItem("@p_Id", id);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return result.Tables[0]?.Rows[0]["ProdutoCompleto"] ?? new object();
    }
    public async Task<PaginationModel> GetAllAsync(int page, int itemsPerPage)
    {
      long idCompany = Convert.ToInt64(AppIdentity.GetClaimValue("IdCompany"));
      string cmd = @"Select * From CadProdutos Where Fk_CadEmpresas = @p_CadEmpresas Limit @p_Pag, @p_rows;";
      _dbContext.Parametros.AddItem("@p_Pag", ExtensionService.OffsetCalculator(page, itemsPerPage));
      _dbContext.Parametros.AddItem("@p_rows", itemsPerPage);
      _dbContext.Parametros.AddItem("@p_CadEmpresas", idCompany);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return _paginationService.GetPaginationData(result, page, itemsPerPage);
    }
    public async Task<object> DeleteAsync(long id)
    {
      if (await IsExists(id) == false)
        return NotificationService.NoExists();
      string cmd = @"Delete From CadProdutos Where Id = @p_Id;";
      AddIdParameter(id);
      await _dbContext.ExecAsync(cmd, 0);
      return NotificationService.Success();
    }
    public List<CadProdutosModel> GetListByDataSet(DataSet ds)
    {
      var list = new List<CadProdutosModel>();

      if (ds?.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        foreach (DataRow row in ds.Tables[0].Rows)
          list.Add(ConvertDataRowToObj(row));

      return list;
    }
    public CadProdutosModel ConvertDataRowToObj(DataRow dr)
    {
      return new CadProdutosModel
      {
        Id = dr.Field<long>("Id"),
        Codigo = dr.Field<string>("Codigo") ?? "",
        Custo = dr.Field<decimal>("Custo"),
        CadEmpresas = new CadEmpresasModel { Id = dr.Field<long>("Fk_CadEmpresas") },
        DataFinal = ExtensionService.SafeConvert<DateTime>(dr["DataFinal"]),
        DataInicial = ExtensionService.SafeConvert<DateTime>(dr["DataInicial"]),
        Descricao = dr.Field<string>("Descricao") ?? "",
        Situacao = ExtensionService.SafeConvert<byte>(dr["Situacao"]),
        LimpaNome = dr.Field<int>("LimpaNome"),
        TipoConsulta = dr.Field<int>("TipoConsulta"),
      };
    }
    public CadProdutosModel SetByDataSet(DataSet ds)
    {
      if (ds?.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        return ConvertDataRowToObj(ds.Tables[0].Rows[0]);
      return new CadProdutosModel();
    }

    public async Task<bool> IsExists(long id)
    {
      var item = await GetByIdAsync(id);
      return item.Id > 0;
    }
  }
}