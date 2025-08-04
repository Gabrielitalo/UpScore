using RecomeceAPI.Interfaces;
using RecomeceAPI.Models;
using RecomeceAPI.Models.Common;
using RecomeceAPI.Server.Auth;
using RecomeceAPI.Server.Database;
using RecomeceAPI.Services.Common;
using System.Data;

namespace RecomeceAPI.Services
{
  public class MovLotesAssociacaoService : MovLotesAssociacaoModel
  {
    private readonly DbContext _dbContext;
    private readonly PaginationService _paginationService;
    private readonly GenericModelValidatorService _genericModelValidatorService;

    public MovLotesAssociacaoService(DbContext dbContext,
      PaginationService paginationService,
      GenericModelValidatorService genericModelValidatorService)
    {
      _dbContext = dbContext;
      _paginationService = paginationService;
      _genericModelValidatorService = genericModelValidatorService;
    }

    public void AddIdParameter(string id)
    {
      _dbContext.Parametros.AddItem("@p_Id", id.ToString());
    }

    public void AddEntityParameters(MovLotesAssociacaoModel entity)
    {
      AddIdParameter(entity.Id);
      _dbContext.Parametros.AddItem("@p_CadEmpresas", entity.CadEmpresas.Id);
      _dbContext.Parametros.AddItem("@p_Situacao", entity.Situacao);
      _dbContext.Parametros.AddItem("@p_Data", entity.Data);
      _dbContext.Parametros.AddItem("@p_TotalBeneficiarios", entity.TotalBeneficiarios);
      _dbContext.Parametros.AddItem("@p_Serasa", entity.Serasa);
      _dbContext.Parametros.AddItem("@p_Boavista", entity.Boavista);
      _dbContext.Parametros.AddItem("@p_SPC", entity.SPC);
      _dbContext.Parametros.AddItem("@p_CenprotSP", entity.CenprotSP);
      _dbContext.Parametros.AddItem("@p_CenprotNacional", entity.CenprotNacional);
      _dbContext.Parametros.AddItem("@p_ValorTotal", entity.ValorTotal);
    }

    public async Task<object> SaveAsync(MovLotesAssociacaoModel entity)
    {
      var validation = _genericModelValidatorService.GenericValidator(entity);
      if (validation.Count > 0)
        return validation;

      string cmd = @"Insert Into MovLotesAssociacao 
      (Id, Fk_CadEmpresas, Situacao, Data, TotalBeneficiarios, Serasa, Boavista, SPC, CenprotSP, CenprotNacional, ValorTotal)
      Select @p_Id, @p_CadEmpresas, @p_Situacao, @p_Data, @p_TotalBeneficiarios, @p_Serasa, @p_Boavista, @p_SPC, @p_CenprotSP, @p_CenprotNacional, @p_ValorTotal;
      Select LAST_INSERT_ID();";

      AddEntityParameters(entity);
      return await _dbContext.SaveAsync(cmd, 0);
    }

    public async Task<object> UpdateAsync(MovLotesAssociacaoModel entity)
    {
      var validation = _genericModelValidatorService.GenericValidator(entity);
      if (validation.Count > 0)
        return validation;

      if (await IsExists(entity.Id) == false)
        return NotificationService.NoExists();

      string cmd = @"Update MovLotesAssociacao Set 
        Fk_CadEmpresas = @p_CadEmpresas,
        Situacao = @p_Situacao,
        Data = @p_Data,
        TotalBeneficiarios = @p_TotalBeneficiarios,
        ValorTotal = @p_ValorTotal,
        Serasa = @p_Serasa,
        Boavista = @p_Boavista,
        SPC = @p_SPC,
        CenprotSP = @p_CenprotSP,
        CenprotNacional = @p_CenprotNacional
      Where Id = @p_Id;";

      AddEntityParameters(entity);
      await _dbContext.ExecAsync(cmd, 0);
      return entity;
    }

    public async Task<MovLotesAssociacaoModel> GetByIdAsync(string id)
    {
      string cmd = @"Select * From MovLotesAssociacao Where Id = @p_Id;";
      AddIdParameter(id);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return SetByDataSet(result);
    }

