using ClosedXML.Excel;
using MySqlX.XDevAPI;
using RecomeceAPI.Interfaces;
using RecomeceAPI.Models;
using RecomeceAPI.Models.Common;
using RecomeceAPI.Server.Database;
using RecomeceAPI.Services.Common;
using System.Data;

namespace RecomeceAPI.Services
{
  public class MovPropostasBeneficiariosService : IRepository<MovPropostasBeneficiariosModel>
  {
    private readonly DbContext _dbContext;
    private readonly PaginationService _paginationService;
    private readonly GenericModelValidatorService _genericModelValidatorService;
    public readonly CadProdutosFaixasService _cadProdutosFaixasService;
    private readonly MovPropostasService _movPropostasService;
    private readonly IServiceProvider _serviceProvider;

    public MovPropostasBeneficiariosService(DbContext dbContext,
      PaginationService paginationService,
      GenericModelValidatorService genericModelValidatorService,
      CadProdutosFaixasService cadProdutosFaixasService,
      MovPropostasService movPropostasService)
    {
      _dbContext = dbContext;
      _paginationService = paginationService;
      _genericModelValidatorService = genericModelValidatorService;
      _cadProdutosFaixasService = cadProdutosFaixasService;
      _movPropostasService = movPropostasService;
    }
    public void AddIdParameter(long id)
    {
      _dbContext.Parametros.AddItem("@p_Id", id);
    }
    public void AddEntityParameters(MovPropostasBeneficiariosModel entity)
    {
      AddIdParameter(entity.Id);
      _dbContext.Parametros.AddItem("@p_Fk_CadClientes", entity.CadClientes.Id);
      _dbContext.Parametros.AddItem("@p_Fk_MovPropostas", entity.MovPropostas.Id);
      _dbContext.Parametros.AddItem("@p_Observacao", entity.Observacao);
      _dbContext.Parametros.AddItem("@p_Score", entity.Score);
      _dbContext.Parametros.AddItem("@p_Tipo", entity.Tipo);
      _dbContext.Parametros.AddItem("@p_ValorContrato", entity.ValorContrato);
      _dbContext.Parametros.AddItem("@p_ValorDivida", entity.ValorDivida);
      _dbContext.Parametros.AddItem("@p_MarkID", entity.MarkID);

    }
    public async Task<object> SaveAsync(MovPropostasBeneficiariosModel entity)
    {
      var validation = _genericModelValidatorService.GenericValidator(entity);
      if (validation.Count > 0)
        return validation;

      string cmd = @"Insert Into MovPropostasBeneficiarios (Id, Fk_CadClientes, Fk_MovPropostas, Observacao, Score, Tipo, ValorContrato, ValorDivida, MarkID) Select @p_Id, @p_Fk_CadClientes, @p_Fk_MovPropostas, @p_Observacao, @p_Score, @p_Tipo, @p_ValorContrato, @p_ValorDivida, @p_MarkID; Select LAST_INSERT_ID();";
      AddEntityParameters(entity);
      return await _dbContext.SaveAsync(cmd, 0);
    }
    public async Task<object> UpdateAsync(MovPropostasBeneficiariosModel entity)
    {
      var validation = _genericModelValidatorService.GenericValidator(entity);
      if (validation.Count > 0)
        return validation;

      if (await IsExists(entity.Id) == false)
        return NotificationService.NoExists();

      string cmd = @"Update MovPropostasBeneficiarios
      Set 
      Id = @p_Id, 
      Fk_CadClientes = @p_Fk_CadClientes, 
      Fk_MovPropostas = @p_Fk_MovPropostas, 
      Observacao = @p_Observacao, 
      Score = @p_Score, 
      Tipo = @p_Tipo, 
      ValorContrato = @p_ValorContrato, 
      ValorDivida = @p_ValorDivida,
      MarkID = @p_MarkID
      Where Id = @p_Id;";
      AddEntityParameters(entity);
      await _dbContext.ExecAsync(cmd, 0);
      return entity;
    }
    public async Task<MovPropostasBeneficiariosModel> GetByIdAsync(long id)
    {
      string cmd = @"Select Mpb.*, Cc.Nome, Cc.Inscricao
      From MovPropostasBeneficiarios Mpb
      Join CadClientes Cc on (Cc.Id = Mpb.Fk_CadClientes)
      Where Mpb.Id = @p_Id;";
      AddIdParameter(id);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return SetByDataSet(result);
    }
    public async Task<List<MovPropostasBeneficiariosModel>> GetAllByProposal(long proposalId)
    {
      string cmd = @"Select Mpb.*, Cc.Nome, Cc.Inscricao
      From MovPropostasBeneficiarios Mpb
      Join CadClientes Cc on (Cc.Id = Mpb.Fk_CadClientes)
      Where Mpb.Fk_MovPropostas = @p_MovPropostas;";
      _dbContext.Parametros.AddItem("@p_MovPropostas", proposalId);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return GetListByDataSet(result);
    }
    public async Task<PaginationModel> GetAllAsync(int page, int itemsPerPage)
    {
      string cmd = @"Select * From MovPropostasBeneficiarios Limit @p_Pag, @p_rows;";
      _dbContext.Parametros.AddItem("@p_Pag", page);
      _dbContext.Parametros.AddItem("@p_rows", itemsPerPage);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return _paginationService.GetPaginationData(result, page, itemsPerPage);
    }
    public async Task<object> DeleteAsync(long id)
    {
      if (await IsExists(id) == false)
        return NotificationService.NoExists();
      string cmd = @"Delete From MovPropostasBeneficiarios Where Id = @p_Id;";
      AddIdParameter(id);
      await _dbContext.ExecAsync(cmd, 0);
      return NotificationService.Success();
    }
    public List<MovPropostasBeneficiariosModel> GetListByDataSet(DataSet ds)
    {
      var list = new List<MovPropostasBeneficiariosModel>();

      if (ds?.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        foreach (DataRow row in ds.Tables[0].Rows)
          list.Add(ConvertDataRowToObj(row));

      return list;
    }
    public MovPropostasBeneficiariosModel ConvertDataRowToObj(DataRow dr)
    {
      return new MovPropostasBeneficiariosModel
      {
        Id = dr.Field<long>("Id"),
        CadClientes = new CadClientesModel { Id = dr.Field<long>("Fk_CadClientes"), Inscricao = dr.Field<string>("Inscricao") ?? "", Nome = dr.Field<string>("Nome") ?? "" },
        MovPropostas = new MovPropostasModel { Id = dr.Field<long>("Fk_MovPropostas") },
        Observacao = dr.Field<string>("Observacao") ?? "",
        MarkID = dr.Field<string>("MarkID") ?? "",
        Score = dr.Field<int>("Score"),
        Tipo = ExtensionService.SafeConvert<byte>(dr["Tipo"]),
        ValorContrato = dr.Field<decimal>("ValorContrato"),
        ValorDivida = dr.Field<decimal>("ValorDivida")
      };
    }
    public MovPropostasBeneficiariosModel SetByDataSet(DataSet ds)
    {
      if (ds?.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        return ConvertDataRowToObj(ds.Tables[0].Rows[0]);
      return new MovPropostasBeneficiariosModel();
    }

    public async Task<bool> IsExists(long id)
    {
      var item = await GetByIdAsync(id);
      return item.Id > 0;
    }
    public async Task<object> NewProposalClientSerasa(long proposalId, MovCadClientesSerasaModel serasa)
    {
      var proposal = await _movPropostasService.GetByIdAsync(proposalId);
      var debitValue = await _cadProdutosFaixasService.GetProductDebit(proposal.CadProdutos.Id, serasa.TotalNegativacao);

      var entity = new MovPropostasBeneficiariosModel
      {
        Id = 0,
        CadClientes = serasa.CadClientes,
        MovPropostas = new MovPropostasModel { Id = proposalId },
        Score = serasa.Score,
        ValorDivida = serasa.TotalNegativacao,
        ValorContrato = debitValue.ValorServico,
        Observacao = "",
        Tipo = 1,
        MarkID = serasa.MarkID,
      };
      return await SaveAsync(entity);
    }

    public decimal TotalDivida(List<MovPropostasBeneficiariosModel>? beneficiarios)
        => beneficiarios?.Sum(s => s.ValorDivida) ?? 0m;
  }
}