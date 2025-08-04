using RecomeceAPI.Interfaces;
using RecomeceAPI.Models;
using RecomeceAPI.Models.Common;
using RecomeceAPI.Models.Serasa;
using RecomeceAPI.Server.Database;
using RecomeceAPI.Services.Common;
using RecomeceAPI.Models.Serasa.Layout;
using System.Data;
using RecomeceAPI.Server.Auth;
using DocumentFormat.OpenXml.ExtendedProperties;

namespace RecomeceAPI.Services
{
  public class MovCadClientesSerasaService : IRepository<MovCadClientesSerasaModel>
  {
    private readonly DbContext _dbContext;
    private readonly PaginationService _paginationService;
    private readonly GenericModelValidatorService _genericModelValidatorService;
    private readonly CadClientesService _clienteService;
    private readonly MovConsultasCompradasService _movConsultasCompradas;

    public MovCadClientesSerasaService(DbContext dbContext,
      PaginationService paginationService,
      CadClientesService clienteService,
      GenericModelValidatorService genericModelValidatorService,
      MovConsultasCompradasService movConsultasCompradas
      )
    {
      _dbContext = dbContext;
      _paginationService = paginationService;
      _clienteService = clienteService;
      _genericModelValidatorService = genericModelValidatorService;
      _movConsultasCompradas = movConsultasCompradas;
    }
    public void AddIdParameter(long id)
    {
      _dbContext.Parametros.AddItem("@p_Id", id);
    }
    public void AddEntityParameters(MovCadClientesSerasaModel entity)
    {
      AddIdParameter(entity.Id);
      _dbContext.Parametros.AddItem("@p_AcoesJudiciais", entity.AcoesJudiciais);
      _dbContext.Parametros.AddItem("@p_AnotacoesSPC", entity.AnotacoesSPC);
      _dbContext.Parametros.AddItem("@p_ArquivoEnvio", entity.ArquivoEnvio);
      _dbContext.Parametros.AddItem("@p_ArquivoRetorno", entity.ArquivoRetorno);
      _dbContext.Parametros.AddItem("@p_ChequesSemFundos", entity.ChequesSemFundos);
      _dbContext.Parametros.AddItem("@p_Concordatas", entity.Concordatas);
      _dbContext.Parametros.AddItem("@p_DataHoraConsulta", entity.DataHoraConsulta);
      _dbContext.Parametros.AddItem("@p_DividasVencidas", entity.DividasVencidas);
      _dbContext.Parametros.AddItem("@p_Fk_CadClientes", entity.CadClientes.Id);
      _dbContext.Parametros.AddItem("@p_Fk_CadEquipe", entity.CadEquipe.Id);
      _dbContext.Parametros.AddItem("@p_PartFalencias", entity.PartFalencias);
      _dbContext.Parametros.AddItem("@p_PendenciasBancariasRefin", entity.PendenciasBancariasRefin);
      _dbContext.Parametros.AddItem("@p_PendenciasComerciaisPefin", entity.PendenciasComerciaisPefin);
      _dbContext.Parametros.AddItem("@p_Protestos", entity.Protestos);
      _dbContext.Parametros.AddItem("@p_Score", entity.Score);
      _dbContext.Parametros.AddItem("@p_Situacao", entity.Situacao);
      _dbContext.Parametros.AddItem("@p_Tipo", entity.Tipo);
      _dbContext.Parametros.AddItem("@p_TotalNegativacao", entity.TotalNegativacao);
      _dbContext.Parametros.AddItem("@p_MarkID", entity.MarkID);

    }
    public async Task<object> SaveAsync(MovCadClientesSerasaModel entity)
    {
      var validation = _genericModelValidatorService.GenericValidator(entity);
      if (validation.Count > 0)
        return validation;

      string cmd = @"Insert Into MovCadClientesSerasa (Id, MarkID, AcoesJudiciais, AnotacoesSPC, ArquivoEnvio, ArquivoRetorno, ChequesSemFundos, Concordatas, DataHoraConsulta, DividasVencidas, Fk_CadClientes, Fk_CadEquipe, PartFalencias, PendenciasBancariasRefin, PendenciasComerciaisPefin, Protestos, Score, Situacao, Tipo, TotalNegativacao) Select @p_Id, @p_MarkID, @p_AcoesJudiciais, @p_AnotacoesSPC, @p_ArquivoEnvio, @p_ArquivoRetorno, @p_ChequesSemFundos, @p_Concordatas, @p_DataHoraConsulta, @p_DividasVencidas, @p_Fk_CadClientes, @p_Fk_CadEquipe, @p_PartFalencias, @p_PendenciasBancariasRefin, @p_PendenciasComerciaisPefin, @p_Protestos, @p_Score, @p_Situacao, @p_Tipo, @p_TotalNegativacao; Select LAST_INSERT_ID();";
      AddEntityParameters(entity);
      return await _dbContext.SaveAsync(cmd, 0);
    }
    public async Task<object> UpdateAsync(MovCadClientesSerasaModel entity)
    {
      var validation = _genericModelValidatorService.GenericValidator(entity);
      if (validation.Count > 0)
        return validation;

      if (await IsExists(entity.Id) == false)
        return NotificationService.NoExists();

      string cmd = @"Update MovCadClientesSerasa
      Set 
      Id = @p_Id, 
      AcoesJudiciais = @p_AcoesJudiciais, 
      AnotacoesSPC = @p_AnotacoesSPC, 
      ArquivoEnvio = @p_ArquivoEnvio, 
      ArquivoRetorno = @p_ArquivoRetorno, 
      ChequesSemFundos = @p_ChequesSemFundos, 
      Concordatas = @p_Concordatas, 
      DataHoraConsulta = @p_DataHoraConsulta, 
      DividasVencidas = @p_DividasVencidas, 
      Fk_CadClientes = @p_Fk_CadClientes, 
      Fk_CadEquipe = @p_Fk_CadEquipe, 
      PartFalencias = @p_PartFalencias, 
      PendenciasBancariasRefin = @p_PendenciasBancariasRefin, 
      PendenciasComerciaisPefin = @p_PendenciasComerciaisPefin, 
      Protestos = @p_Protestos, 
      Score = @p_Score, 
      Situacao = @p_Situacao, 
      Tipo = @p_Tipo, 
      TotalNegativacao = @p_TotalNegativacao
      Where Id = @p_Id;";
      AddEntityParameters(entity);
      await _dbContext.ExecAsync(cmd, 0);
      return entity;
    }
    public async Task<MovCadClientesSerasaModel> GetByIdAsync(long id)
    {
      string cmd = @"Select * From MovCadClientesSerasa Where Id = @p_Id;";
      AddIdParameter(id);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return SetByDataSet(result);
    }
    public async Task<MovCadClientesSerasaModel> GetByMarkIdAsync(string markID)
    {
      string cmd = @"Select * From MovCadClientesSerasa Where MarkID = @p_MarkID Limit 1;";
      _dbContext.Parametros.AddItem("@p_MarkID", markID);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return SetByDataSet(result);
    }
    public string GetConStrGetAll()
    {
      string cmd = "";
      long idRole = Convert.ToInt64(AppIdentity.GetClaimValue("IdRole"));
      if (idRole == 0) // Adm Global
        cmd = @"Select * From MovCadClientesSerasa Order by DataHoraConsulta Desc Limit @p_Pag, @p_rows;";
      else if (idRole == 1) // Adm
        cmd = @"Select Mc.* From MovCadClientesSerasa Mcs Join CadEquipe Ce on (Ce.Id = Mcs.Fk_CadEquipe) Where (Ce.Fk_CadEmpresas = @p_CadEmpresas) Order by Mcs.DataHoraConsulta Desc Limit @p_Pag, @p_rows;";
      else if (idRole == 2) // Vendedor
        cmd = @"Select Mc.* From MovCadClientesSerasa Mcs Join CadEquipe Ce on (Ce.Id = Mcs.Fk_CadEquipe) Where (Ce.Id = @p_CadEquipe) Order by Mcs.DataHoraConsulta Desc Limit @p_Pag, @p_rows;";

      return cmd;
    }
    public async Task<PaginationModel> GetAllAsync(int page, int itemsPerPage)
    {
      long idCompany = Convert.ToInt64(AppIdentity.GetClaimValue("IdCompany"));
      long idUser = Convert.ToInt64(AppIdentity.GetClaimValue("IdUser"));
      _dbContext.Parametros.AddItem("@p_Pag", page);
      _dbContext.Parametros.AddItem("@p_rows", itemsPerPage);
      _dbContext.Parametros.AddItem("@p_CadEmpresas", idCompany);
      _dbContext.Parametros.AddItem("@p_CadEquipe", idUser);
      DataSet result = await _dbContext.QueryAsync(GetConStrGetAll(), 0);
      return _paginationService.GetPaginationData(result, page, itemsPerPage);
    }
    public async Task<object> GetAllClients(long clientId)
    {
      string cmd = @"Select * From MovCadClientesSerasa Where Fk_CadClientes = @p_CadClientes";
      _dbContext.Parametros.AddItem("@p_CadClientes", clientId);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return result.Tables[0];
    }
    public async Task<MovCadClientesSerasaModel> GetLastSerasaInsc(string insc)
    {
      long idCompany = Convert.ToInt64(AppIdentity.GetClaimValue("IdCompany"));
      string cmd = @"Select Mc.*
      From MovCadClientesSerasa Mc
      Join CadClientes C on (C.Id = Mc.Fk_CadClientes)
      Join CadEquipe Ce on (Ce.Id = C.Fk_CadEquipe) and (Ce.Fk_CadEmpresas = @p_CadEmpresas)
      Where C.Inscricao = @p_Inscricao
      Order by DataHoraConsulta Desc
      Limit 1;";
      _dbContext.Parametros.AddItem("@p_Inscricao", insc);
      _dbContext.Parametros.AddItem("@p_CadEmpresas", idCompany);
      DataSet result = await _dbContext.QueryAsync(cmd, 0);
      return SetByDataSet(result);
    }
    public bool CanQuery(DateTime queryDate)
    {
      return queryDate < DateTime.Now.AddDays(-60);
    }
    public async Task<object> DeleteAsync(long id)
    {
      if (await IsExists(id) == false)
        return NotificationService.NoExists();
      string cmd = @"Delete From MovCadClientesSerasa Where Id = @p_Id;";
      AddIdParameter(id);
      await _dbContext.ExecAsync(cmd, 0);
      return NotificationService.Success();
    }
    public List<MovCadClientesSerasaModel> GetListByDataSet(DataSet ds)
    {
      var list = new List<MovCadClientesSerasaModel>();

      if (ds?.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        foreach (DataRow row in ds.Tables[0].Rows)
          list.Add(ConvertDataRowToObj(row));

      return list;
    }
    public MovCadClientesSerasaModel ConvertDataRowToObj(DataRow dr)
    {
      return new MovCadClientesSerasaModel
      {
        Id = dr.Field<long>("Id"),
        AcoesJudiciais = dr.Field<decimal>("AcoesJudiciais"),
        AnotacoesSPC = dr.Field<decimal>("AnotacoesSPC"),
        ArquivoEnvio = dr.Field<string>("ArquivoEnvio") ?? "",
        ArquivoRetorno = dr.Field<string>("ArquivoRetorno") ?? "",
        ChequesSemFundos = dr.Field<decimal>("ChequesSemFundos"),
        Concordatas = dr.Field<decimal>("Concordatas"),
        DataHoraConsulta = dr.Field<DateTime>("DataHoraConsulta"),
        DividasVencidas = dr.Field<decimal>("DividasVencidas"),
        CadClientes = new CadClientesModel { Id = dr.Field<long>("Fk_CadClientes") },
        CadEquipe = new CadEquipeModel { Id = dr.Field<long>("Fk_CadEquipe") },
        PartFalencias = dr.Field<decimal>("PartFalencias"),
        PendenciasBancariasRefin = dr.Field<decimal>("PendenciasBancariasRefin"),
        PendenciasComerciaisPefin = dr.Field<decimal>("PendenciasComerciaisPefin"),
        Protestos = dr.Field<decimal>("Protestos"),
        Score = dr.Field<int>("Score"),
        Situacao = ExtensionService.SafeConvert<byte>(dr["Situacao"]),
        Tipo = ExtensionService.SafeConvert<byte>(dr["Tipo"]),
        TotalNegativacao = dr.Field<decimal>("TotalNegativacao"),
        MarkID = dr.Field<string>("MarkID") ?? ""
      };
    }
    public MovCadClientesSerasaModel SetByDataSet(DataSet ds)
    {
      if (ds?.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        return ConvertDataRowToObj(ds.Tables[0].Rows[0]);
      return new MovCadClientesSerasaModel();
    }

    public async Task<bool> IsExists(long id)
    {
      var item = await GetByIdAsync(id);
      return item.Id > 0;
    }
    public static string RetornaInsc(string i)
    {
      if (i.Length == 14)
        return "J";
      else
        return "F";
    }
    public static string RetornaRs(string i)
    {
      if (i.Length == 14)
        return "RSHJ";
      else
        return "RSHM";
    }
    public static string RetornaCpf(string cpf)
    {
      return cpf.PadLeft(15, '0');
    }
    public string MontaRequisicao(string insc)
    {
      string query = @"B49C      replaceaquipfpjC     FI0000000            S99SINIAN                               N                                                         000000000000000                                                                                                                                                                                                                                          P002RSPU                     RSPFPJ                                                                                  I00100DS       00000000000000  00                                                                                  T999000                                                                                                            ";
      string inscN = RetornaCpf(insc);
      query = query.Replace("replaceaquipfpj", inscN + RetornaInsc(insc));
      query = query.Replace("RSPFPJ", RetornaRs(insc));
      return query;
    }
    public async Task<string> NewClient(string insc, string tel, string email)
    {
      insc = ExtensionService.CleanSpecialChars(insc);
      return await HandleRequestsSerasa(insc, tel, email, false);
    }
    public async Task<string> RequestSerasa(string markID, string insc, string b49c)
    {
      string baseURL = "https://sitenet43-2.serasa.com.br/Prod/consultahttps?p=41434149@RECOM10        ";

      try
      {
        using (var client = new HttpClient())
        using (var request = new HttpRequestMessage(HttpMethod.Post, $"{baseURL}{b49c}"))
        {
          var response = await client.SendAsync(request);

          string responseSTR = await response.Content.ReadAsStringAsync();
          await LogSerasa(markID, insc, b49c, responseSTR);
          return responseSTR;
        }
      }
      catch (Exception ex)
      {
        // Aqui você pode fazer log ou tratamento mais detalhado
        return "";
        throw new Exception($"Erro na consulta ao Serasa: {ex.Message}", ex);
      }
    }
    public bool CheckSearchAgain(string response)
    {
      return response.Contains("T999000STRING PARCIAL - HA MAIS REGISTROS A ENVIAR");
    }
    public class LogSerasaModel()
    {
      public string Inscricao { get; set; } = string.Empty;
      public string MarkID { get; set; } = string.Empty;
      public byte Situacao { get; set; }
      public DateTime DataHoraConsulta { get; set; }
      public string ArquivoEnvio { get; set; } = string.Empty;
      public string ArquivoRetorno { get; set; } = string.Empty;
    }
    public async Task<object> LogSerasa(string markID, string insc, string request, string response)
    {
      int situacao = response.Contains("T999000PROCESSO ENCERRADO NORMALMENTE") ? 1 : 0;
      long idUser = Convert.ToInt64(AppIdentity.GetClaimValue("IdUser"));

      string cmd = @"Insert Into LogSerasaConsultas(Inscricao, Fk_CadEquipe, MarkID, Situacao, DataHoraConsulta, ArquivoEnvio, ArquivoRetorno)
      Select @p_Inscricao, @p_CadEquipe, @p_MarkID, @p_Situacao, @p_DataHoraConsulta, @p_ArquivoEnvio, @p_ArquivoRetorno;";
      _dbContext.Parametros.AddItem("@p_Inscricao", insc);
      _dbContext.Parametros.AddItem("@p_CadEquipe", idUser);
      _dbContext.Parametros.AddItem("@p_MarkID", markID);
      _dbContext.Parametros.AddItem("@p_Situacao", situacao);
      _dbContext.Parametros.AddItem("@p_DataHoraConsulta", DateTime.Now);
      _dbContext.Parametros.AddItem("@p_ArquivoEnvio", request);
      _dbContext.Parametros.AddItem("@p_ArquivoRetorno", response);
      return await _dbContext.SaveAsync(cmd, 0);
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
    public async Task<long> GetQueryCountCompany(long idCompany)
    {
      string cmd = @"Select Count(1) Total
      From LogSerasaConsultas L
      Join CadEquipe Ce on (Ce.Id = L.Fk_CadEquipe)
      Where (Ce.Fk_CadEmpresas = @p_CadEmpresas) and (L.Situacao = 1);";
      _dbContext.Parametros.AddItem("@p_CadEmpresas", idCompany);
      DataSet ds = await _dbContext.QueryAsync(cmd, 0);
      if (ds?.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        return ExtensionService.SafeConvert<long>(ds.Tables[0].Rows[0]["Total"]);

      return 0;
    }

    public async Task<List<LogSerasaModel>> QueryLogSerasa(string markID)
    {
      List<LogSerasaModel> list = new List<LogSerasaModel>();
      string cmd = "Select * From LogSerasaConsultas Where MarkID = @p_MarkID;";
      _dbContext.Parametros.AddItem("@p_MarkID", markID);
      DataSet ds = await _dbContext.QueryAsync(cmd, 0);

      if (ds?.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        foreach (DataRow row in ds.Tables[0].Rows)
        {
          list.Add(new LogSerasaModel
          {
            Inscricao = row.Field<string>("Inscricao") ?? "",
            MarkID = row.Field<string>("MarkID") ?? "",
            Situacao = ExtensionService.SafeConvert<byte>(row["Situacao"]),
            DataHoraConsulta = ExtensionService.SafeConvert<DateTime>(row["DataHoraConsulta"]),
            ArquivoEnvio = row.Field<string>("ArquivoEnvio") ?? "",
            ArquivoRetorno = row.Field<string>("ArquivoRetorno") ?? ""
          });
        }

      return list;
    }
    public async Task<string> HandleRequestsSerasa(string insc, string tel, string email, bool useLast)
    {
      long totalQuerys = await GetQueryCountCompany(Convert.ToInt64(AppIdentity.GetClaimValue("IdCompany")));
      long totalBuy = await _movConsultasCompradas.GetQueryQtdLimit(Convert.ToInt64(AppIdentity.GetClaimValue("IdCompany")));

      if (totalQuerys >= totalBuy)
        return "401";
      insc = ExtensionService.CleanSpecialChars(insc);
      if (useLast == true)
      {
        var lastQuery = await GetLastSerasaInsc(insc);
        if (lastQuery != null && lastQuery.Id > 0) // Verifica se a consulta já foi feita
        {
          if (CanQuery(lastQuery.DataHoraConsulta) == false)
            return lastQuery.MarkID;
        }
      }

      string markID = Guid.NewGuid().ToString();
      Dictionary<string, string> responses = new Dictionary<string, string>();
      string reqSTR = MontaRequisicao(insc); // Montando a string de requisição inicial
      string firstResponse = await RequestSerasa(markID, insc, reqSTR); // Obtendo a primeira resposta
      responses.Add(reqSTR, firstResponse); // Adicionando a primeira resposta

      if (CheckSearchAgain(firstResponse)) // Verifica se tem que repetir a consulta
      {
        string b49c = firstResponse.Substring(0, 400); // Extrai do retorno do consulta anterior o próximo request conforme o manual
        bool searchAgain = true; // Necessário para parar o loop
        while (searchAgain == true)
        {
          string newReesponse = await RequestSerasa(markID, insc, b49c); // Repete a requisição com o cabeçalho da última resposta
          if (newReesponse.Length > 399)
            b49c = newReesponse.Substring(0, 400); // Obtém para a próxima requisição
          responses.Add(b49c, newReesponse); // Adiciona a lista de respostas
          searchAgain = CheckSearchAgain(newReesponse); // Verifica se tem que repetir a busca, caso não finaliza o loop
        }
      }

      await ProcessResponses(markID, insc, tel, email, responses);
      return markID;
    }

    public async Task ProcessResponses(string markID, string insc, string tel, string email, Dictionary<string, string> responses)
    {
      var client = await _clienteService.GetByInscAsync(insc);
      long idUser = Convert.ToInt64(AppIdentity.GetClaimValue("IdUser"));
      bool isClientSave = false;

      foreach (var response in responses)
      {
        var entity = new MovCadClientesSerasaModel();
        entity.MarkID = markID;
        entity.CadEquipe.Id = idUser;

        if (client.Id > 0)
          entity.CadClientes = client;

        entity.CadClientes.Inscricao = insc;

        HandleSerasaResponse(entity, response.Key, response.Value);

        if (entity.CadClientes.Id == 0 && isClientSave == false)
        {
          entity.CadClientes.Nome = entity.Concentre.RegI100.Nome;
          entity.CadClientes.Telefone = tel;
          entity.CadClientes.Email = email;
          var isSave = await _clienteService.SimpleSave(entity.CadClientes);
          if (isSave is long)
          {
            entity.CadClientes.Id = (long)isSave;
            isClientSave = true;
          }
        }
        await SaveAsync(entity);
      }
    }

    public async Task ProcessMarkID(string markID)
    {
      var list = await QueryLogSerasa(markID);
      foreach (var item in list)
      {
        var responses = new Dictionary<string, string>();
        responses.Add(item.ArquivoEnvio, item.ArquivoRetorno);
        await ProcessResponses(markID, item.Inscricao, "", "", responses);
      }
    }
    public async Task<Concentre> GetConcentreMarkID(string markID)
    {
      var list = await QueryLogSerasa(markID);
      var cc = new Concentre();
      int people = 0;
      string insc = "";
      List<ArquivoRetornoConcentre> mapeado = new List<ArquivoRetornoConcentre>();

      foreach (var item in list)
      {
        people = item.Inscricao.Length == 11 ? 1 : 2;
        insc = item.Inscricao;
        ArquivoRetornoConcentre retorno = new ArquivoRetornoConcentre();
        mapeado = retorno.MapearArquivo(item.ArquivoRetorno, mapeado);
      }
      cc.Processar(people, mapeado);
      cc.RegI100.Inscricao = insc;
      cc.DataConsulta = list[0].DataHoraConsulta;
      return cc;
    }

    public void HandleSerasaResponse(MovCadClientesSerasaModel entity, string send, string response)
    {
      ArquivoRetornoConcentre retorno = new ArquivoRetornoConcentre();
      List<ArquivoRetornoConcentre> concentre = retorno.MapearArquivo(response);
      TrataRetorno(concentre, entity);

      entity.DataHoraConsulta = DateTime.Now;
      entity.ArquivoRetorno = response;
      entity.ArquivoEnvio = send;

      entity.TotalNegativacao = entity.Concentre.TotalNegativacao;
    }
    public void TotalizarNegativacao(MovCadClientesSerasaModel entity)
    {
      try
      {
        entity.TotalNegativacao = entity.AcoesJudiciais + entity.Concordatas + entity.DividasVencidas + entity.PartFalencias + entity.PendenciasBancariasRefin + entity.PendenciasComerciaisPefin + entity.Protestos + entity.AnotacoesSPC;
      }
      catch (Exception ex)
      {
      }
    }
    public int ContaQuantidadeRegistros(List<ArquivoRetornoConcentre> concentre, string b)
    {
      try
      {
        return concentre.Where(c => c.Registro == b).ToList().Count;
      }
      catch
      {
        return 0;
      }
    }
    public void TrataRetorno(List<ArquivoRetornoConcentre> concentre, MovCadClientesSerasaModel entity)
    {
      // Lendo status
      BuscaStatus(concentre.FirstOrDefault(c => c.Registro == "T999"), entity);
      // Buscando valores de resumo
      BuscaResumo(concentre, entity);
    }
    public void BuscaResumo(List<ArquivoRetornoConcentre> concentre, MovCadClientesSerasaModel entity)
    {
      // Totalizando os cheques sem fundos, tem que ser feito
      entity.ChequesSemFundos = ContaQuantidadeRegistros(concentre, "I170");

      // Removendo valores com 0
      //List<ArquivoRetornoConcentre> details = concentre.Where(c => c.Subtipo != "00").ToList();

      int people = entity.CadClientes.Inscricao.Length == 11 ? 1 : 2;

      entity.Concentre.Processar(people, concentre);
      entity.Score = entity.Concentre.RegF900Score.Score;
      entity.Concentre.DataConsulta = entity.DataHoraConsulta;

      List<ArquivoRetornoConcentre> list = concentre.ToList();
      foreach (ArquivoRetornoConcentre r in list)
      {
        if (r.Registro == "I220")
          entity.PendenciasComerciaisPefin = r.Valor;
        else if (r.Registro == "I140")
          entity.PendenciasBancariasRefin = r.Valor;
        else if (r.Registro == "I160" || r.Registro == "I170")
          entity.ChequesSemFundos += r.Valor; // procurar
        else if (r.Registro == "I110")
          entity.Protestos = r.Valor;
        else if (r.Registro == "I120")
          entity.AcoesJudiciais = r.Valor;
        else if (r.Registro == "I150")
          entity.PartFalencias = r.Valor;
        else if (r.Registro == "I130")
          entity.Concordatas = r.Valor;
        else if (r.Registro == "I420")
          entity.Score = Convert.ToInt32(r.Valor);
        else if (r.Registro == "I230")
          entity.DividasVencidas = r.Valor;
        else if (r.Registro == "F900")
          entity.AnotacoesSPC += r.Valor;
      }

      System.Diagnostics.Debug.WriteLine("Acabou...");
    }
    public void BuscaStatus(ArquivoRetornoConcentre T999, MovCadClientesSerasaModel entity)
    {
      try
      {
        if (T999.Texto.Contains("PROCESSO ENCERRADO NORMALMENTE"))
          entity.StatusConsulta = 1; // OK
        else
          entity.StatusConsulta = 0; // Falha
      }
      catch (Exception ex)
      {
      }

    }
  }
}