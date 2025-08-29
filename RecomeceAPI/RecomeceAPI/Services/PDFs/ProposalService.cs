using DocumentFormat.OpenXml.Office2010.Excel;
using MySqlX.XDevAPI;
using RecomeceAPI.Models;
using RecomeceAPI.Models.Serasa.REST;
using RecomeceAPI.Models.Serasa.REST.PJ;
using RecomeceAPI.Server.Auth;
using RecomeceAPI.Services.Common;
using RecomeceAPI.Services.Serasa;

namespace RecomeceAPI.Services.PDFs
{
  public class ProposalService
  {
    private readonly HandleCatchService _handleCatchService;
    private readonly MovPropostasBeneficiariosService _movPropostasBeneficiariosService;
    private readonly MovCadClientesSerasaService _movCadClientesSerasaService;
    private readonly MovPropostasService _movPropostasService;
    private readonly CadProdutosFaixasPagamentoService _cadProdutosFaixasPagamentoService;
    private readonly CadEmpresasService _cadEmpresasService;
    private readonly CadProdutosFaixasService _cadProdutosFaixasService;
    private readonly CadClientesService _cadClientesService;
    private readonly RestSerasaService _restSerasaService;
    private readonly CadProdutosService _cadProdutosService;
    private readonly LogConsultasService _logConsultasService;
    private readonly CadConsultasService _cadConsultasService;
    private readonly MovContaCorrenteService _movContaCorrenteService;

    public ProposalService(HandleCatchService handleCatchService, MovPropostasBeneficiariosService movPropostasBeneficiariosService,
      MovCadClientesSerasaService movCadClientesSerasaService, MovPropostasService movPropostasService,
      CadProdutosFaixasPagamentoService cadProdutosFaixasPagamentoService, CadEmpresasService cadEmpresasService,
      CadProdutosFaixasService cadProdutosFaixasService, CadClientesService cadClientesService, RestSerasaService restSerasaService, CadProdutosService cadProdutosService, 
        LogConsultasService logConsultasService, CadConsultasService cadConsultasService, MovContaCorrenteService movContaCorrenteService)
    {
      _handleCatchService = handleCatchService;
      _movPropostasBeneficiariosService = movPropostasBeneficiariosService;
      _movCadClientesSerasaService = movCadClientesSerasaService;
      _movPropostasService = movPropostasService;
      _cadProdutosFaixasPagamentoService = cadProdutosFaixasPagamentoService;
      _cadEmpresasService = cadEmpresasService;
      _cadProdutosFaixasService = cadProdutosFaixasService;
      _restSerasaService = restSerasaService;
      _cadClientesService = cadClientesService;
      _cadProdutosService = cadProdutosService;
      _logConsultasService = logConsultasService;
      _cadConsultasService = cadConsultasService;
      _movContaCorrenteService = movContaCorrenteService;
    }

    public async Task<byte[]> GenerateProposal(long proposalId)
    {
      var pdfBytes = new byte[0];
      var company = await _cadEmpresasService.GetCompanyUserLogged();

      if (company.WhiteLabelConfig.ModeloProposta.ToLower() == "upscore" || company.WhiteLabelConfig.ModeloProposta.ToLower() == "recomece")
      {
        UpScoreProposal pdf = new UpScoreProposal(_movPropostasBeneficiariosService, _movCadClientesSerasaService, _movPropostasService, _cadProdutosFaixasPagamentoService, _logConsultasService);
        var generated = await pdf.GenerateProposalPDF(proposalId, company.WhiteLabelConfig.ModeloProposta);
        pdfBytes = generated.Item1;
      }
      return pdfBytes;
    }

    public async Task<CadClientesModel> CreateClientFromSerasa(string name, string inscricao)
    {
      var isExists = await _cadClientesService.GetByInscAsync(inscricao);
      if (isExists.Id > 0)
        return isExists;

      CadClientesModel client = new CadClientesModel();
      client.Inscricao = inscricao;
      client.Nome = name;
      client.Cidades.Id = 5562;
      client.CadEquipe.Id = AppIdentity.GetIdUserValue();

      var isSave = await _cadClientesService.SaveAsync(client);
      if (isSave is long)
        client.Id = (long)isSave;
      return client;
    }

