using DocumentFormat.OpenXml.Vml.Office;
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
  public class MovConsultasCompradasService : IRepository<MovConsultasCompradasModel>
  {
    private readonly DbContext _dbContext;
    private readonly PaginationService _paginationService;
    private readonly GenericModelValidatorService _genericModelValidatorService;
    private readonly PaymentService _paymentService;

    public MovConsultasCompradasService(DbContext dbContext,
      PaginationService paginationService,
      GenericModelValidatorService genericModelValidatorService,
      PaymentService paymentService)
    {
      _dbContext = dbContext;
      _paginationService = paginationService;
      _genericModelValidatorService = genericModelValidatorService;
      _paymentService = paymentService;
    }

    public void AddIdParameter(long id)
    {
      _dbContext.Parametros.AddItem("@p_Id", id);
    }

    public void AddEntityParameters(MovConsultasCompradasModel entity)
    {
      AddIdParameter(entity.Id);
      _dbContext.Parametros.AddItem("@p_CadEmpresas", entity.CadEmpresas.Id);
      _dbContext.Parametros.AddItem("@p_Ativo", entity.Ativo);
      _dbContext.Parametros.AddItem("@p_Situacao", entity.Situacao);
      _dbContext.Parametros.AddItem("@p_QtdConsultas", entity.QtdConsultas);
      _dbContext.Parametros.AddItem("@p_Valor", entity.Valor);
      _dbContext.Parametros.AddItem("@p_IdExtPagto", entity.IdExtPagto);
      _dbContext.Parametros.AddItem("@p_DataCriacao", entity.DataCriacao);
    }

    public async Task<object> SaveAsync(MovConsultasCompradasModel entity)
    {
      var validation = _genericModelValidatorService.GenericValidator(entity);
      if (validation.Count > 0)
        return validation;

      string cmd = @"Insert Into MovConsultasCompradas 
        (Fk_CadEmpresas, Ativo, Situacao, QtdConsultas, Valor, IdExtPagto, DataCriacao)
        Values (@p_CadEmpresas, @p_Ativo, @p_Situacao, @p_QtdConsultas, @p_Valor, @p_IdExtPagto, @p_DataCriacao); 
        Select LAST_INSERT_ID();";

      AddEntityParameters(entity);
      return await _dbContext.SaveAsync(cmd, 0);
    }

    public async Task<object> UpdateAsync(MovConsultasCompradasModel entity)
    {
      var validation = _genericModelValidatorService.GenericValidator(entity);
      if (validation.Count > 0)
        return validation;

      if (!await IsExists(entity.Id))
        return NotificationService.NoExists();

      string cmd = @"Update MovConsultasCompradas
        Set Ativo = @p_Ativo,
            Situacao = @p_Situacao,
            QtdConsultas = @p_QtdConsultas,
            Valor = @p_Valor,
            IdExtPagto = @p_IdExtPagto,
            DataCriacao = @p_DataCriacao
        Where Id = @p_Id;";

      AddEntityParameters(entity);
      await _dbContext.ExecAsync(cmd, 0);
      return entity;
    }

    public async Task ChangeStatus(long id, long status)
    {
      string cmd = @"Update MovConsultasCompradas
        Set Ativo = @p_Ativo, Situacao = @p_Situacao
        Where Id = @p_Id;";

      _dbContext.Parametros.AddItem("@p_Ativo", status);
      _dbContext.Parametros.AddItem("@p_Situacao", status);
      _dbContext.Parametros.AddItem("@p_Id", id);
      await _dbContext.ExecAsync(cmd, 0);
    }

    public async Task ReceivePayment(PaymentWebHookModel webHook)
    {
      var model = await GetByIdPagtoExt(webHook.Payment.Id);
      if (model.Id > 0)
        await ChangeStatus(model.Id, 1);
    }
    public async Task<MovConsultasCompradasModel> GetByIdAsync(long id)
    {
      string cmd = "Select * From MovConsultasCompradas Where Id = @p_Id;";
      AddIdParameter(id);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return SetByDataSet(result);
    }
    public async Task<MovConsultasCompradasModel> GetByIdPagtoExt(string payId)
    {
      string cmd = "Select * From MovConsultasCompradas Where IdExtPagto = @p_IdExtPagto;";
      _dbContext.Parametros.AddItem("@p_IdExtPagto", payId);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return SetByDataSet(result);
    }
    public async Task<long> GetQueryQtdLimit(long empresaId)
    {
      string cmd = @"Select Sum(QtdConsultas) Total
      From MovConsultasCompradas
      Where (Fk_CadEmpresas = @p_CadEmpresas) and 
      (Situacao = 1);";
      _dbContext.Parametros.AddItem("@p_CadEmpresas", empresaId);
      DataSet ds = await _dbContext.QueryAsync(cmd, 0);
      if (ds?.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        return ExtensionService.SafeConvert<long>(ds.Tables[0].Rows[0]["Total"]);

      return 0;
    }
    public async Task<PaginationModel> GetAllAsync(int page, int itemsPerPage)
    {
      string cmd = "Select * From MovConsultasCompradas Where (Fk_CadEmpresas = @p_CadEmpresas) Order By DataCriacao Desc  Limit @p_Pag, @p_rows;";
      _dbContext.Parametros.AddItem("@p_Pag", ExtensionService.OffsetCalculator(page, itemsPerPage));
      _dbContext.Parametros.AddItem("@p_CadEmpresas", Convert.ToInt64(AppIdentity.GetClaimValue("IdCompany")));
      _dbContext.Parametros.AddItem("@p_rows", itemsPerPage);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return _paginationService.GetPaginationData(result, page, itemsPerPage);
    }

    public async Task<object> DeleteAsync(long id)
    {
      if (!await IsExists(id))
        return NotificationService.NoExists();

      string cmd = "Delete From MovConsultasCompradas Where Id = @p_Id;";
      AddIdParameter(id);
      await _dbContext.ExecAsync(cmd, 0);
      return NotificationService.Success();
    }

    public List<MovConsultasCompradasModel> GetListByDataSet(DataSet ds)
    {
      var list = new List<MovConsultasCompradasModel>();

      if (ds?.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        foreach (DataRow row in ds.Tables[0].Rows)
          list.Add(ConvertDataRowToObj(row));

      return list;
    }

    public MovConsultasCompradasModel ConvertDataRowToObj(DataRow dr)
    {
      return new MovConsultasCompradasModel
      {
        Id = dr.Field<long>("Id"),
        CadEmpresas = new CadEmpresasModel { Id = dr.Field<long>("Fk_CadEmpresas") },
        Ativo = ExtensionService.SafeConvert<byte>(dr["Ativo"]),
        Situacao = dr.Field<int>("Situacao"),
        QtdConsultas = dr.Field<int>("QtdConsultas"),
        Valor = dr.Field<decimal>("Valor"),
        IdExtPagto = dr.Field<string>("IdExtPagto") ?? "",
        DataCriacao = dr.Field<DateTime>("DataCriacao")
      };
    }

    public MovConsultasCompradasModel SetByDataSet(DataSet ds)
    {
      if (ds?.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        return ConvertDataRowToObj(ds.Tables[0].Rows[0]);
      return new MovConsultasCompradasModel();
    }

    public async Task<bool> IsExists(long id)
    {
      var item = await GetByIdAsync(id);
      return item.Id > 0;
    }

    public async Task<PaymentModel> CreateAsaas(MovConsultasCompradasModel entity)
    {
      return await _paymentService.Create(entity.CadEmpresas.IdAsaas, "PIX", entity.Valor, DateTime.Now, $"Compra de {entity.QtdConsultas} consultas do Serasa");
    }
    public List<MovConsultasCompradasModel> Plans()
    {
      List<MovConsultasCompradasModel> planos = new List<MovConsultasCompradasModel>();
      planos.Add(new MovConsultasCompradasModel { Id = 1, Valor = 170, QtdConsultas = 20, Situacao = 0, Ativo = 0, DataCriacao = DateTime.Now });
      planos.Add(new MovConsultasCompradasModel { Id = 2, Valor = 400, QtdConsultas = 50, Situacao = 0, Ativo = 0, DataCriacao = DateTime.Now });
      return planos;
    }
    public MovConsultasCompradasModel GetChoosePlan(long id)
    {
      return Plans().FirstOrDefault(d => d.Id == id) ?? new MovConsultasCompradasModel();
    }

    public async Task<PaymentModel> GetByAsaasId(string pay)
    {
      return await _paymentService.GetById(pay); ;
    }
  }
}
