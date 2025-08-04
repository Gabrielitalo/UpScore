using RecomeceAPI.Interfaces;
using RecomeceAPI.Models;
using RecomeceAPI.Models.Serasa.REST;
using RecomeceAPI.Models.Serasa.REST.PJ;
using RecomeceAPI.Server.Auth;
using RecomeceAPI.Services.Common;
using System;
using System.Net.Http.Headers;
using System.Security.Policy;

namespace RecomeceAPI.Services.Serasa
{
  public class RestSerasaService
  {
    private readonly HttpClient _httpClient;
    private readonly LogConsultasService _logConsultasService;
    private readonly MovContaCorrenteService _movContaCorrenteService;
    private readonly CadConsultasService _cadConsultasService;
    private readonly ISerasaTokenService _tokenService;
    private readonly IConfiguration _config;
    private readonly HandleCatchService _handleCatchService;

    public RestSerasaService(HttpClient httpClient, ISerasaTokenService tokenService, IConfiguration config, LogConsultasService logConsultasService, MovContaCorrenteService movContaCorrenteService, CadConsultasService cadConsultasService, HandleCatchService handleCatchService)
    {
      _httpClient = httpClient;
      _tokenService = tokenService;
      _config = config;
      _logConsultasService = logConsultasService;
      _movContaCorrenteService = movContaCorrenteService;
      _cadConsultasService = cadConsultasService;
      _handleCatchService = handleCatchService;
    }
    public Dictionary<long, string> Querys(int tipo)
    {
      var querys = new Dictionary<long, string>();
      if (tipo == 1)
      {
        querys.Add(1, "optionalFeatures=COMPROMETIMENTO_RENDA");
        querys.Add(2, "optionalFeatures=ANOTACOES_CONSULTAS_SPC,COMPROMETIMENTO_RENDA");
        querys.Add(3, "optionalFeatures=ANOTACOES_CONSULTAS_SPC,COMPROMETIMENTO_RENDA");
      }
      else
      {
        querys.Add(1, "optionalFeatures=ANOTACOES_CONSULTAS_SPC,FATURAMENTO_ESTIMADO_POSITIVO"); // Verificar isso aqui
        querys.Add(2, "optionalFeatures=ANOTACOES_CONSULTAS_SPC");
        querys.Add(3, "optionalFeatures=ANOTACOES_CONSULTAS_SPC");
        querys.Add(4, "optionalFeatures=ANOTACOES_CONSULTAS_SPC");
      }
      return querys;
    }
    public string GetProduct(string inscricao)
    {
      return inscricao.Length == 11 ? "RELATORIO_AVANCADO_TOP_SCORE_PF_PME" : "RELATORIO_AVANCADO_PJ_PME";
    }
    public string GetSource(string inscricao)
    {
      return inscricao.Length == 11 ? "person-information-report/v1/creditreport" : "business-information-report/v1/reports";
    }
    public async Task<RelatorioAvancadoPjModel> GetPJ(string inscricao, long consultaId, string markId)
    {
      var consulta = await _cadConsultasService.GetByIdAsync(consultaId);
      var model = new RelatorioAvancadoPjModel();
      var token = await _tokenService.GetTokenAsync();

      int personType = inscricao.Length == 1 ? 1 : 2;

      var base_Url = _config["Serasa:Base_Url"];

      if (consulta == null)
        throw new Exception($"Modelo {consultaId} não encontrado");

      //var url = $"{base_Url}/credit-services/{GetSource(inscricao)}?reportName={GetProduct(inscricao)}&{consulta.ProductURL}";
      var url = $"{base_Url}/credit-services/{consulta.ProductURL}";

      var request = new HttpRequestMessage(HttpMethod.Get, url);

      request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
      request.Headers.Add("X-Document-Id", inscricao);
      request.Headers.Add("X-Retailer-Document-Id", "52028698000157");

      var response = await _httpClient.SendAsync(request);
      var responseStr = await response.Content.ReadAsStringAsync();

      if (!response.IsSuccessStatusCode)
      {
        _handleCatchService.HandleCatch($"Erro ao consultar relatório: inscricao: {inscricao} - consultaId: {consultaId} - markId: {markId} - statusCode: {response.StatusCode} -  \r\n \r\n {responseStr}");
        await LogQuery(consultaId, inscricao, 0, 0, url, responseStr, markId, 0);
        return model;
      }

      if (responseStr.Contains("SpikeArrest"))
      {
        _handleCatchService.HandleCatch($"Erro ao consultar relatório: inscricao: {inscricao} - consultaId: {consultaId} - markId: {markId} -  \r\n \r\n {responseStr}");
        await LogQuery(consultaId, inscricao, 0, 0, url, responseStr, markId, 0);
        return model;
      }

      try
      {
        HandleJsonService.SetByJson<RelatorioAvancadoPjModel>(responseStr, model);
        if (model.reports.Count > 0)
        {
          if (!string.IsNullOrEmpty(model.reports[0].registration.companyName))
          {
            model.reports[0].identificationReport.companyName = model.reports[0].registration.companyName;
            model.reports[0].identificationReport.documentNumber = model.reports[0].registration.companyDocument;
          }
        }
        model.CalcDebit();
        await LogQuery(consultaId, inscricao, (decimal)model.reports[0].score.score, model.reports[0].debitTotal, url, responseStr, markId);
      }
      catch (Exception ex)
      {
        string msg = $"consultaId: {consultaId} - inscricao: {inscricao}, \r\n\r\n responseStr: {responseStr}";
        _handleCatchService.HandleCatch(ex);
        await LogQuery(consultaId, inscricao, 0, 0, url, responseStr, markId, 0);
      }

      return model;
    }
    public async Task<RelatorioAvancadoPfModel> GetPF(string inscricao, long consultaId, string markId)
    {
      var consulta = await _cadConsultasService.GetByIdAsync(consultaId);
      var model = new RelatorioAvancadoPfModel();
      var token = await _tokenService.GetTokenAsync();
      var base_Url = _config["Serasa:Base_Url"];
      int personType = inscricao.Length == 1 ? 1 : 2;
      var query = Querys(personType).FirstOrDefault(f => f.Key == consultaId).Value;

      if (consulta == null)
        throw new Exception($"Modelo {consultaId} não encontrado");

      //var url = $"{base_Url}/credit-services/{GetSource(inscricao)}?reportName={GetProduct(inscricao)}&{consulta.ProductURL}";
      var url = $"{base_Url}/credit-services/{consulta.ProductURL}";

      var request = new HttpRequestMessage(HttpMethod.Get, url);

      request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
      request.Headers.Add("X-Document-Id", inscricao);
      request.Headers.Add("X-Retailer-Document-Id", "52028698000157");

      var response = await _httpClient.SendAsync(request);
      var responseStr = await response.Content.ReadAsStringAsync();

      if (!response.IsSuccessStatusCode)
      {
        _handleCatchService.HandleCatch($"Erro ao consultar relatório: inscricao: {inscricao} - consultaId: {consultaId} - markId: {markId} - statusCode: {response.StatusCode} -  \r\n \r\n {responseStr}");
        await LogQuery(consultaId, inscricao, 0, 0, url, responseStr, markId, 0);
        return model;
      }

      if (responseStr.Contains("SpikeArrest"))
      {
        _handleCatchService.HandleCatch($"Erro ao consultar relatório: inscricao: {inscricao} - consultaId: {consultaId} - markId: {markId} -  \r\n \r\n {responseStr}");
        await LogQuery(consultaId, inscricao, 0, 0, url, responseStr, markId, 0);
        return model;
      }


      try
      {
        HandleJsonService.SetByJson<RelatorioAvancadoPfModel>(responseStr, model);
        model.CalcDebit();
        await LogQuery(consultaId, inscricao, (decimal)model.reports[0].score.score, model.reports[0].debitTotal, url, responseStr, markId);
      }
      catch (Exception ex)
      {
        string msg = $"consultaId: {consultaId} - inscricao: {inscricao}, \r\n\r\n responseStr: {responseStr}";
        _handleCatchService.HandleCatch(ex);
        await LogQuery(consultaId, inscricao, 0, 0, url, responseStr, markId, 0);
      }
      return model;
    }
    public async Task LogQuery(long consultaId, string inscricao, decimal score, double totalNegativacao, string request, string response, string markId, int situacao = 1)
    {
      var queryValue = await _cadConsultasService.GetByIdAsync(consultaId);

      var model = new LogConsultasModel
      {
        Id = Guid.NewGuid().ToString(),
        Inscricao = inscricao,
        Fk_CadConsultas = consultaId,
        Fk_CadEquipe = AppIdentity.GetIdUserValue(),
        Score = score,
        TotalNegativacao = (decimal?)totalNegativacao,
        ArquivoEnvio = request,
        MarkID = markId,
        Situacao = situacao,
        ArquivoRetorno = response
      };

      if (situacao == 1)
        await _movContaCorrenteService.AddDebitFromQuery(queryValue.Venda, model.Id);
      await _logConsultasService.SaveAsync(model);
    }
    public async Task<object> Get(string inscricao, long consultaId)
    {
      //if (inscricao.Length == 11)
      //  return await GetPF(inscricao, consultaId);
      //else
      //  return await GetPJ(inscricao, consultaId);
      return "";
    }
  }
}
