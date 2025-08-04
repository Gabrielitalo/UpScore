using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.ExtendedProperties;
using RecomeceAPI.Interfaces;
using RecomeceAPI.Models;
using RecomeceAPI.Models.ASAAS;
using RecomeceAPI.Models.Common;
using RecomeceAPI.Server.Auth;
using RecomeceAPI.Server.Database;
using RecomeceAPI.Services.ASAAS;
using RecomeceAPI.Services.Common;
using System.Data;

namespace RecomeceAPI.Services
{
  public class MovContaCorrenteService : IRepository<MovContaCorrenteModel>
  {
    private readonly DbContext _dbContext;
    private readonly PaginationService _paginationService;
    private readonly GenericModelValidatorService _genericModelValidatorService;
    private readonly PaymentService _paymentService;
    private readonly CadEmpresasService _cadEmpresasService;

    public MovContaCorrenteService(DbContext dbContext,
      PaginationService paginationService,
      GenericModelValidatorService genericModelValidatorService,
      PaymentService paymentService, CadEmpresasService cadEmpresasService)
    {
      _dbContext = dbContext;
      _paginationService = paginationService;
      _genericModelValidatorService = genericModelValidatorService;
      _paymentService = paymentService;
      _cadEmpresasService = cadEmpresasService;
    }

    public void AddIdParameter(long id)
    {
      _dbContext.Parametros.AddItem("@p_Id", id.ToString());
    }

    public void AddEntityParameters(MovContaCorrenteModel entity)
    {
      _dbContext.Parametros.AddItem("@p_Id", entity.Id);
      _dbContext.Parametros.AddItem("@p_CadEmpresas", entity.CadEmpresas.Id);
      _dbContext.Parametros.AddItem("@p_Fk_Origem", entity.Fk_Origem);
      _dbContext.Parametros.AddItem("@p_OrigemTipo", entity.OrigemTipo);
      _dbContext.Parametros.AddItem("@p_Situacao", entity.Situacao);
      _dbContext.Parametros.AddItem("@p_Tipo", entity.Tipo);
      _dbContext.Parametros.AddItem("@p_Valor", entity.Valor);
      _dbContext.Parametros.AddItem("@p_IdExtPagto", entity.IdExtPagto);
      _dbContext.Parametros.AddItem("@p_DataCriacao", entity.DataCriacao);
      _dbContext.Parametros.AddItem("@p_DataPagamento", entity.DataPagamento);
      _dbContext.Parametros.AddItem("@p_Historico", entity.Historico);
    }

    public async Task<object> SaveAsync(MovContaCorrenteModel entity)
    {
      var validation = _genericModelValidatorService.GenericValidator(entity);
      if (validation.Count > 0)
        return validation;

      string cmd = @"INSERT INTO MovContaCorrente 
        (Id, Fk_CadEmpresas, Fk_Origem, OrigemTipo, Situacao, Tipo, Valor, IdExtPagto, DataCriacao, DataPagamento, Historico)
        SELECT @p_Id, @p_CadEmpresas, @p_Fk_Origem, @p_OrigemTipo, @p_Situacao, @p_Tipo, @p_Valor, @p_IdExtPagto, @p_DataCriacao, @p_DataPagamento, @p_Historico;
        ";
      AddEntityParameters(entity);
      return await _dbContext.SaveAsync(cmd, 0);
    }

    public async Task<object> UpdateAsync(MovContaCorrenteModel entity)
    {
      var validation = _genericModelValidatorService.GenericValidator(entity);
      if (validation.Count > 0)
        return validation;

      if (!await IsExists(entity.Id))
        return NotificationService.NoExists();

      string cmd = @"UPDATE MovContaCorrente SET
        Fk_CadEmpresas = @p_CadEmpresas,
        Fk_Origem = @p_Fk_Origem,
        OrigemTipo = @p_OrigemTipo,
        Situacao = @p_Situacao,
        Tipo = @p_Tipo,
        Valor = @p_Valor,
        IdExtPagto = @p_IdExtPagto,
        DataCriacao = @p_DataCriacao,
        DataPagamento = @p_DataPagamento,
        Historico = @p_Historico
        WHERE Id = @p_Id;";
      AddEntityParameters(entity);
      await _dbContext.ExecAsync(cmd, 0);
      return entity;
    }

