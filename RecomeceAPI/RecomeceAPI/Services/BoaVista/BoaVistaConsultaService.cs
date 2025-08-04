using DocumentFormat.OpenXml.EMMA;
using RecomeceAPI.Interfaces;
using RecomeceAPI.Models;
using RecomeceAPI.Server.Auth;
using RecomeceAPI.Services.Common;
using System.Security.Policy;

namespace RecomeceAPI.Services.BoaVista
{
  public class BoaVistaConsultaService
  {
    private readonly HttpClient _httpClient;
    private readonly LogConsultasService _logConsultasService;
    private readonly MovContaCorrenteService _movContaCorrenteService;
    private readonly CadConsultasService _cadConsultasService;
    private readonly IConfiguration _config;
    private readonly CadClientesService _clienteService;

    public BoaVistaConsultaService(HttpClient httpClient, IConfiguration config, LogConsultasService logConsultasService, MovContaCorrenteService movContaCorrenteService, CadConsultasService cadConsultasService, CadClientesService clienteService)
    {
      _httpClient = httpClient;
      _config = config;
      _logConsultasService = logConsultasService;
      _movContaCorrenteService = movContaCorrenteService;
      _cadConsultasService = cadConsultasService;
      _clienteService = clienteService;
    }

    public async Task<string> GetPJ(string inscricao, long consultaId, string markId)
    {
      var consulta = await _cadConsultasService.GetByIdAsync(consultaId);

      if (consulta == null)
        throw new Exception($"Modelo {consultaId} não encontrado");

      var base_Url = _config["BoaVista:Base_Url_PJ"] ?? "";
      var user = _config["BoaVista:User"];
      var password = _config["BoaVista:Password"];

      var content = new StringContent($"<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>\r\n<defineRiscoPositivoContratoEntradaXml xmlns=\"http://boavistaservicos.com.br/define/risco/positivo/entrada\">\r\n    <usuario>{user}</usuario>\r\n    <senha>{password}</senha>\r\n    <cnpj>{inscricao}</cnpj>\r\n    <chequeBlocoInferior>\r\n        <CMC7Campo1>0000000000</CMC7Campo1>\r\n        <CMC7Campo2>0000000000</CMC7Campo2>\r\n        <CMC7Campo3>0000000000000</CMC7Campo3>\r\n        <CMC7TotalCheque>00000</CMC7TotalCheque>\r\n        <CMC7Valor>00000000000000</CMC7Valor>\r\n    </chequeBlocoInferior>\r\n    <opcionais>\r\n        <opcional>anvisa</opcional>\r\n        <opcional>decisao</opcional>\r\n        <opcional>empresasMesmoEndereco</opcional>\r\n        <opcional>faturamento</opcional>\r\n        <opcional>folhaCheque</opcional>\r\n        <opcional>limiteCredito</opcional>\r\n        <opcional>participacoes</opcional>\r\n        <opcional>quadroSocial</opcional>\r\n    </opcionais>\r\n    <extras>\r\n        <extra>administradores</extra>\r\n        <extra>chequesDevInfUsuario</extra>\r\n        <extra>chequesSemFundo</extra>\r\n        <extra>chequesSustado</extra>\r\n        <extra>participacoesEmpresa</extra>\r\n        <extra>participacoesSocio</extra>\r\n        <extra>pendenciasRestricoes</extra>\r\n        <extra>protestos</extra>\r\n        <extra>quantidadeDeConsultas</extra>\r\n        <extra>socios</extra>\r\n    </extras>\r\n</defineRiscoPositivoContratoEntradaXml>", null, "application/xml");
    
      var request = new HttpRequestMessage(HttpMethod.Post, base_Url);
      request.Content = content;

      var response = await _httpClient.SendAsync(request);
      var responseStr = await response.Content.ReadAsStringAsync();

      var score = ExtensionService.GetXmlValueByPath(responseStr, "http://boavistaservicos.com.br/define/positivo/xml/score", "score/score_pj/score");
      var debit = ExtensionService.GetXmlValueByPath(responseStr, "http://boavistaservicos.com.br/define/positivo/xml/pendencias_restricoes", "pendencias_restricoes/valor");
      var name = ExtensionService.GetXmlValueByPath(responseStr, "http://boavistaservicos.com.br/define/positivo/xml/identificacao_basico", "identificacao_basico/razao_social");
      
      try
      {
        await LogQuery(consultaId, inscricao, score ?? "0", debit ?? "0", base_Url, responseStr, markId);
      }
      catch
      {
        await LogQuery(consultaId, inscricao, "0", "0", base_Url, responseStr, markId);
      }

      try
      {
        await SaveClient(inscricao, name);
      }
      catch
      {

      }
      return responseStr;
    }