    public async Task<PaginationModel> GetAllAsync(string? membro, string? dataInicial, string? dataFinal, int page)
    {
      long idCompany = AppIdentity.GetIdCompanyValue();
      bool isAssociacao = AppIdentity.GetIsAssociacaoValue();

      int itemsPerPage = 30;
      if (string.IsNullOrEmpty(membro)) membro = "";

      string cmd = "";
      if (string.IsNullOrEmpty(membro))
      {
        cmd = @"
          Select Distinct Ma.Id, Ma.Fk_CadEmpresas, Ma.Situacao, Ma.Data, Ma.TotalBeneficiarios, Ma.ValorTotal, C.Nome
          From MovLotesAssociacao Ma
          Join MovLotesAssociacaoComponentes Mac on (Mac.Fk_MovLotesAssociacao = Ma.Id)
          Join MovLotesEmpresas M on (M.Id = Mac.Fk_MovLotesEmpresas)
          Join CadEmpresas C on (C.Id = Ma.Fk_CadEmpresas)
          Where Ma.Fk_CadEmpresas > 0
        ";
      }
      else
      {
        cmd = @"
          Select Distinct Ma.Id, Ma.Fk_CadEmpresas, Ma.Situacao, Ma.Data, Ma.TotalBeneficiarios, Ma.ValorTotal, C.Nome
          From MovLotesAssociacao Ma
          Join MovLotesAssociacaoComponentes Mac on (Mac.Fk_MovLotesAssociacao = Ma.Id)
          Join MovLotesEmpresas M on (M.Id = Mac.Fk_MovLotesEmpresas)
          Join CadEmpresas C on (C.Id = Ma.Fk_CadEmpresas)
          Where ((Mb.Inscricao like @p_texto) or (Mb.Nome like @p_texto))
        ";
      }
      if (DateOnly.TryParse(dataInicial, out var dateStart) &&
          DateOnly.TryParse(dataFinal, out var dateEnd))
      {
        cmd += " AND (DATE(M.Data) BETWEEN @p_DataInicial AND @p_DataFinal)";
        _dbContext.Parametros.AddItem("@p_DataInicial", dateStart.ToDateTime(TimeOnly.MinValue));
        _dbContext.Parametros.AddItem("@p_DataFinal", dateEnd.ToDateTime(TimeOnly.MaxValue));
      }

      cmd += @"  Order by Ma.Data Desc Limit @p_Pag, @p_rows;";

      _dbContext.Parametros.AddItem("@p_Pag", ExtensionService.OffsetCalculator(page, itemsPerPage));
      _dbContext.Parametros.AddItem("@p_CadEmpresas", idCompany);
      _dbContext.Parametros.AddItem("@p_texto", $"%{membro}%", 1);
      _dbContext.Parametros.AddItem("@p_rows", itemsPerPage);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return _paginationService.GetPaginationData(result, page, itemsPerPage);
    }
    public async Task<object> GetAllAvailable()
    {
      string cmd = @"Select M.*, C.Inscricao, C.Nome, Case When M.Situacao = 1 Then true Else false End checked
      From MovLotesEmpresas M
      Join CadEmpresas C on (C.Id = M.Fk_CadEmpresas)
      Left Join MovLotesAssociacaoComponentes Mac on (Mac.Fk_MovLotesEmpresas = M.Id)
      Where Mac.Fk_MovLotesEmpresas is null;";
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return result.Tables[0];
    }

    public async Task<object> GetAllMembers(string batchId)
    {
      string cmd = @"Select Mb.Inscricao, Mb.Nome
      From MovLotesAssociacao Ma
      Join MovLotesAssociacaoComponentes Mac on (Mac.Fk_MovLotesAssociacao = Ma.Id)
      Join MovLotesEmpresas M on (M.Id = Mac.Fk_MovLotesEmpresas)
      Join MovLotesEmpresasBeneficiarios Mb on (Mb.Fk_MovLotesEmpresas = M.Id)
      Where Ma.Id = @p_Id
      Order by Mb.Nome Asc;";
      _dbContext.Parametros.AddItem("@p_Id", batchId);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return result.Tables[0];
    }

    public async Task<object> DeleteAsync(string id)
    {
      if (await IsExists(id) == false)
        return NotificationService.NoExists();

      string cmd = @"Delete From MovLotesAssociacao Where Id = @p_Id;";
      AddIdParameter(id);
      await _dbContext.ExecAsync(cmd, 0);
      return NotificationService.Success();
    }

