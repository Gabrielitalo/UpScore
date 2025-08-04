using ClosedXML.Excel;
using DocumentFormat.OpenXml.Vml.Office;
using RecomeceAPI.Interfaces;
using RecomeceAPI.Models;
using RecomeceAPI.Models.Common;
using RecomeceAPI.Server.Database;
using RecomeceAPI.Services.Common;
using System.Data;

namespace RecomeceAPI.Services
{
  public class MovLotesEmpresasBeneficiariosService : MovLotesEmpresasBeneficiariosModel
  {
    private readonly DbContext _dbContext;
    private readonly PaginationService _paginationService;
    private readonly GenericModelValidatorService _genericModelValidatorService;

    public MovLotesEmpresasBeneficiariosService(DbContext dbContext,
      PaginationService paginationService,
      GenericModelValidatorService genericModelValidatorService)
    {
      _dbContext = dbContext;
      _paginationService = paginationService;
      _genericModelValidatorService = genericModelValidatorService;
    }

    public void AddIdParameter(string id)
    {
      _dbContext.Parametros.AddItem("@p_Id", id);
    }

    public void AddEntityParameters(MovLotesEmpresasBeneficiariosModel entity)
    {
      _dbContext.Parametros.AddItem("@p_Id", entity.Id);
      _dbContext.Parametros.AddItem("@p_MovLotesEmpresas", entity.MovLotesEmpresas.Id);
      _dbContext.Parametros.AddItem("@p_Inscricao", entity.Inscricao);
      _dbContext.Parametros.AddItem("@p_Nome", entity.Nome);
      _dbContext.Parametros.AddItem("@p_Situacao", entity.Situacao);
    }

    public async Task<object> SaveAsync(MovLotesEmpresasBeneficiariosModel entity)
    {
      var validation = _genericModelValidatorService.GenericValidator(entity);
      if (validation.Count > 0)
        return validation;

      string cmd = @"Insert Into MovLotesEmpresasBeneficiarios (Id, Fk_MovLotesEmpresas, Inscricao, Nome, Situacao)
                     Select @p_Id, @p_MovLotesEmpresas, @p_Inscricao, @p_Nome, @p_Situacao;
                     Select LAST_INSERT_ID();";
      AddEntityParameters(entity);
      return await _dbContext.SaveAsync(cmd, 0);
    }

    public async Task<object> UpdateAsync(MovLotesEmpresasBeneficiariosModel entity)
    {
      var validation = _genericModelValidatorService.GenericValidator(entity);
      if (validation.Count > 0)
        return validation;

      if (await IsExists(entity.Id) == false)
        return NotificationService.NoExists();

      string cmd = @"Update MovLotesEmpresasBeneficiarios
                     Set Fk_MovLotesEmpresas = @p_Fk_MovLotesEmpresas,
                         Inscricao = @p_Inscricao,
                         Nome = @p_Nome,
                         Situacao = @p_Situacao
                     Where Id = @p_Id;";
      AddEntityParameters(entity);
      await _dbContext.ExecAsync(cmd, 0);
      return entity;
    }

    public async Task<object> UpdateAsync(string id, string inscricao, string nome)
    {
      inscricao = ExtensionService.CleanSpecialChars(inscricao);  
      int situacao = DocumentoValidator.IsCpfOuCnpjValido(inscricao) == true ? 1 : 0;
      string cmd = @"Update MovLotesEmpresasBeneficiarios
                     Set Inscricao = @p_Inscricao,
                         Nome = @p_Nome,
                         Situacao = @p_Situacao
                     Where Id = @p_Id;";
      _dbContext.Parametros.AddItem("@p_Id", id);
      _dbContext.Parametros.AddItem("@p_Inscricao", inscricao);
      _dbContext.Parametros.AddItem("@p_Nome", nome);
      _dbContext.Parametros.AddItem("@p_Situacao", situacao);
      await _dbContext.ExecAsync(cmd, 0);
      return id;
    }

    public async Task<MovLotesEmpresasBeneficiariosModel> GetByIdAsync(string id)
    {
      string cmd = "Select * From MovLotesEmpresasBeneficiarios Where Id = @p_Id;";
      AddIdParameter(id);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return SetByDataSet(result);
    }

    public async Task<PaginationModel> GetAllAsync(int page, int itemsPerPage)
    {
      string cmd = @"Select * From MovLotesEmpresasBeneficiarios Limit @p_Pag, @p_rows;";
      _dbContext.Parametros.AddItem("@p_Pag", page);
      _dbContext.Parametros.AddItem("@p_rows", itemsPerPage);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return _paginationService.GetPaginationData(result, page, itemsPerPage);
    }

