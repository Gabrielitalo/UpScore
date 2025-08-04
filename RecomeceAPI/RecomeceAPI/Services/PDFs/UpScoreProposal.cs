using PdfSharpCore.Pdf.IO;
using PdfSharpCore.Pdf;
using QuestPDF.Fluent;
using RecomeceAPI.Services.Common;
using RecomeceAPI.Models;
using RecomeceAPI.Models.Serasa.REST;
using RecomeceAPI.Models.Serasa.REST.PJ;

namespace RecomeceAPI.Services.PDFs
{
  public class UpScoreProposal
  {
    private readonly MovPropostasBeneficiariosService _movPropostasBeneficiariosService;
    private readonly MovCadClientesSerasaService _movCadClientesSerasaService;
    private readonly MovPropostasService _movPropostasService;
    private readonly CadProdutosFaixasPagamentoService _cadProdutosFaixasPagamentoService;
    private readonly LogConsultasService _logConsultasService;

    public UpScoreProposal(MovPropostasBeneficiariosService movPropostasBeneficiariosService, MovCadClientesSerasaService movCadClientesSerasaService, MovPropostasService movPropostasService, CadProdutosFaixasPagamentoService cadProdutosFaixasPagamentoService, LogConsultasService logConsultasService)
    {
      _movPropostasBeneficiariosService = movPropostasBeneficiariosService;
      _movCadClientesSerasaService = movCadClientesSerasaService;
      _movPropostasService = movPropostasService;
      _cadProdutosFaixasPagamentoService = cadProdutosFaixasPagamentoService;
      _logConsultasService = logConsultasService;
    }

    public class ProposalConfigs
    {
      public string Name { get; set; } = string.Empty;
      public string Path { get; set; } = string.Empty;
      public string GridHeaderColor { get; set; } = string.Empty;
      public int PageLimit { get; set; } = 6;
      public int PageStartBeneficiarios { get; set; } = 6;
    }

    public List<ProposalConfigs> ProposalConfigsList()
    {
      var list = new List<ProposalConfigs>();
      list.Add(new ProposalConfigs { Name = "upscore", PageLimit = 5, PageStartBeneficiarios = 6, GridHeaderColor = "orange", Path = "PropostaModeloUpScore.pdf" });
      list.Add(new ProposalConfigs { Name = "recomece", PageLimit = 2, PageStartBeneficiarios = 2, GridHeaderColor = "blue", Path = "PropostaModeloRecomece.pdf" });
      return list;
    }

    public async Task<(byte[], MovPropostasModel)> GenerateProposalPDF(long proposalId, string proposalModel)
    {
      //proposalModel = "upscore";
      List<byte[]> pags = new List<byte[]>();
      var proposal = await _movPropostasService.GetByIdAsync(proposalId);
      var proposalConfig = ProposalConfigsList().FirstOrDefault(d => d.Name == proposalModel);

      var beneficiarios = await _movPropostasBeneficiariosService.GetAllByProposal(proposalId);
      var debitTotal = _movPropostasBeneficiariosService.TotalDivida(beneficiarios);
      var product = await _movPropostasBeneficiariosService._cadProdutosFaixasService.GetProductDebit(proposal.CadProdutos.Id, debitTotal);
      var range = await _cadProdutosFaixasPagamentoService.GetRangeByProduct(product.Id);

      foreach (var item in beneficiarios)
      {
        var query = await _logConsultasService.GetByMarkId(item.MarkID);
        var questPdf = new UpScoreBeneficiario();
        questPdf.PersonType = item.CadClientes.Inscricao.Length == 11 ? 1 : 2;
        questPdf.GridHeaderColor = proposalConfig?.GridHeaderColor ?? "blue";
        questPdf.TotalDivida = debitTotal;
        questPdf.MovPropostas = proposal;
        questPdf.CadProdutosFaixasPagamento = range;
        if(questPdf.PersonType == 1)
        {
          questPdf.RelatoroAvancadoPf = HandleJsonService.SetByJson<RelatorioAvancadoPfModel>(query.ArquivoRetorno, questPdf.RelatoroAvancadoPf);
          //questPdf.RelatoroAvancadoPf.reports[0].CalcDebit();
          questPdf.RelatoroAvancadoPf.CalcDebit();
        }
        else
        {
          questPdf.RelatoroAvancadoPj = HandleJsonService.SetByJson<RelatorioAvancadoPjModel>(query.ArquivoRetorno, questPdf.RelatoroAvancadoPj);
          questPdf.RelatoroAvancadoPj.CalcDebit();
        }


        //questPdf.Concentre = await _movCadClientesSerasaService.GetConcentreMarkID(item.MarkID);

        byte[] novaPaginaBytes = questPdf.GeneratePdf();
        pags.Add(novaPaginaBytes);
      }

      var pagamentos = new UpScorePagamentos();
      pagamentos.GridHeaderColor = proposalConfig?.GridHeaderColor ?? "blue";
      pagamentos.MovPropostasBeneficiarios = beneficiarios;
      pagamentos.CadProdutosFaixasPagamento = range;
      pagamentos.MovPropostas = proposal;
      byte[] pagtos = pagamentos.GeneratePdf();

      string path = Path.Combine(Directory.GetCurrentDirectory(), "Docs", proposalConfig?.Path ?? "PropostaModeloUpScore.pdf");

      byte[] original = File.ReadAllBytes(path);

      var noFinal = new List<byte[]> { pagtos };

      var final = InsertPagesAtPositions(proposalConfig, original, pags, noFinal);
      //File.WriteAllBytes(edited, final);
      return (final, proposal);
    }

    public static byte[] InsertPagesAtPositions(
      ProposalConfigs proposalConfig,
  byte[] existingPdfBytes,
  List<byte[]> insertAfterPage6,
  List<byte[]> insertAtEnd)
    {
      using var outputStream = new MemoryStream();
      using var streamOriginal = new MemoryStream(existingPdfBytes);

      var docOriginal = PdfReader.Open(streamOriginal, PdfDocumentOpenMode.Import);
      var docFinal = new PdfDocument();

      // 1. Copia páginas 0 até 6 (inclusive)
      for (int i = 0; i <= proposalConfig.PageLimit && i < docOriginal.PageCount; i++)
        docFinal.AddPage(docOriginal.Pages[i]);

      // 2. Insere páginas logo após a página 6
      foreach (var pageBytes in insertAfterPage6)
      {
        using var stream = new MemoryStream(pageBytes);
        var docTemp = PdfReader.Open(stream, PdfDocumentOpenMode.Import);
        for (int i = 0; i < docTemp.PageCount; i++)
          docFinal.AddPage(docTemp.Pages[i]);
      }

      docFinal.AddPage(docOriginal.Pages[proposalConfig.PageStartBeneficiarios]); // Copia a página de proposta

      foreach (var pageBytes in insertAtEnd)
      {
        using var stream = new MemoryStream(pageBytes);
        var docTemp = PdfReader.Open(stream, PdfDocumentOpenMode.Import);
        for (int i = 0; i < docTemp.PageCount; i++)
          docFinal.AddPage(docTemp.Pages[i]);
      }

      // 3. Copia o restante das páginas do original
      for (int i = proposalConfig.PageStartBeneficiarios + 1; i < docOriginal.PageCount; i++)
        docFinal.AddPage(docOriginal.Pages[i]);

      // 4. Insere páginas ao final do PDF


      docFinal.Save(outputStream);
      return outputStream.ToArray();
    }
  }
}