    public List<MovLotesAssociacaoModel> GetListByDataSet(DataSet ds)
    {
      var list = new List<MovLotesAssociacaoModel>();
      if (ds?.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        foreach (DataRow row in ds.Tables[0].Rows)
          list.Add(ConvertDataRowToObj(row));
      return list;
    }

    public MovLotesAssociacaoModel ConvertDataRowToObj(DataRow dr)
    {
      return new MovLotesAssociacaoModel
      {
        Id = dr.Field<string>("Id") ?? "",
        CadEmpresas = new CadEmpresasModel { Id = dr.Field<long>("Fk_CadEmpresas") },
        Situacao = dr.Field<int>("Situacao"),
        Data = dr.Field<DateTime>("Data"),
        TotalBeneficiarios = dr.Field<int>("TotalBeneficiarios"),
        Serasa = dr.Field<int?>("Serasa") ?? 0,
        Boavista = dr.Field<int?>("Boavista") ?? 0,
        SPC = dr.Field<int?>("SPC") ?? 0,
        CenprotSP = dr.Field<int?>("CenprotSP") ?? 0,
        CenprotNacional = dr.Field<int?>("CenprotNacional") ?? 0
      };
    }

    public MovLotesAssociacaoModel SetByDataSet(DataSet ds)
    {
      if (ds?.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        return ConvertDataRowToObj(ds.Tables[0].Rows[0]);
      return new MovLotesAssociacaoModel();
    }

    public async Task<bool> IsExists(string id)
    {
      var item = await GetByIdAsync(id);
      return !string.IsNullOrEmpty(item?.Id);
    }
    public async Task<object> SaveBatch(string associacaoId, string batchId)
    {
      string cmd = @"Insert Into MovLotesAssociacaoComponentes(Fk_MovLotesEmpresas, Fk_MovLotesAssociacao)
        Select @p_MovLotesEmpresas, @p_MovLotesAssociacao;
        ;";

      _dbContext.Parametros.AddItem("@p_MovLotesAssociacao", associacaoId);
      _dbContext.Parametros.AddItem("@p_MovLotesEmpresas", batchId);
      return await _dbContext.SaveAsync(cmd, 0);
    }
    public async Task<(int, decimal)> SumBenefits(string[] batchsIds)
    {
      int total = 0;
      decimal valorTotal = 0;

      // Gera parâmetros únicos: @id0, @id1, ...
      var parameters = batchsIds.Select((id, index) =>
      {
        string paramName = $"@id{index}";
        _dbContext.Parametros.AddItem(paramName, id);
        return paramName;
      });

      string inClause = string.Join(",", parameters);

      string cmd = $@"
    Select Sum(TotalBeneficiarios) Total, Sum(Valor) Valor
    From MovLotesEmpresas
    Where Id in ({inClause});";

      DataSet result = await _dbContext.QueryAsync(cmd, 0);

      if (result?.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
      {
        total = ExtensionService.SafeConvert<int>(result.Tables[0].Rows[0]["Total"], 0);
        valorTotal = ExtensionService.SafeConvert<int>(result.Tables[0].Rows[0]["Valor"], 0);
      }

      return (total, valorTotal);
    }

    public async Task NewBatch(string[] batchs)
    {
      var total = await SumBenefits(batchs);
      var entity = new MovLotesAssociacaoModel
      {
        CadEmpresas = new CadEmpresasModel() { Id = AppIdentity.GetIdCompanyValue() },
        Data = DateTime.Now,
        Situacao = 1,
        TotalBeneficiarios = total.Item1,
        ValorTotal = total.Item2,
        Id = Guid.NewGuid().ToString(),
      };
      await SaveAsync(entity);
      var batch = await GetByIdAsync(entity.Id);
      if (batch.CadEmpresas.Id > 0)
      {
        foreach (string id in batchs)
        {
          await SaveBatch(batch.Id, id);
        }
      }
    }
    public async Task<object> GetDashboard(string dataInicial, string dataFinal)
    {
      long idUser = Convert.ToInt64(AppIdentity.GetClaimValue("IdUser"));
      long idRole = Convert.ToInt64(AppIdentity.GetClaimValue("IdRole"));
      long idCompany = Convert.ToInt64(AppIdentity.GetClaimValue("IdCompany"));

      _dbContext.Parametros.AddItem("@p_IdUser", idUser);
      _dbContext.Parametros.AddItem("@p_IdRole", idRole);
      _dbContext.Parametros.AddItem("@p_IdCompany", idCompany);
      _dbContext.Parametros.AddItem("@p_DataInicial", dataInicial);
      _dbContext.Parametros.AddItem("@p_DataFinal", dataFinal);
      DataSet result = await _dbContext.QueryAsync("DashboardAssociacao", 1);
      return result.Tables;
    }
  }
}
