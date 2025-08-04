using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecomeceAPI.Services.Common;
using RecomeceAPI.Services;
using RecomeceAPI.Services.Serasa;
using RecomeceAPI.Services.PDFs;
using RecomeceAPI.Services.BoaVista;
using DinkToPdf;
using DinkToPdf.Contracts;
using System;
using RecomeceAPI.DTOs;
using PuppeteerSharp.Media;
using PuppeteerSharp;

namespace RecomeceAPI.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class LogConsultasController : ControllerBase
  {
    private readonly LogConsultasService _service;
    private readonly HandleCatchService _handleCatchService;
    private readonly ProposalService _proposalService;
    private readonly BoaVistaConsultaService _boaVistaConsultaService;
    private readonly IConverter _converter;
    private readonly CadConsultasService _cadConsultasService;

    public LogConsultasController(LogConsultasService service, HandleCatchService handleCatchService, ProposalService proposalService, BoaVistaConsultaService boaVistaConsultaService, IConverter converter, CadConsultasService cadConsultasService)
    {
      _service = service;
      _handleCatchService = handleCatchService;
      _proposalService = proposalService;
      _boaVistaConsultaService = boaVistaConsultaService;
      _converter = converter;
      _cadConsultasService = cadConsultasService;
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
      try
      {
        var isFind = await _service.GetById(id);

        if (isFind == null || string.IsNullOrEmpty(isFind.Id))
          return Ok(NotificationService.NoExists());

        return Ok(isFind);
      }
      catch (Exception ex)
      {
        _handleCatchService.HandleCatch(ex);
        return Ok(NotificationService.Error());
      }
    }
    [HttpGet("QuerySerasa")]
    public async Task<IActionResult> QuerySerasa(int page, string? nome, string? insc, string dataInicial, string dataFinal)
    {
      try
      {
        var pagination = await _service.QuerySerasa(page, nome, insc, dataInicial, dataFinal);

        if (pagination == null || pagination?.TotalItens == 0)
          return Ok(NotificationService.NoContent());

        return Ok(HandleJsonService.GetJson(pagination));
      }
      catch (Exception ex)
      {
        _handleCatchService.HandleCatch(ex);
        return Ok(NotificationService.Error());
      }
    }
    [HttpGet("GetAllClients")]
    public async Task<IActionResult> GetAllClients(long clientId)
    {
      try
      {
        var pagination = await _service.GetAllClients(clientId);

        if (pagination == null)
          return Ok(NotificationService.NoContent());

        return Ok(HandleJsonService.GetJson(pagination));
      }
      catch (Exception ex)
      {
        _handleCatchService.HandleCatch(ex);
        return Ok(NotificationService.Error());
      }
    }
    [HttpGet("NewQuery")]
    public async Task<IActionResult> NewQuery([FromQuery] long productId, [FromQuery] string inscricao)
    {
      try
      {
        inscricao = ExtensionService.CleanSpecialChars(inscricao);
        var query = await _cadConsultasService.GetByIdAsync(productId);
        if (query.Tipo == 1)
        {
          var response = await _proposalService.NewQuerySerasa(productId, inscricao);
          var isExists = await _service.GetByInscricaoProdutoAsync(inscricao, productId);
          return Ok(new { isExists.Id, query.Tipo });
        }
        else if (query.Tipo == 2)
        {
          string guid = Guid.NewGuid().ToString();
          await _boaVistaConsultaService.Handle(inscricao, productId, guid);
          var isExists = await _service.GetByInscricaoProdutoAsync(inscricao, productId);
          return Ok(new { isExists.Id, query.Tipo });
        }
        else
        {
          return Ok(NotificationService.Validation("Tipo de consulta escolhido é inválido"));
        }
      }
      catch (Exception ex)
      {
        _handleCatchService.HandleCatch(ex);
        return Ok(NotificationService.Error());
      }
    }

    [HttpGet("BoaVista")]
    public async Task<IActionResult> BoaVista([FromQuery] long productId, [FromQuery] string inscricao)
    {
      try
      {
        inscricao = ExtensionService.CleanSpecialChars(inscricao);
        string guid = Guid.NewGuid().ToString();
        await _boaVistaConsultaService.GetPJ(inscricao, productId, guid);
        return Ok(guid);
      }
      catch (Exception ex)
      {
        _handleCatchService.HandleCatch(ex);
        return Ok(NotificationService.Error());
      }
    }

    [HttpPost("GerarPdf")]
    public async Task<IActionResult> GerarPdf([FromBody] UploadDTO body)
    {
      try
      {

        //https://googlechromelabs.github.io/chrome-for-testing/last-known-good-versions-with-downloads.json
        var revision = "138.0.7204.168"; // pegar a version
        var chromePath = Path.Combine(AppContext.BaseDirectory, "chrome");

        var browserFetcher = new BrowserFetcher(new BrowserFetcherOptions
        {
          Path = chromePath
        });

        var executablePath = browserFetcher.GetExecutablePath(revision);

        // Verifica se o Chrome já foi baixado
        if (!System.IO.File.Exists(executablePath))
        {
          await browserFetcher.DownloadAsync(revision);
        }

        using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
        {
          Headless = true,
          ExecutablePath = executablePath,
          Args = new[] { "--no-sandbox" }
        });

        using var page = await browser.NewPageAsync();

        await page.SetViewportAsync(new ViewPortOptions { Width = 1200, Height = 800 });

        await page.SetContentAsync(body.FileName, new NavigationOptions
        {
          WaitUntil = new[] { WaitUntilNavigation.Load }
        });

        var pdfBytes = await page.PdfDataAsync(new PdfOptions
        {
          Format = PaperFormat.A4,
          PrintBackground = true,
          MarginOptions = new MarginOptions
          {
            Top = "20px",
            Bottom = "20px",
            Left = "15px",
            Right = "15px"
          }
        });

        return File(pdfBytes, "application/pdf", "relatorio.pdf");

      }
      catch (Exception ex)
      {
        System.Diagnostics.Debug.WriteLine(ex.Message);
        _handleCatchService.HandleCatch(ex);
        return StatusCode(500, ex.Message);
      }
    }

  }
}