    public async Task<object> GetAllByBatch(string batchId)
    {
      string cmd = @"Select Id, Fk_MovLotesEmpresas, Nome, Situacao,
  CASE 
    WHEN LENGTH(Inscricao) = 11 THEN
      CONCAT(
        SUBSTRING(Inscricao, 1, 3), '.', 
        SUBSTRING(Inscricao, 4, 3), '.', 
        SUBSTRING(Inscricao, 7, 3), '-', 
        SUBSTRING(Inscricao, 10, 2)
      )
    WHEN LENGTH(Inscricao) = 14 THEN
      CONCAT(
        SUBSTRING(Inscricao, 1, 2), '.', 
        SUBSTRING(Inscricao, 3, 3), '.', 
        SUBSTRING(Inscricao, 6, 3), '/', 
        SUBSTRING(Inscricao, 9, 4), '-', 
        SUBSTRING(Inscricao, 13, 2)
      )
    ELSE Inscricao
  END AS InscricaoFormatada
From MovLotesEmpresasBeneficiarios Where Fk_MovLotesEmpresas = @p_MovLotesEmpresas;";
      _dbContext.Parametros.AddItem("@p_MovLotesEmpresas", batchId);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return result.Tables[0];
    }


    public async Task<object> DeleteAsync(string id)
    {
      if (await IsExists(id) == false)
        return NotificationService.NoExists();

      string cmd = "Delete From MovLotesEmpresasBeneficiarios Where Id = @p_Id;";
      AddIdParameter(id);
      await _dbContext.ExecAsync(cmd, 0);
      return NotificationService.Success();
    }

    public List<MovLotesEmpresasBeneficiariosModel> GetListByDataSet(DataSet ds)
    {
      var list = new List<MovLotesEmpresasBeneficiariosModel>();

      if (ds?.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        foreach (DataRow row in ds.Tables[0].Rows)
          list.Add(ConvertDataRowToObj(row));

      return list;
    }

    public MovLotesEmpresasBeneficiariosModel ConvertDataRowToObj(DataRow dr)
    {
      return new MovLotesEmpresasBeneficiariosModel
      {
        Id = dr.Field<string>("Id") ?? string.Empty,
        MovLotesEmpresas = new MovLotesEmpresasModel { Id = dr.Field<string>("Fk_MovLotesEmpresas") ?? "" },
        Inscricao = dr.Field<string>("Inscricao") ?? string.Empty,
        Nome = dr.Field<string>("Nome") ?? string.Empty,
        Situacao = dr.Field<int?>("Situacao") ?? 0
      };
    }

    public MovLotesEmpresasBeneficiariosModel SetByDataSet(DataSet ds)
    {
      if (ds?.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        return ConvertDataRowToObj(ds.Tables[0].Rows[0]);
      return new MovLotesEmpresasBeneficiariosModel();
    }

    public async Task<bool> IsExists(string id)
    {
      var item = await GetByIdAsync(id);
      return !string.IsNullOrWhiteSpace(item.Id);
    }


    public async Task<List<MovLotesEmpresasBeneficiariosModel>> GetAllMembers(string batchId)
    {
      string cmd = @"Select Mb.Id, Mb.Fk_MovLotesEmpresas, Mb.Inscricao, Mb.Nome, Mb.Situacao
      From MovLotesAssociacao Ma
      Join MovLotesAssociacaoComponentes Mac on (Mac.Fk_MovLotesAssociacao = Ma.Id)
      Join MovLotesEmpresas M on (M.Id = Mac.Fk_MovLotesEmpresas)
      Join MovLotesEmpresasBeneficiarios Mb on (Mb.Fk_MovLotesEmpresas = M.Id)
      Where Ma.Id = @p_Id
      Order by Mb.Nome Asc;";
      _dbContext.Parametros.AddItem("@p_Id", batchId);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return GetListByDataSet(result);
    }

    public byte[] GetPlan(List<MovLotesEmpresasBeneficiariosModel> clientes)
    {
      using var workbook = new XLWorkbook();
      var worksheet = workbook.Worksheets.Add("Clientes");

      // Cabeçalhos
      worksheet.Cell(1, 1).Value = "Inscricao";
      worksheet.Cell(1, 2).Value = "Nome";

      // Dados
      for (int i = 0; i < clientes.Count; i++)
      {
        var cliente = clientes[i];
        worksheet.Cell(i + 2, 1).Value = ExtensionService.CleanSpecialChars(cliente.Inscricao);
        worksheet.Cell(i + 2, 2).Value = cliente.Nome;
      }

      using var stream = new MemoryStream();
      workbook.SaveAs(stream);
      return stream.ToArray(); // Retorna o conteúdo como byte[] para download via API
    }
  }
}
