using RecomeceAPI.Interfaces;
using RecomeceAPI.Models;
using RecomeceAPI.Models.Common;
using RecomeceAPI.Server.Database;
using RecomeceAPI.Services.Common;
using System.Data;

namespace RecomeceAPI.Services
{
    public class MovPropostasArquivosService : IRepository<MovPropostasArquivosModel>
  {
    private readonly DbContext _dbContext;
    private readonly PaginationService _paginationService;
    private readonly GenericModelValidatorService _genericModelValidatorService;

    public MovPropostasArquivosService(DbContext dbContext, 
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
    public void AddEntityParameters(MovPropostasArquivosModel entity)
    {
      AddIdParameter(entity.Id);
      _dbContext.Parametros.AddItem("@p_ArquivoBytes", entity.ArquivoBytes); 
      _dbContext.Parametros.AddItem("@p_DataConferencia", entity.DataConferencia); 
      _dbContext.Parametros.AddItem("@p_DataCriacao", entity.DataCriacao); 
      _dbContext.Parametros.AddItem("@p_Fk_MovPropostas", entity.MovPropostas.Id); 
      _dbContext.Parametros.AddItem("@p_NomeArquivo", entity.NomeArquivo); 
      _dbContext.Parametros.AddItem("@p_Observacao", entity.Observacao); 
      _dbContext.Parametros.AddItem("@p_Situacao", entity.Situacao); 
      _dbContext.Parametros.AddItem("@p_Tipo", entity.Tipo); 
      _dbContext.Parametros.AddItem("@p_MovPropostasBeneficiarios", entity.MovPropostasBeneficiarios.Id); 

    }
    public async Task<object> SaveAsync(MovPropostasArquivosModel entity)
    {
      var validation = _genericModelValidatorService.GenericValidator(entity);
      if(validation.Count > 0)
        return validation;

      entity.DataCriacao = DateTime.Now;
      string cmd = @"Insert Into MovPropostasArquivos (Id, ArquivoBytes, DataConferencia, DataCriacao, Fk_MovPropostas, NomeArquivo, Observacao, Situacao, Tipo, Fk_MovPropostasBeneficiarios) Select @p_Id, @p_ArquivoBytes, @p_DataConferencia, @p_DataCriacao, @p_Fk_MovPropostas, @p_NomeArquivo, @p_Observacao, @p_Situacao, @p_Tipo, @p_MovPropostasBeneficiarios; Select LAST_INSERT_ID();";
      AddEntityParameters(entity);
      return await _dbContext.SaveAsync(cmd, 0);
    }
    public async Task<object> UpdateAsync(MovPropostasArquivosModel entity)
    {
      var validation = _genericModelValidatorService.GenericValidator(entity);
      if (validation.Count > 0)
        return validation;

      if (await IsExists(entity.Id) == false)
        return NotificationService.NoExists();

      string cmd = @"Update MovPropostasArquivos
      Set 
      Id = @p_Id, 
      ArquivoBytes = @p_ArquivoBytes, 
      DataConferencia = @p_DataConferencia, 
      DataCriacao = @p_DataCriacao, 
      Fk_MovPropostas = @p_Fk_MovPropostas, 
      NomeArquivo = @p_NomeArquivo, 
      Observacao = @p_Observacao, 
      Situacao = @p_Situacao, 
      Tipo = @p_Tipo
      Where Id = @p_Id;";
      AddEntityParameters(entity);
      await _dbContext.ExecAsync(cmd, 0);
      return entity;
    }
    public async Task<MovPropostasArquivosModel> GetByIdAsync(long id)
    {
      string cmd = @"Select * From MovPropostasArquivos Where Id = @p_Id;";
      AddIdParameter(id);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return SetByDataSet(result);
    }
    public async Task<PaginationModel> GetAllAsync(int page, int itemsPerPage)
    {
      string cmd = @"Select * From MovPropostasArquivos Limit @p_Pag, @p_rows;";
      _dbContext.Parametros.AddItem("@p_Pag", page);
      _dbContext.Parametros.AddItem("@p_rows", itemsPerPage);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return _paginationService.GetPaginationData(result, page, itemsPerPage);
    }
    public async Task<object> GetAllProposal(long proposal)
    {
      string cmd = @"Select Mpb.Id, Cc.Nome, Coalesce(Mpa.Id, 0) IdArquivo
      From MovPropostasBeneficiarios Mpb
      Join CadClientes Cc on (Cc.Id = Mpb.Fk_CadClientes)
      Left Join MovPropostasArquivos Mpa on (Mpa.Fk_MovPropostas = Mpb.Fk_MovPropostas) and (Mpa.Fk_MovPropostasBeneficiarios = Mpb.Id) and (Mpa.Tipo = 2)
      Where (Mpb.Fk_MovPropostas = @p_MovPropostas);";
      _dbContext.Parametros.AddItem("@p_MovPropostas", proposal);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return result.Tables[0];
    }
    public async Task<object> GetContractProposal(long proposal)
    {
      string cmd = @"Select Id, DataCriacao
      From MovPropostasArquivos
      Where (Fk_MovPropostas = @p_MovPropostas) and (Tipo = 1);";
      _dbContext.Parametros.AddItem("@p_MovPropostas", proposal);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return result.Tables[0];
    }
    public async Task<object> GetFichasProposal(long proposal)
    {
      string cmd = @"Select Id, DataCriacao
      From MovPropostasArquivos
      Where (Fk_MovPropostas = @p_MovPropostas) and (Tipo = 2);";
      _dbContext.Parametros.AddItem("@p_MovPropostas", proposal);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return result.Tables[0];
    }
    public async Task<object> DeleteAsync(long id)
    {
      if(await IsExists(id) == false)
        return NotificationService.NoExists();
      string cmd = @"Delete From MovPropostasArquivos Where Id = @p_Id;";
      AddIdParameter(id);
      await _dbContext.ExecAsync(cmd, 0);
      return NotificationService.Success();
    }
    public List<MovPropostasArquivosModel> GetListByDataSet(DataSet ds)
    {
      var list = new List<MovPropostasArquivosModel>();

      if (ds?.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        foreach (DataRow row in ds.Tables[0].Rows)
          list.Add(ConvertDataRowToObj(row));

      return list;
    }
    public MovPropostasArquivosModel ConvertDataRowToObj(DataRow dr)
    {
      return new MovPropostasArquivosModel
      {
       Id = dr.Field<long>("Id"), 
       ArquivoBytes = dr.Field<byte[]>("ArquivoBytes"), 
       DataConferencia = dr.Field<DateTime>("DataConferencia"), 
       DataCriacao = dr.Field<DateTime>("DataCriacao"), 
       MovPropostas = new MovPropostasModel { Id = dr.Field<long>("Fk_MovPropostas")}, 
       NomeArquivo = dr.Field<string>("NomeArquivo") ?? "", 
       Observacao = dr.Field<string>("Observacao") ?? "", 
       Situacao = ExtensionService.SafeConvert<byte>("Situacao"),
       Tipo = ExtensionService.SafeConvert<byte>("Tipo"),
      };
    }
    public MovPropostasArquivosModel SetByDataSet(DataSet ds)
    {
      if (ds?.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        return ConvertDataRowToObj(ds.Tables[0].Rows[0]);
      return new MovPropostasArquivosModel(); 
    }

    public async Task<bool> IsExists(long id)
    {
      var item = await GetByIdAsync(id);
      return item.Id > 0;
    }
  }
}