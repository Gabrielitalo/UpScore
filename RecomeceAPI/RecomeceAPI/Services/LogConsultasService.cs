using RecomeceAPI.Interfaces;
using RecomeceAPI.Models;
using RecomeceAPI.Models.Common;
using RecomeceAPI.Server.Auth;
using RecomeceAPI.Server.Database;
using RecomeceAPI.Services.Common;
using RecomeceAPI.Services.PDFs;
using RecomeceAPI.Services.Serasa;
using System.Data;

namespace RecomeceAPI.Services
{
  public class LogConsultasService : IRepository<LogConsultasModel>
  {
    private readonly DbContext _dbContext;
    private readonly PaginationService _paginationService;
    private readonly GenericModelValidatorService _genericModelValidatorService;
    private readonly CadProdutosService _cadProdutosService;
    private readonly CadConsultasService _cadConsultasService;
    private readonly MovContaCorrenteService _movContaCorrenteService;

    public LogConsultasService(DbContext dbContext,
      PaginationService paginationService,
      GenericModelValidatorService genericModelValidatorService,
      CadProdutosService cadProdutosService,
      CadConsultasService cadConsultasService,
      MovContaCorrenteService movContaCorrenteService
      )
    {
      _dbContext = dbContext;
      _paginationService = paginationService;
      _genericModelValidatorService = genericModelValidatorService;
      _cadProdutosService = cadProdutosService;
      _cadConsultasService = cadConsultasService;
      _movContaCorrenteService = movContaCorrenteService;
    }

    public void AddIdParameter(long id)
    {
      _dbContext.Parametros.AddItem("@p_Id", id.ToString());
    }

    public void AddEntityParameters(LogConsultasModel entity)
    {
      _dbContext.Parametros.AddItem("@p_Id", entity.Id);
      _dbContext.Parametros.AddItem("@p_Fk_CadEquipe", entity.Fk_CadEquipe);
      _dbContext.Parametros.AddItem("@p_Fk_CadConsultas", entity.Fk_CadConsultas);
      _dbContext.Parametros.AddItem("@p_Inscricao", entity.Inscricao);
      _dbContext.Parametros.AddItem("@p_MarkID", entity.MarkID);
      _dbContext.Parametros.AddItem("@p_Situacao", entity.Situacao);
      _dbContext.Parametros.AddItem("@p_DataHoraConsulta", entity.DataHoraConsulta);
      _dbContext.Parametros.AddItem("@p_Score", entity.Score);
      _dbContext.Parametros.AddItem("@p_TotalNegativacao", entity.TotalNegativacao);
      _dbContext.Parametros.AddItem("@p_ArquivoEnvio", entity.ArquivoEnvio);
      _dbContext.Parametros.AddItem("@p_ArquivoRetorno", entity.ArquivoRetorno);
    }

    public async Task<object> SaveAsync(LogConsultasModel entity)
    {
      var validation = _genericModelValidatorService.GenericValidator(entity);
      if (validation.Count > 0)
        return validation;

      entity.DataHoraConsulta = DateTime.Now;

      string cmd = @"INSERT INTO LogConsultas 
        (Id, Fk_CadEquipe, Inscricao, MarkID, Situacao, DataHoraConsulta, Score, TotalNegativacao, ArquivoEnvio, ArquivoRetorno, Fk_CadConsultas)
        SELECT @p_Id, @p_Fk_CadEquipe, @p_Inscricao, @p_MarkID, @p_Situacao, @p_DataHoraConsulta, @p_Score, @p_TotalNegativacao, @p_ArquivoEnvio, @p_ArquivoRetorno, @p_Fk_CadConsultas;
        Select LAST_INSERT_ID();";
      AddEntityParameters(entity);
      return await _dbContext.SaveAsync(cmd, 0);
    }

