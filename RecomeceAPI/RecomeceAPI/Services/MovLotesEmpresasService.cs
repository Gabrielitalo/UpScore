using ClosedXML.Excel;
using DocumentFormat.OpenXml.ExtendedProperties;
using RecomeceAPI.DTOs;
using RecomeceAPI.Interfaces;
using RecomeceAPI.Models;
using RecomeceAPI.Models.Common;
using RecomeceAPI.Server.Auth;
using RecomeceAPI.Server.Database;
using RecomeceAPI.Services.Common;
using System.Data;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace RecomeceAPI.Services
{
  public class MovLotesEmpresasService : MovLotesEmpresasModel
  {
    private readonly DbContext _dbContext;
    private readonly PaginationService _paginationService;
    private readonly GenericModelValidatorService _genericModelValidatorService;
    private readonly CadEmpresasService _cadEmpresasService;
    private readonly MovLotesEmpresasBeneficiariosService _movLotesEmpresasBeneficiariosService;

    public MovLotesEmpresasService(DbContext dbContext,
      PaginationService paginationService,
      GenericModelValidatorService genericModelValidatorService,
      CadEmpresasService cadEmpresasService,
      MovLotesEmpresasBeneficiariosService movLotesEmpresasBeneficiariosService)
    {
      _dbContext = dbContext;
      _paginationService = paginationService;
      _genericModelValidatorService = genericModelValidatorService;
      _cadEmpresasService = cadEmpresasService;
      _movLotesEmpresasBeneficiariosService = movLotesEmpresasBeneficiariosService;
    }

    public void AddIdParameter(string id)
    {
      _dbContext.Parametros.AddItem("@p_Id", id);
    }

    public void AddEntityParameters(MovLotesEmpresasModel entity)
    {
      AddIdParameter(entity.Id);
      _dbContext.Parametros.AddItem("@p_CadEmpresas", entity.CadEmpresas.Id);
      _dbContext.Parametros.AddItem("@p_Situacao", entity.Situacao);
      _dbContext.Parametros.AddItem("@p_Data", entity.Data);
      _dbContext.Parametros.AddItem("@p_TotalBeneficiarios", entity.TotalBeneficiarios);
      _dbContext.Parametros.AddItem("@p_Valor", entity.Valor);
      _dbContext.Parametros.AddItem("@p_IdExtPagto", entity.IdExtPagto);
      _dbContext.Parametros.AddItem("@p_Serasa", entity.Serasa);
      _dbContext.Parametros.AddItem("@p_Boavista", entity.Boavista);
      _dbContext.Parametros.AddItem("@p_SPC", entity.SPC);
      _dbContext.Parametros.AddItem("@p_CenprotSP", entity.CenprotSP);
      _dbContext.Parametros.AddItem("@p_CenprotNacional", entity.CenprotNacional);
      _dbContext.Parametros.AddItem("@p_Tipo", entity.Tipo);
    }

    public async Task<object> SaveAsync(MovLotesEmpresasModel entity)
    {
      var validation = _genericModelValidatorService.GenericValidator(entity);
      if (validation.Count > 0)
        return validation;

      string cmd = @"Insert Into MovLotesEmpresas 
        (Id, Fk_CadEmpresas, Situacao, Data, TotalBeneficiarios, Valor, Tipo) 
        Select @p_Id, @p_CadEmpresas, @p_Situacao, @p_Data, @p_TotalBeneficiarios, @p_Valor, @p_Tipo;
        Select LAST_INSERT_ID();";

      AddEntityParameters(entity);
      return await _dbContext.SaveAsync(cmd, 0);
    }

    public async Task<object> UpdateAsync(MovLotesEmpresasModel entity)
    {
      var validation = _genericModelValidatorService.GenericValidator(entity);
      if (validation.Count > 0)
        return validation;

      if (await IsExists(entity.Id) == false)
        return NotificationService.NoExists();

      string cmd = @"Update MovLotesEmpresas
        Set 
          Fk_CadEmpresas = @p_CadEmpresas, 
          Situacao = @p_Situacao, 
          Data = @p_Data, 
          TotalBeneficiarios = @p_TotalBeneficiarios
        Where Id = @p_Id;";

      AddEntityParameters(entity);
      await _dbContext.ExecAsync(cmd, 0);
      return entity;
    }

    public async Task<object> UpdateStatus(string id, int situacao)
    {
      string cmd = @"Update MovLotesEmpresas
        Set Situacao = @p_Situacao
        Where Id = @p_Id;";

      _dbContext.Parametros.AddItem("@p_Id", id);
      _dbContext.Parametros.AddItem("@p_Situacao", situacao);
      await _dbContext.ExecAsync(cmd, 0);
      return true;
    }

    public async Task<MovLotesEmpresasModel> GetByIdAsync(string id)
    {
      string cmd = @"Select * From MovLotesEmpresas Where Id = @p_Id;";
      AddIdParameter(id);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return SetByDataSet(result);
    }

    public async Task<PaginationModel> GetAllAsync(int page, int itemsPerPage)
    {
      string cmd = @"Select * From MovLotesEmpresas Limit @p_Pag, @p_rows;";
      _dbContext.Parametros.AddItem("@p_Pag", page);
      _dbContext.Parametros.AddItem("@p_rows", itemsPerPage);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return _paginationService.GetPaginationData(result, page, itemsPerPage);
    }
    public async Task<object> GetAllContractsAvailables()
    {
      long idCompany = Convert.ToInt64(AppIdentity.GetClaimValue("IdCompany"));
      string cmd = @"Select Mc.Id, Mc.Fk_MovPropostas, Cc.Id ClienteId, Mc.DataCriacao, Mc.Situacao, Cc.Nome, Cc.Inscricao, Mp.ValorAprovado, true Checked,
      (
      Select Count(1) From MovPropostasBeneficiarios Mpb Where Mpb.Fk_MovPropostas = Mc.Fk_MovPropostas
      ) Beneficiarios
      From MovContratos Mc
      Join MovPropostas Mp on (Mp.Id = Mc.Fk_MovPropostas)
      Join CadEquipe Ce on (Ce.Id = Mp.Fk_CadEquipe) and (Ce.Fk_CadEmpresas = @p_CadEmpresas)
      Join CadClientes Cc on (Cc.Id = Mp.Fk_CadClientes)
      WHERE Mc.Situacao = 3
      AND NOT EXISTS (
      SELECT 1
      FROM MovLotesEmpresasContratos Mle
      WHERE Mle.Fk_MovContratos = Mc.Id
      );";
      _dbContext.Parametros.AddItem("@p_CadEmpresas", idCompany);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return result.Tables[0];
    }

    public async Task<object> DeleteAsync(string id)
    {
      if (await IsExists(id) == false)
        return NotificationService.NoExists();

      string cmd = @"Delete From MovLotesEmpresasBeneficiarios Where Fk_MovLotesEmpresas = @p_Id;
      Delete From MovLotesEmpresas Where Id = @p_Id;";
      AddIdParameter(id);
      await _dbContext.ExecAsync(cmd, 0);
      return NotificationService.Success();
    }
    public async Task UpdateTotalBeneficarios(long id)
    {
      string cmd = @"UPDATE MovLotesEmpresas
      JOIN (
        SELECT Me.Id AS LoteId, COUNT(1) AS Total
        FROM MovLotesEmpresas Me
        JOIN MovLotesEmpresasContratos Mec ON Mec.Fk_MovLotesEmpresas = Me.Id
        JOIN MovContratos Mc ON Mc.Id = Mec.Fk_MovContratos
        JOIN MovPropostas Mp ON Mp.Id = Mc.Fk_MovPropostas
        JOIN MovPropostasBeneficiarios Mpb ON Mpb.Fk_MovPropostas = Mp.Id
        WHERE Me.Id = @p_MovLotesEmpresas
        GROUP BY Me.Id
      ) AS Totais ON MovLotesEmpresas.Id = Totais.LoteId
      SET MovLotesEmpresas.TotalBeneficiarios = Totais.Total
      WHERE MovLotesEmpresas.Id = @p_MovLotesEmpresas;";
      _dbContext.Parametros.AddItem("@p_MovLotesEmpresas", id);
      await _dbContext.ExecAsync(cmd, 0);
    }

    public List<MovLotesEmpresasModel> GetListByDataSet(DataSet ds)
    {
      var list = new List<MovLotesEmpresasModel>();

      if (ds?.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        foreach (DataRow row in ds.Tables[0].Rows)
          list.Add(ConvertDataRowToObj(row));

      return list;
    }

    public MovLotesEmpresasModel ConvertDataRowToObj(DataRow dr)
    {
      return new MovLotesEmpresasModel
      {
        Id = dr.Field<string>("Id") ?? "",
        CadEmpresas = new CadEmpresasModel { Id = dr.Field<long>("Fk_CadEmpresas") },
        Situacao = ExtensionService.SafeConvert<byte>(dr["Situacao"]),
        Valor = ExtensionService.SafeConvert<decimal>(dr["Valor"], 0),
        Data = dr.Field<DateTime>("Data"),
        TotalBeneficiarios = dr.Field<int>("TotalBeneficiarios"),
        Serasa = dr.Field<int>("Serasa"),
        Boavista = dr.Field<int>("Boavista"),
        SPC = dr.Field<int>("SPC"),
        CenprotSP = dr.Field<int>("CenprotSP"),
        CenprotNacional = dr.Field<int>("CenprotNacional"),
        IdExtPagto = dr.Field<string>("IdExtPagto") ?? "",
      };
    }

    public MovLotesEmpresasModel SetByDataSet(DataSet ds)
    {
      if (ds?.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        return ConvertDataRowToObj(ds.Tables[0].Rows[0]);
      return new MovLotesEmpresasModel();
    }

    public async Task<bool> IsExists(string id)
    {
      var item = await GetByIdAsync(id);
      return !string.IsNullOrEmpty(item.Id);
    }
    public async Task<PaginationModel> GetAllAsync(string membro, string dataInicial, string dataFinal, int page, string? associacaoId)
    {
      long idCompany = AppIdentity.GetIdCompanyValue();
      bool isAssociacao = AppIdentity.GetIsAssociacaoValue();

      int itemsPerPage = 30;
      if (string.IsNullOrEmpty(membro)) membro = "";

      string cmd = "";
      if (string.IsNullOrEmpty(associacaoId))
      {

        if (isAssociacao == false)
        {
          if (string.IsNullOrEmpty(membro))
          {
            cmd = @"
        Select M.Id, M.Fk_CadEmpresas, M.Situacao, M.Data, M.TotalBeneficiarios, M.Tipo, M.Valor,
        Case When M.Tipo = 1 Then 'Envio' Else 'Reenvio' End TipoDesc, C.Nome
        From MovLotesEmpresas M
        Join CadEmpresas C on (C.Id = M.Fk_CadEmpresas)
        Where M.Fk_CadEmpresas = @p_CadEmpresas
        ";
          }
          else
          {
            cmd = @"
        Select Distinct M.Id, M.Fk_CadEmpresas, M.Situacao, M.Data, M.TotalBeneficiarios, M.Tipo, M.Valor,
        Case When M.Tipo = 1 Then 'Envio' Else 'Reenvio' End TipoDesc, C.Nome
        From MovLotesEmpresas M
        Join MovLotesEmpresasBeneficiarios Mb on (Mb.Fk_MovLotesEmpresas = M.Id)
        Join CadEmpresas C on (C.Id = M.Fk_CadEmpresas)
        Where M.Fk_CadEmpresas = @p_CadEmpresas and
        ((Mb.Inscricao like @p_texto) or (Mb.Nome like @p_texto))
        ";
          }
        }
        else
        {
          if (string.IsNullOrEmpty(membro))
          {
            cmd = @"
          Select M.Id, M.Fk_CadEmpresas, M.Situacao, M.Data, M.TotalBeneficiarios, M.Tipo, M.Valor,
          Case When M.Tipo = 1 Then 'Envio' Else 'Reenvio' End TipoDesc, C.Nome
          From MovLotesEmpresas M
          Join CadEmpresas C on (C.Id = M.Fk_CadEmpresas)
          Where M.Tipo > 0
        ";
          }
          else
          {
            cmd = @"
          Select Distinct M.Id, M.Fk_CadEmpresas, M.Situacao, M.Data, M.TotalBeneficiarios, M.Tipo, M.Valor,
          Case When M.Tipo = 1 Then 'Envio' Else 'Reenvio' End TipoDesc, C.Nome
          From MovLotesEmpresas M
          Join MovLotesEmpresasBeneficiarios Mb on (Mb.Fk_MovLotesEmpresas = M.Id)
          Join CadEmpresas C on (C.Id = M.Fk_CadEmpresas)
          Where ((Mb.Inscricao like @p_texto) or (Mb.Nome like @p_texto))
        ";
          }
        }

        if (DateOnly.TryParse(dataInicial, out var dateStart) && DateOnly.TryParse(dataFinal, out var dateEnd))
        {
          cmd += " AND (DATE(M.Data) BETWEEN @p_DataInicial AND @p_DataFinal)";
          _dbContext.Parametros.AddItem("@p_DataInicial", dateStart.ToDateTime(TimeOnly.MinValue));
          _dbContext.Parametros.AddItem("@p_DataFinal", dateEnd.ToDateTime(TimeOnly.MaxValue));
        }

        cmd += @"  Order by M.Data Desc Limit @p_Pag, @p_rows;";
      }
      else
      {
        cmd = @"Select M.Id, M.Fk_CadEmpresas, M.Situacao, M.Data, M.TotalBeneficiarios, M.Tipo, M.Valor,
        Case When M.Tipo = 1 Then 'Envio' Else 'Reenvio' End TipoDesc, C.Nome
        From MovLotesAssociacao Ma
        Join MovLotesAssociacaoComponentes Mac on (Mac.Fk_MovLotesAssociacao = Ma.Id)
        Join MovLotesEmpresas M on (M.Id = Mac.FK_MovLotesEmpresas)
        Join CadEmpresas C on (C.Id = M.Fk_CadEmpresas)
        Where (Ma.Id = @p_associacaoId);";
      }

      _dbContext.Parametros.AddItem("@p_Pag", ExtensionService.OffsetCalculator(page, itemsPerPage));
      _dbContext.Parametros.AddItem("@p_CadEmpresas", idCompany);
      _dbContext.Parametros.AddItem("@p_texto", $"%{membro}%", 1);
      _dbContext.Parametros.AddItem("@p_rows", itemsPerPage);
      _dbContext.Parametros.AddItem("@p_associacaoId", associacaoId);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return _paginationService.GetPaginationData(result, page, itemsPerPage);
    }
    public async Task<object> SaveContract(long contractId, string batchId)
    {
      string cmd = @"Insert Into MovLotesEmpresasContratos(Fk_MovLotesEmpresas, Fk_MovContratos)
        Select @p_MovLotesEmpresas, @p_MovContratos;
        Select LAST_INSERT_ID();";

      _dbContext.Parametros.AddItem("@p_MovLotesEmpresas", batchId);
      _dbContext.Parametros.AddItem("@p_MovContratos", contractId);
      return await _dbContext.SaveAsync(cmd, 0);
    }
    public async Task<object> GetBeneficiarios(long batchId)
    {
      string cmd = @"SELECT Mpb.Id BeneficiarioId, Cc.Id ClienteId, Cc.Inscricao, Cc.Nome
      FROM MovLotesEmpresas Me
      JOIN MovLotesEmpresasContratos Mec ON Mec.Fk_MovLotesEmpresas = Me.Id
      JOIN MovContratos Mc ON Mc.Id = Mec.Fk_MovContratos
      JOIN MovPropostas Mp ON Mp.Id = Mc.Fk_MovPropostas
      JOIN MovPropostasBeneficiarios Mpb ON Mpb.Fk_MovPropostas = Mp.Id
      Join CadClientes Cc on (Cc.Id = Mpb.Fk_CadClientes)
      WHERE Me.Id = @p_MovLotesEmpresas;";

      _dbContext.Parametros.AddItem("@p_MovLotesEmpresas", batchId);
      var result = await _dbContext.QueryAsync(cmd, 0);
      return result.Tables[0];
    }
    public string NewBatchNumber()
    {
      var dtAtual = DateTime.Now;
      var random = new Random();
      int codigoAleatorio = random.Next(1000, 9999); // Número aleatório de 4 dígitos

      return $"{dtAtual:yyyyMM}-{codigoAleatorio}";
    }
    public async Task<DataTable> GetAllFiles(long batchId)
    {
      string cmd = @"SELECT Mpa.*
      FROM MovLotesEmpresas Me
      JOIN MovLotesEmpresasContratos Mec ON Mec.Fk_MovLotesEmpresas = Me.Id
      JOIN MovContratos Mc ON Mc.Id = Mec.Fk_MovContratos
      JOIN MovPropostas Mp ON Mp.Id = Mc.Fk_MovPropostas
      JOIN MovPropostasArquivos Mpa ON Mpa.Fk_MovPropostas = Mp.Id
      WHERE Me.Id = @p_MovLotesEmpresas;";

      _dbContext.Parametros.AddItem("@p_MovLotesEmpresas", batchId);
      var result = await _dbContext.QueryAsync(cmd, 0);
      return result.Tables[0];
    }
    public async Task<byte[]> GenerateZipFiles(long batchId)
    {
      var dt = await GetAllFiles(batchId);
      var files = ExportFilesLoteDTO.GetListFromDataTable(dt);
      var zip = ExportFilesLoteDTO.ZipFiles(files);
      return zip;
    }
    public async Task<byte[]> GenerateWord(long batchId)
    {
      var peoples = await GetBeneficiarios(batchId);
      List<AssociacaoWordModel> list = AssociacaoWordModel.GetListFromDataTable((DataTable)peoples);

      using (var stream = new MemoryStream())
      {
        using (var doc = DocX.Create(stream))
        {
          //doc.InsertParagraph("Tabela de Pessoas").FontSize(16).Bold().SpacingAfter(10);

          var table = doc.AddTable(list.Count + 1, 2);
          table.Design = TableDesign.TableGrid;

          table.Rows[0].Cells[0].Paragraphs[0].Append("NOME").Bold();
          table.Rows[0].Cells[1].Paragraphs[0].Append("CPF/CNPJ").Bold();

          for (int i = 0; i < list.Count; i++)
          {
            table.Rows[i + 1].Cells[0].Paragraphs[0].Append(list[i].Nome);
            table.Rows[i + 1].Cells[1].Paragraphs[0].Append(list[i].Documento);
          }

          doc.InsertTable(table);
          doc.Save();
        }

        stream.Position = 0; // volta para o início

        return stream.ToArray();
      }
    }

    public async Task<object> NewLot(long[] contractIds)
    {
      // Nesse caso tem sempre que pegar a empresa logada
      var company = AppIdentity.GetIdCompanyValue();
      var list = await GetAllNamesFromContracts(contractIds);
      return await ProcessBatch(1, list, contractIds, company);
    }
    public MovLotesEmpresasModel NewXlsBatch(int total)
    {
      var entity = new MovLotesEmpresasModel
      {
        Data = DateTime.Now,
        TotalBeneficiarios = total
      };

      return entity;
    }
    public async Task<object> ProcessXlxs(UploadDTO uploadDTO, long companyId)
    {
      var xlsx = LerPlanilhaDeBase64(uploadDTO.Base64, companyId);
      return await ProcessBatch(uploadDTO.Tipo, xlsx, null, companyId);
    }
    public async Task<object> ProcessBatch(int tipo, List<MovLotesEmpresasBeneficiariosModel> list, long[] contractIds, long companyId)
    {
      //var company = await _cadEmpresasService.GetCompanyUserLogged();
      var company = await _cadEmpresasService.GetByIdAsync(companyId);

      if (list.Count > 0)
      {
        var entity = new MovLotesEmpresasModel
        {
          Id = Guid.NewGuid().ToString(),
          Data = DateTime.Now,
          TotalBeneficiarios = list.Count,
          Tipo = tipo,
          CadEmpresas = company,
          Valor = list.Count * company.WhiteLabelConfig.ValorNomeLote
        };

        await SaveAsync(entity);
        var isSave = await GetByIdAsync(entity.Id);
        if (isSave.CadEmpresas.Id > 0)
        {
          foreach (var item in list)
          {
            item.MovLotesEmpresas = isSave;
            await _movLotesEmpresasBeneficiariosService.SaveAsync(item);
          }

          if (contractIds != null)
          {
            foreach (long id in contractIds)
            {
              await SaveContract(id, isSave.Id);
            }
          }

          return entity;
        }
        else
        {
          return new MovLotesEmpresasModel();
        }
      }
      return new MovLotesEmpresasModel();
    }
    public List<MovLotesEmpresasBeneficiariosModel> LerPlanilhaDeBase64(string base64String, long companyId)
    {
      var lista = new List<MovLotesEmpresasBeneficiariosModel>();

      // Converte a base64 string para array de bytes
      byte[] fileBytes = Convert.FromBase64String(base64String);

      using (var stream = new MemoryStream(fileBytes))
      using (var workbook = new XLWorkbook(stream))
      {
        var worksheet = workbook.Worksheet(1); // primeira aba
        var rows = worksheet.RangeUsed().RowsUsed();

        foreach (var row in rows.Skip(1)) // pula o cabeçalho
        {
          var pessoa = new MovLotesEmpresasBeneficiariosModel
          {
            Id = Guid.NewGuid().ToString(),
            Inscricao = row.Cell(1).GetString().Trim(),
            Nome = row.Cell(2).GetString().Trim(),
            Situacao = DocumentoValidator.IsCpfOuCnpjValido(row.Cell(1).GetString().Trim()) == true ? 1 : 0,
          };

          lista.Add(pessoa);
        }
      }

      return lista;
    }
    public async Task<List<MovLotesEmpresasBeneficiariosModel>> GetAllNamesFromContracts(long[] contractsIds)
    {
      var list = new List<MovLotesEmpresasBeneficiariosModel>();

      string contracts = string.Join(",", contractsIds);

      string cmd = @$"Select Cc.Inscricao, Cc.Nome
      From MovContratos Mc
      Join MovPropostasBeneficiarios Mb on (Mb.Fk_MovPropostas = Mc.Fk_MovPropostas)
      Join CadClientes Cc on (Cc.Id = Mb.Fk_CadClientes)
      Where Mc.Id in ({contracts});";

      var result = await _dbContext.QueryAsync(cmd, 0);

      if (result?.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
      {
        foreach (DataRow row in result.Tables[0].Rows)
        {
          var people = new MovLotesEmpresasBeneficiariosModel
          {
            Id = Guid.NewGuid().ToString(),
            Inscricao = row.Field<string>("Inscricao") ?? string.Empty,
            Nome = row.Field<string>("Nome") ?? string.Empty,
            Situacao = 1,
          };

          list.Add(people);
        }

      }

      return list;
    }
    public byte[] DownloadXls()
    {
      string path = Path.Combine(Directory.GetCurrentDirectory(), "Docs", "ModeloLote.xlsx");
      return File.ReadAllBytes(path);
    }
  }
}