    public async Task SaveClient(string inscricao, string nome)
    {
      var client = await _clienteService.GetByInscAsync(inscricao);
      if(client.Id == 0)
      {
        var entity = new CadClientesModel { Inscricao = inscricao, Nome = nome, Cidades = new CidadesModel { Id = 5562 } }; 
        await _clienteService.SimpleSave(entity);
      }
    }

    public async Task<string> GetPF(string inscricao, long consultaId, string markId)
    {
      var consulta = await _cadConsultasService.GetByIdAsync(consultaId);

      if (consulta == null)
        throw new Exception($"Modelo {consultaId} não encontrado");

      var base_Url = _config["BoaVista:Base_Url_PF"] ?? "";
      var user = _config["BoaVista:User"];
      var password = _config["BoaVista:Password"];

      var content = new StringContent($"<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>\r\n<acertaPositivoContratoEntrada xmlns=\"http://boavistaservicos.com.br/familia/acerta/positivo/pf\">\r\n<usuario>{user}</usuario>\r\n    <senha>{password}</senha>\r\n<cpf>{inscricao}</cpf>\r\n<tipoCredito>CD</tipoCredito>\r\n</acertaPositivoContratoEntrada>", null, "application/xml");


      var request = new HttpRequestMessage(HttpMethod.Post, base_Url);
      request.Content = content;

      var response = await _httpClient.SendAsync(request);
      var responseStr = await response.Content.ReadAsStringAsync();

      var score = ExtensionService.GetXmlValueByPath(responseStr, "http://boavistaservicos.com.br/familia/acerta/positivo/pf/score_classificacao_varios_modelos", "score_classificacao_varios_modelos/score");
      var debit = ExtensionService.GetXmlValueByPath(responseStr, "http://boavistaservicos.com.br/familia/acerta/positivo/pf/resumo_ocorrencias_de_debitos", "resumo_ocorrencias_de_debitos/valorAcomulado");
      var name = ExtensionService.GetXmlValueByPath(responseStr, "http://boavistaservicos.com.br/familia/acerta/positivo/pf/identificacao", "identificacao/nome");
      
      try
      {
        await LogQuery(consultaId, inscricao, score ?? "0", debit ?? "0", base_Url, responseStr, markId);
      }
      catch
      {
        await LogQuery(consultaId, inscricao, "0", "0", base_Url, responseStr, markId);
      }

      try
      {
        await SaveClient(inscricao, name);
      }
      catch
      {

      }
      return responseStr;
    }

    public async Task Handle(string inscricao, long consultaId, string markId)
    {
      string response = "";
      if(inscricao.Length == 11)
        response = await GetPF(inscricao, consultaId, markId);
      else
        response = await GetPJ(inscricao, consultaId, markId);
    }

    public async Task LogQuery(long consultaId, string inscricao, string score, string totalNegativacao, string request, string response, string markId)
    {
      var queryValue = await _cadConsultasService.GetByIdAsync(consultaId);

      var model = new LogConsultasModel
      {
        Id = Guid.NewGuid().ToString(),
        Inscricao = inscricao,
        Fk_CadConsultas = consultaId,
        Fk_CadEquipe = AppIdentity.GetIdUserValue(),
        Score = ExtensionService.SafeConvert<decimal>(ExtensionService.ConvertBrlToInvariant(score), 0),
        TotalNegativacao = ExtensionService.SafeConvert<decimal>(ExtensionService.ConvertBrlToInvariant(totalNegativacao), 0),
        ArquivoEnvio = request,
        Situacao = 1,
        MarkID = markId,
        ArquivoRetorno = response
      };

      await _movContaCorrenteService.AddDebitFromQuery(queryValue.Venda, model.Id);
      await _logConsultasService.SaveAsync(model);
    }
  }
}