    public async Task<long> CreateProposal(MovPropostasModel proposal, decimal debitTotal)
    {
      var total = await _cadProdutosFaixasService.GetProductDebit(proposal.CadProdutos.Id, debitTotal);
      proposal.NumeroContrato = _movPropostasService.NewContractNumber();
      proposal.DataHoraCadastro = DateTime.Now;
      proposal.Observacao = "";
      proposal.PercDesconto = 0;
      proposal.Situacao = 1; // Gerado
      proposal.Termometro = 1;
      proposal.ValorAprovado = total.ValorServico;
      proposal.ValorContrato = total.ValorServico;
      proposal.ValorDivida = debitTotal;
      proposal.ValorEntrada = 0;

      var isSave = await _movPropostasService.SaveAsync(proposal);
      if (isSave is long)
        return (long)isSave;
      else
        return 0;
    }
    public async Task AddClient(CadProdutosModel product, string markId, long proposalId, long clientId, decimal score, decimal debitTotal)
    {
      var total = await _cadProdutosFaixasService.GetProductDebit(product.Id, debitTotal);

      var entity = new MovPropostasBeneficiariosModel
      {
        Id = 0,
        CadClientes = new CadClientesModel { Id = clientId },
        MovPropostas = new MovPropostasModel { Id = proposalId },
        Score = Convert.ToInt32(score),
        ValorDivida = debitTotal,
        ValorContrato = total.ValorServico,
        Observacao = "",
        Tipo = 1,
        MarkID = markId,
      };
      await _movPropostasBeneficiariosService.SaveAsync(entity);
    }
    public bool CanQuery(DateTime queryDate)
    {
      return queryDate < DateTime.Now.AddDays(-60);
    }
    public async Task<object> ProposalNew(long productID, long proposalId, string inscricao, bool forceNew = false)
    {
      inscricao = ExtensionService.CleanSpecialChars(inscricao);
      int personType = inscricao.Length == 11 ? 1 : 2;
      int consultaId = inscricao.Length == 11 ? 3 : 4; 

      var product = await _cadProdutosService.GetByIdAsync(productID);
      if(product.TipoConsulta == 1)
        consultaId = personType == 1 ? 3 : 4; // Relatório Avançado
      else if (product.TipoConsulta == 2)
        consultaId = personType == 1 ? 7 : 8; // Relatório Básico

      var queryValue = await _cadConsultasService.GetByIdAsync(consultaId);
      var currentValue = await _movContaCorrenteService.GetTotal();

      if(queryValue.Venda > currentValue)
      {
        return NotificationService.Validation("Você não possui saldo disponível para esta consulta!", new { redirect = "saldo"});
      }


      decimal debitTotal = 0, score = 0;
      MovPropostasModel model = new MovPropostasModel { CadProdutos = product, CadEquipe = new CadEquipeModel { Id = AppIdentity.GetIdUserValue() } };
      CadClientesModel client = new CadClientesModel();
      string markId = Guid.NewGuid().ToString();

      var queryExists = await _logConsultasService.GetByInscricaoAsync(inscricao);

      // Forçando uma nova busca caso a consulta tenha mais de 60 dias
      forceNew = CanQuery(queryExists.DataHoraConsulta ?? DateTime.Now);

      if (personType == 1)
      {
        var queryPF = new RelatorioAvancadoPfModel();
        if (!string.IsNullOrEmpty(queryExists.Id) && forceNew == false)
        {
          HandleJsonService.SetByJson<RelatorioAvancadoPfModel>(queryExists.ArquivoRetorno, queryPF);
          queryPF.CalcDebit();
        }
        else
          queryPF = await _restSerasaService.GetPF(inscricao, queryValue.Id, markId);

        if (queryPF.reports.Count == 0)
        {
          return NotificationService.Validation("Houve um problema para realizar a consulta, tente novamente em alguns instantes");
        }
        client = await CreateClientFromSerasa(queryPF.reports[0].registration.consumerName, queryPF.reports[0].registration.documentNumber);
        model.CadClientes = client;
        debitTotal = (decimal)queryPF.debitTotal;
        score = (decimal)queryPF.reports[0].score.score;
      }
      else if (personType == 2)
      {
        var queryPJ = new RelatorioAvancadoPjModel();
        if (!string.IsNullOrEmpty(queryExists.Id) && forceNew == false)
        {
          HandleJsonService.SetByJson<RelatorioAvancadoPjModel>(queryExists.ArquivoRetorno, queryPJ);
          queryPJ.CalcDebit();
        }
        else
          queryPJ = await _restSerasaService.GetPJ(inscricao, queryValue.Id, markId);


        if (queryPJ.reports.Count == 0)
        {
          return NotificationService.Validation("Houve um problema para realizar a consulta, tente novamente em alguns instantes");
        }

        client = await CreateClientFromSerasa(queryPJ.reports[0].identificationReport.companyName, queryPJ.reports[0].identificationReport.documentNumber);
        model.CadClientes = client;
        debitTotal = (decimal)queryPJ.debitTotal;
        score = (decimal)queryPJ.reports[0].score.score;
      }

      if (proposalId == 0)
        proposalId = await CreateProposal(model, debitTotal);

      if(!string.IsNullOrEmpty(queryExists.MarkID))
        markId = queryExists.MarkID;  

      await AddClient(product, markId, proposalId, client.Id, score, debitTotal);
      await _movPropostasService.UpdateMovPropostas(proposalId);
      return proposalId;
    }
    public async Task<object> NewQuerySerasa(long productId, string inscricao)
    {
      inscricao = ExtensionService.CleanSpecialChars(inscricao);
      int personType = inscricao.Length == 11 ? 1 : 2;

      var queryValue = await _cadConsultasService.GetByIdAsync(productId);
      var currentValue = await _movContaCorrenteService.GetTotal();
      string markId = Guid.NewGuid().ToString();

      if (queryValue.Venda > currentValue)
      {
        return NotificationService.Validation("Você não possui saldo disponível para esta consulta!");
      }

      if (personType == 1)
      {
        var query = await _restSerasaService.GetPF(inscricao, queryValue.Id, markId);
        await CreateClientFromSerasa(query.reports[0].registration.consumerName, query.reports[0].registration.documentNumber);
        return query;
      }
      else if (personType == 2)
      {
        var query = await _restSerasaService.GetPJ(inscricao, queryValue.Id, markId);
        await CreateClientFromSerasa(query.reports[0].identificationReport.companyName, query.reports[0].identificationReport.documentNumber);
        return query;
      }

      return false;
    }
  }
}