    public async Task SetReceivePayment(string id)
    {
      string cmd = @"UPDATE MovContaCorrente SET DataPagamento = Now(), Situacao = 1 WHERE Id = @p_Id;";
      _dbContext.Parametros.AddItem("@p_Id", id);
      await _dbContext.ExecAsync(cmd, 0);
    }

    public async Task<MovContaCorrenteModel> GetByIdAsync(long id)
    {
      string cmd = @"SELECT * FROM MovContaCorrente WHERE Id = @p_Id;";
      AddIdParameter(id);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return SetByDataSet(result);
    }

    public async Task<object> GetLastBuy10()
    {
      string cmd = @"SELECT * FROM MovContaCorrente WHERE Fk_CadEmpresas = @p_CadEmpresas and Tipo = 2 Order by DataCriacao Desc Limit 10;";
      _dbContext.Parametros.AddItem("@p_CadEmpresas", AppIdentity.GetIdCompanyValue());
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return result.Tables[0];
    }

    public async Task<decimal> GetTotal()
    {
      string cmd = @"SELECT Sum(Valor) Valor FROM MovContaCorrente WHERE Fk_CadEmpresas = @p_CadEmpresas and Situacao = 1;";
      _dbContext.Parametros.AddItem("@p_CadEmpresas", AppIdentity.GetIdCompanyValue());
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      if (result?.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
        return ExtensionService.SafeConvert<decimal>(result.Tables[0].Rows[0]["Valor"]);
      else
        return 0;
    }

    public async Task<PaginationModel> GetAllAsync(int page, int itemsPerPage, int tipo, string dataInicial, string dataFinal)
    {
      string cmd = @"Select Mc.Id, Mc.Fk_Origem, Mc.OrigemTipo, Mc.Situacao, Mc.Tipo, Mc.Valor, Mc.IdExtPagto, Mc.DataCriacao, Mc.DataPagamento, Mc.Historico,
      Case When Mc.Tipo = 1 Then 'Débito' Else 'Crédito' End TipoDesc,
      Lc.Inscricao, Cc.Tipo TipoConsulta
      From MovContaCorrente Mc
      Left Join LogConsultas Lc on (Lc.Id = Mc.Fk_Origem)
      Left Join CadConsultas Cc on (Cc.Id = Lc.Fk_CadConsultas)
      Where (Fk_CadEmpresas = @p_CadEmpresas) and (Mc.Situacao = 1)
      and Mc.DataCriacao BETWEEN @p_DataInicial AND @p_DataFinal 
      and Mc.Tipo = @p_Tipo
      Order by Mc.DataCriacao Desc
      LIMIT @p_Pag, @p_rows;";

      if (dataInicial == "0001-01-01 00:00:00")
        cmd = cmd.Replace("and Mc.DataCriacao BETWEEN @p_DataInicial AND @p_DataFinal", "");

      if (tipo == 0)
        cmd = cmd.Replace("and Mc.Tipo = @p_Tipo", "");

      _dbContext.Parametros.AddItem("@p_CadEmpresas", AppIdentity.GetIdCompanyValue());
      _dbContext.Parametros.AddItem("@p_Pag", page);
      _dbContext.Parametros.AddItem("@p_rows", itemsPerPage);
      _dbContext.Parametros.AddItem("@p_Tipo", tipo);
      _dbContext.Parametros.AddItem("@p_DataInicial", dataInicial);
      _dbContext.Parametros.AddItem("@p_DataFinal", dataFinal);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return _paginationService.GetPaginationData(result, page, itemsPerPage);
    }

    public async Task<object> DeleteAsync(long id)
    {
      if (!await IsExists(id))
        return NotificationService.NoExists();

      string cmd = @"DELETE FROM MovContaCorrente WHERE Id = @p_Id;";
      AddIdParameter(id);
      await _dbContext.ExecAsync(cmd, 0);
      return NotificationService.Success();
    }

    public List<MovContaCorrenteModel> GetListByDataSet(DataSet ds)
    {
      var list = new List<MovContaCorrenteModel>();

      if (ds?.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        foreach (DataRow row in ds.Tables[0].Rows)
          list.Add(ConvertDataRowToObj(row));

      return list;
    }

    public MovContaCorrenteModel ConvertDataRowToObj(DataRow dr)
    {
      return new MovContaCorrenteModel
      {
        Id = dr.Field<string>("Id"),
        CadEmpresas = new CadEmpresasModel { Id = dr.Field<long>("Fk_CadEmpresas") },
        Fk_Origem = dr.Field<string>("Fk_Origem") ?? "",
        OrigemTipo = dr.Field<int>("OrigemTipo"),
        Situacao = dr.Field<int>("Situacao"),
        Tipo = dr.Field<int>("Tipo"),
        Valor = dr.Field<decimal>("Valor"),
        IdExtPagto = dr.Field<string>("IdExtPagto") ?? "",
        DataCriacao = dr.Field<DateTime>("DataCriacao"),
        DataPagamento = dr.Field<DateTime?>("DataPagamento"),
        Historico = dr.Field<string>("Historico") ?? ""
      };
    }

    public MovContaCorrenteModel SetByDataSet(DataSet ds)
    {
      if (ds?.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        return ConvertDataRowToObj(ds.Tables[0].Rows[0]);
      return new MovContaCorrenteModel();
    }

    public async Task<bool> IsExists(string id)
    {
      string cmd = @"SELECT COUNT(*) FROM MovContaCorrente WHERE Id = @p_Id;";
      _dbContext.Parametros.AddItem("@p_Id", id);
      var ds = await _dbContext.QueryAsync(cmd, 0);
      if (ds?.Tables[0].Rows.Count > 0)
      {
        var count = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
        return count > 0;
      }
      return false;
    }

    // Sobrecarga para aceitar long (caso necessário)
    public async Task<bool> IsExists(long id) => await IsExists(id.ToString());

    public async Task<MovContaCorrenteModel> GetByIdPagtoExt(string payId)
    {
      string cmd = "Select * From MovContaCorrente Where IdExtPagto = @p_IdExtPagto;";
      _dbContext.Parametros.AddItem("@p_IdExtPagto", payId);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return SetByDataSet(result);
    }
    public async Task SaveLogSerasa(string tipoEvento, string arquivo)
    {
      string cmd = @"Insert Into LogAsaas 
        (DataHoraConsulta, TipoEvento, ArquivoRetorno)
        Values (@p_DataHoraConsulta, @p_TipoEvento, @p_ArquivoRetorno); 
        Select LAST_INSERT_ID();";

      _dbContext.Parametros.AddItem("@p_DataHoraConsulta", DateTime.Now);
      _dbContext.Parametros.AddItem("@p_TipoEvento", tipoEvento);
      _dbContext.Parametros.AddItem("@p_ArquivoRetorno", arquivo);
      await _dbContext.SaveAsync(cmd, 0);
    }

    public async Task ReceivePayment(PaymentWebHookModel webHook)
    {
      var model = await GetByIdPagtoExt(webHook.Payment.Id);
      if (!string.IsNullOrEmpty(model.Id))
        await SetReceivePayment(model.Id);
    }

    public async Task<PaymentModel> CreateAsaas(MovContaCorrenteModel entity)
    {
      return await _paymentService.Create(entity.CadEmpresas.IdAsaas, "PIX", entity.Valor, DateTime.Now, entity.Historico);
    }
    public async Task AddAccountValue(decimal value)
    {
      var company = await _cadEmpresasService.GetCompanyUserLogged();
      var entity = new MovContaCorrenteModel();
      entity.Valor = value;
      entity.CadEmpresas = company;
      entity.Tipo = 2; // Crédito
      entity.Historico = $"Depósito de R$ {entity.Valor} na conta corrente";
      entity.DataCriacao = DateTime.Now;
      entity.Fk_Origem = "";
      var payment = await CreateAsaas(entity);
      if (!string.IsNullOrEmpty(payment.Id))
      {
        entity.IdExtPagto = payment.Id;
        await SaveAsync(entity);
      }
    }
    public async Task AddDebitFromQuery(decimal value, string markId)
    {
      var entity = new MovContaCorrenteModel();
      var company = await _cadEmpresasService.GetCompanyUserLogged();
      entity.Valor = -value;
      entity.CadEmpresas = company;
      entity.Situacao = 1;
      entity.Tipo = 1; // Débito
      entity.Historico = $"Débito de R$ {entity.Valor} referente a consulta realizadas";
      entity.DataCriacao = DateTime.Now;
      entity.Fk_Origem = markId;
      entity.OrigemTipo = 1; // Consultas
      entity.IdExtPagto = "";
      await SaveAsync(entity);
    }

    public Task<PaginationModel> GetAllAsync(int page, int itensPerPage)
    {
      throw new NotImplementedException();
    }
  }
}
