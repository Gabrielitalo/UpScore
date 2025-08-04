using RecomeceAPI.Interfaces;
using RecomeceAPI.Models.Common;
using RecomeceAPI.Models;
using RecomeceAPI.Server.Database;
using RecomeceAPI.Services.Common;
using System.Data;
using RecomeceAPI.Server.Auth;

public class MovContratosService : IRepository<MovContratosModel>
{
  private readonly DbContext _dbContext;
  private readonly PaginationService _paginationService;
  private readonly GenericModelValidatorService _genericModelValidatorService;

  public MovContratosService(DbContext dbContext,
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

  public void AddEntityParameters(MovContratosModel entity)
  {
    AddIdParameter(entity.Id);
    _dbContext.Parametros.AddItem("@p_MovPropostas", entity.MovPropostasModel.Id);
    _dbContext.Parametros.AddItem("@p_Situacao", entity.Situacao);
    _dbContext.Parametros.AddItem("@p_DataCriacao", entity.DataCriacao);
    _dbContext.Parametros.AddItem("@p_DataFechamento", entity.DataFechamento);
  }

  public async Task NewFromProposal(long proposalId)
  {
    var entity = new MovContratosModel { MovPropostasModel = new MovPropostasModel { Id = proposalId }, DataCriacao = DateTime.Now, Situacao = 1 };
    await SaveAsync(entity);
  }
  public async Task<object> SaveAsync(MovContratosModel entity)
  {
    var validation = _genericModelValidatorService.GenericValidator(entity);
    if (validation.Count > 0)
      return validation;

    string cmd = @"Insert Into MovContratos (Fk_MovPropostas, Situacao, DataCriacao, DataFechamento)
                   Select @p_MovPropostas, @p_Situacao, @p_DataCriacao, @p_DataFechamento;
                   Select LAST_INSERT_ID();";
    AddEntityParameters(entity);
    return await _dbContext.SaveAsync(cmd, 0);
  }

  public async Task<object> UpdateAsync(MovContratosModel entity)
  {
    var validation = _genericModelValidatorService.GenericValidator(entity);
    if (validation.Count > 0)
      return validation;

    if (await IsExists(entity.Id) == false)
      return NotificationService.NoExists();

    string cmd = @"Update MovContratos
                   Set 
                     Fk_MovPropostas = @p_MovPropostas,
                     Situacao = @p_Situacao,
                     DataCriacao = @p_DataCriacao,
                     DataFechamento = @p_DataFechamento
                   Where Id = @p_Id;";
    AddEntityParameters(entity);
    await _dbContext.ExecAsync(cmd, 0);
    return entity;
  }

  public async Task<MovContratosModel> GetByIdAsync(long id)
  {
    string cmd = @"Select * From MovContratos Where Id = @p_Id;";
    AddIdParameter(id);
    DataSet result = await _dbContext.QueryAsync(cmd, 0);
    return SetByDataSet(result);
  }

  public async Task<PaginationModel> GetAllAsync(int page, int itemsPerPage)
  {
    string cmd = @"Select * From MovContratos Limit @p_Pag, @p_rows;";
    _dbContext.Parametros.AddItem("@p_Pag", page);
    _dbContext.Parametros.AddItem("@p_rows", itemsPerPage);
    DataSet result = await _dbContext.QueryAsync(cmd, 0);
    return _paginationService.GetPaginationData(result, page, itemsPerPage);
  }
  public async Task<PaginationModel> GetAllAsync(string nome, string insc, int situacao, string dataInicial, string dataFinal, int page, long vendedor)
  {
    long idCompany = Convert.ToInt64(AppIdentity.GetClaimValue("IdCompany"));
    int itemsPerPage = 30;
    if (string.IsNullOrEmpty(nome)) nome = "";
    if (string.IsNullOrEmpty(insc))
      insc = "";
    else
      insc = ExtensionService.CleanSpecialChars(insc);

    string cmd = @"
    Select Mc.Id, Mc.Fk_MovPropostas, Cc.Id ClienteId, Mc.DataCriacao, Mc.Situacao, Cc.Nome, Cc.Inscricao, Mp.ValorAprovado, Ce.Nome Vendedor,
    (
	    Select Count(1) From MovPropostasBeneficiarios Mpb Where Mpb.Fk_MovPropostas = Mc.Fk_MovPropostas
    ) Beneficiarios
    From MovContratos Mc
    Join MovPropostas Mp on (Mp.Id = Mc.Fk_MovPropostas)
    Join CadClientes Cc on (Cc.Id = Mp.Fk_CadClientes)
    Join CadEquipe Ce on (Ce.Id = Cc.Fk_CadEquipe) and (Ce.Fk_CadEmpresas = @p_CadEmpresas)
    WHERE 
    (
    @p_Inscricao <> '' AND Cc.Inscricao = @p_Inscricao
    )
    OR
    (
    @p_Inscricao = '' AND
    Mc.DataCriacao BETWEEN @p_DataInicial AND @p_DataFinal AND
    (@p_Nome = '' OR Cc.Nome LIKE CONCAT('%', @p_Nome, '%')) AND
    (@p_Situacao = 0 OR Mc.Situacao = @p_Situacao) and 
    (@p_Vendedor = 0 OR Mp.Fk_CadEquipe = @p_Vendedor)
    )
    Order by Mc.DataCriacao Desc
    Limit @p_Pag, @p_rows;";

    if (dataInicial == "0001-01-01 00:00:00")
      cmd = cmd.Replace("Mc.DataCriacao BETWEEN @p_DataInicial AND @p_DataFinal AND", "");
    _dbContext.Parametros.AddItem("@p_Nome", nome);
    _dbContext.Parametros.AddItem("@p_Inscricao", insc);
    _dbContext.Parametros.AddItem("@p_Situacao", situacao);
    _dbContext.Parametros.AddItem("@p_DataInicial", dataInicial);
    _dbContext.Parametros.AddItem("@p_DataFinal", dataFinal);
    _dbContext.Parametros.AddItem("@p_Pag", ExtensionService.OffsetCalculator(page, itemsPerPage));
    _dbContext.Parametros.AddItem("@p_CadEmpresas", idCompany);
    _dbContext.Parametros.AddItem("@p_rows", itemsPerPage);
    _dbContext.Parametros.AddItem("@p_Vendedor", vendedor);
    DataSet result = await _dbContext.QueryAsync(cmd, 0);
    return _paginationService.GetPaginationData(result, page, itemsPerPage);
  }
  public async Task<object> DeleteAsync(long id)
  {
    if (await IsExists(id) == false)
      return NotificationService.NoExists();

    string cmd = @"Delete From MovContratos Where Id = @p_Id;";
    AddIdParameter(id);
    await _dbContext.ExecAsync(cmd, 0);
    return NotificationService.Success();
  }

  public List<MovContratosModel> GetListByDataSet(DataSet ds)
  {
    var list = new List<MovContratosModel>();

    if (ds?.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
      foreach (DataRow row in ds.Tables[0].Rows)
        list.Add(ConvertDataRowToObj(row));

    return list;
  }

  public MovContratosModel ConvertDataRowToObj(DataRow dr)
  {
    return new MovContratosModel
    {
      Id = dr.Field<long>("Id"),
      MovPropostasModel = new MovPropostasModel { Id = dr.Field<long>("Fk_MovPropostas") },
      Situacao = ExtensionService.SafeConvert<byte>(dr["Situacao"]),
      DataCriacao = ExtensionService.SafeConvert<DateTime>(dr["DataCriacao"]),
      DataFechamento = ExtensionService.SafeConvert<DateTime>(dr["DataFechamento"])
    };
  }

  public MovContratosModel SetByDataSet(DataSet ds)
  {
    if (ds?.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
      return ConvertDataRowToObj(ds.Tables[0].Rows[0]);
    return new MovContratosModel();
  }

  public async Task<bool> IsExists(long id)
  {
    var item = await GetByIdAsync(id);
    return item.Id > 0;
  }
  public async Task<object> UpdateStatus(long contractId, int status)
  {
    if (await IsExists(contractId) == false)
      return NotificationService.NoExists();

    string cmd = @"Update MovContratos
      Set Situacao = @p_Situacao,
      DataFechamento = Now()
      Where Id = @p_Id;";
    _dbContext.Parametros.AddItem("@p_Situacao", status);
    _dbContext.Parametros.AddItem("@p_Id", contractId);
    await _dbContext.ExecAsync(cmd, 0);
    return NotificationService.Success();
  }
}