    public async Task<object> UpdateAsync(LogConsultasModel entity)
    {
      var validation = _genericModelValidatorService.GenericValidator(entity);
      if (validation.Count > 0)
        return validation;

      if (!await IsExists(entity.Id))
        return NotificationService.NoExists();

      string cmd = @"UPDATE LogConsultas SET
        Fk_CadEquipe = @p_Fk_CadEquipe,
        Inscricao = @p_Inscricao,
        MarkID = @p_MarkID,
        Situacao = @p_Situacao,
        DataHoraConsulta = @p_DataHoraConsulta,
        Score = @p_Score,
        TotalNegativacao = @p_TotalNegativacao,
        ArquivoEnvio = @p_ArquivoEnvio,
        ArquivoRetorno = @p_ArquivoRetorno
        WHERE Id = @p_Id;";
      AddEntityParameters(entity);
      await _dbContext.ExecAsync(cmd, 0);
      return entity;
    }
    public async Task<PaginationModel> QuerySerasa(int page, string nome, string insc, string dataInicial, string dataFinal)
    {
      long idUser = Convert.ToInt64(AppIdentity.GetClaimValue("IdUser"));
      long idRole = Convert.ToInt64(AppIdentity.GetClaimValue("IdRole"));
      long idCompany = Convert.ToInt64(AppIdentity.GetClaimValue("IdCompany"));
      insc = ExtensionService.CleanSpecialChars(insc);
      _dbContext.Parametros.AddItem("@p_Pag", page);
      _dbContext.Parametros.AddItem("@p_Nome", nome);
      _dbContext.Parametros.AddItem("@p_Inscricao", insc);
      _dbContext.Parametros.AddItem("@p_DataInicial", dataInicial);
      _dbContext.Parametros.AddItem("@p_DataFinal", dataFinal);
      _dbContext.Parametros.AddItem("@p_IdUser", idUser);
      _dbContext.Parametros.AddItem("@p_IdCompany", idCompany);
      _dbContext.Parametros.AddItem("@p_IdRole", idRole);
      var result = await _dbContext.QueryAsync("GetLogSerasaConsultas", 1);
      return _paginationService.GetPaginationData(result, page, 20);
    }
    public async Task<LogConsultasModel> GetByIdAsync(long id)
    {
      string cmd = @"SELECT * FROM LogConsultas WHERE Id = @p_Id;";
      AddIdParameter(id);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return SetByDataSet(result);
    }
    public async Task<object> GetAllClients(long clientId)
    {
      string cmd = @"Select L.*
      From LogConsultas L
      Join CadClientes C on (C.Inscricao = L.Inscricao)
      Where (C.Id = @p_Id)
      Order by L.DataHoraConsulta Desc;";
      _dbContext.Parametros.AddItem("@p_Id", clientId);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return result.Tables[0];
    }
    public async Task<LogConsultasModel> GetById(string id)
    {
      string cmd = @"SELECT * FROM LogConsultas WHERE Id = @p_Id;";
      _dbContext.Parametros.AddItem("@p_Id", id);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return SetByDataSet(result);
    }
    public async Task<LogConsultasModel> GetByMarkId(string markId)
    {
      string cmd = @"SELECT * FROM LogConsultas WHERE MarkID = @p_MarkID;";
      _dbContext.Parametros.AddItem("@p_MarkID", markId);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return SetByDataSet(result);
    }
    public async Task<LogConsultasModel> GetByInscricaoAsync(string inscricao)
    {
      inscricao = ExtensionService.CleanSpecialChars(inscricao);  
      string cmd = @"SELECT * FROM LogConsultas WHERE Inscricao = @p_Inscricao and Legado = 0 and Situacao = 1 Limit 1;";
      _dbContext.Parametros.AddItem("@p_Inscricao", inscricao);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return SetByDataSet(result);
    }
    public async Task<LogConsultasModel> GetByInscricaoProdutoAsync(string inscricao, long productId)
    {
      inscricao = ExtensionService.CleanSpecialChars(inscricao);  
      string cmd = @"SELECT * FROM LogConsultas WHERE Inscricao = @p_Inscricao and Legado = 0 and Fk_CadConsultas = @p_CadConsultas Order by DataHoraConsulta Desc Limit 1;";
      _dbContext.Parametros.AddItem("@p_Inscricao", inscricao);
      _dbContext.Parametros.AddItem("@p_CadConsultas", productId);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return SetByDataSet(result);
    }
    public async Task<PaginationModel> GetAllAsync(int page, int itemsPerPage)
    {
      string cmd = @"SELECT * FROM LogConsultas LIMIT @p_Pag, @p_rows;";
      _dbContext.Parametros.AddItem("@p_Pag", page);
      _dbContext.Parametros.AddItem("@p_rows", itemsPerPage);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return _paginationService.GetPaginationData(result, page, itemsPerPage);
    }

    public async Task<object> DeleteAsync(long id)
    {
      if (!await IsExists(id))
        return NotificationService.NoExists();

      string cmd = @"DELETE FROM LogConsultas WHERE Id = @p_Id;";
      AddIdParameter(id);
      await _dbContext.ExecAsync(cmd, 0);
      return NotificationService.Success();
    }

    public List<LogConsultasModel> GetListByDataSet(DataSet ds)
    {
      var list = new List<LogConsultasModel>();

      if (ds?.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        foreach (DataRow row in ds.Tables[0].Rows)
          list.Add(ConvertDataRowToObj(row));

      return list;
    }

    public LogConsultasModel ConvertDataRowToObj(DataRow dr)
    {
      return new LogConsultasModel
      {
        Id = dr.Field<string>("Id"),
        Fk_CadEquipe = dr.Field<long?>("Fk_CadEquipe"),
        Fk_CadConsultas = dr.Field<long?>("Fk_CadConsultas"),
        Inscricao = dr.Field<string>("Inscricao") ?? "",
        MarkID = dr.Field<string>("MarkID") ?? "",
        Situacao = ExtensionService.SafeConvert<int>(dr["Situacao"]),
        DataHoraConsulta = dr.Field<DateTime?>("DataHoraConsulta"),
        Score = dr.Field<decimal?>("Score"),
        TotalNegativacao = dr.Field<decimal?>("TotalNegativacao"),
        ArquivoEnvio = dr.Field<string>("ArquivoEnvio") ?? "",
        ArquivoRetorno = dr.Field<string>("ArquivoRetorno") ?? ""
      };
    }

    public LogConsultasModel SetByDataSet(DataSet ds)
    {
      if (ds?.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        return ConvertDataRowToObj(ds.Tables[0].Rows[0]);
      return new LogConsultasModel();
    }

    public async Task<bool> IsExists(string id)
    {
      string cmd = @"SELECT COUNT(*) FROM LogConsultas WHERE Id = @p_Id;";
      _dbContext.Parametros.AddItem("@p_Id", id);
      var ds = await _dbContext.QueryAsync(cmd, 0);
      if (ds?.Tables[0].Rows.Count > 0)
      {
        var count = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
        return count > 0;
      }
      return false;
    }

    // Sobrecarga para long
    public async Task<bool> IsExists(long id) => await IsExists(id.ToString());

  }
}
