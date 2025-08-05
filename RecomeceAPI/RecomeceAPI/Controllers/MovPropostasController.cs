using Microsoft.AspNetCore.Mvc;
using RecomeceAPI.Services;
using RecomeceAPI.Services.Common;
using RecomeceAPI.Models;
using RecomeceAPI.Services.PDFs;
using System.IO;

namespace RecomeceAPI.Controllers
{
  [Route("api/[controller]")]
  public class MovPropostasController : ControllerBase
  {
    private readonly MovPropostasService _service;
    private readonly HandleCatchService _handleCatchService;
    private readonly MovPropostasBeneficiariosService _movPropostasBeneficiariosService;
    private readonly MovContratosService _movContratosService;
    private readonly MovCadClientesSerasaService _movCadClientesSerasaService;
    private readonly MovPropostasService _movPropostasService;
    private readonly CadProdutosFaixasPagamentoService _cadProdutosFaixasPagamentoService;
    private readonly ProposalService _proposalService;

    public MovPropostasController(MovPropostasService service, HandleCatchService handleCatchService, MovPropostasBeneficiariosService movPropostasBeneficiariosService,
      MovContratosService movContratosService, MovCadClientesSerasaService movCadClientesSerasaService, MovPropostasService movPropostasService, CadProdutosFaixasPagamentoService cadProdutosFaixasPagamentoService, ProposalService proposalService)
    {
      _service = service;
      _handleCatchService = handleCatchService;
      _movPropostasBeneficiariosService = movPropostasBeneficiariosService;
      _movContratosService = movContratosService;
      _movCadClientesSerasaService = movCadClientesSerasaService;
      _movPropostasService = movPropostasService;
      _cadProdutosFaixasPagamentoService = cadProdutosFaixasPagamentoService;
      _proposalService = proposalService;
    }

    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll(string nome, string insc, int situacao, string dataInicial, string dataFinal, int page, long vendedor)
    {
      try
      {
        var pagination = await _service.GetAllAsync(nome, insc, situacao, dataInicial, dataFinal, page, vendedor);

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
    [HttpGet("GetAllFromClient")]
    public async Task<IActionResult> GetAllFromClient(long clientId)
    {
      try
      {
        var pagination = await _service.GetAllFromClient(clientId);

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
    [HttpGet("GeneratePdf")]
    public async Task<IActionResult> GeneratePdf()
    {
      try
      {
        PdfGenerator pdf = new PdfGenerator();
        var b = pdf.GerarPdf();
        return File(b, "application/pdf", "Relatorio.pdf");
      }
      catch (Exception ex)
      {
        _handleCatchService.HandleCatch(ex);
        return Ok(NotificationService.Error());
      }
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
      try
      {
        var isFind = await _service.GetByIdDetails(id);

        if (isFind == null)
          return Ok(NotificationService.NoExists());

        return Ok(HandleJsonService.GetJson(isFind));
      }
      catch (Exception ex)
      {
        _handleCatchService.HandleCatch(ex);
        return Ok(NotificationService.Error());
      }
    }
    [HttpPost]
    public async Task<IActionResult> Save([FromBody] MovPropostasModel model)
    {
      try
      {
        var isDone = await _service.SaveAsync(model);
        return Ok(isDone);
      }
      catch (Exception ex)
      {
        _handleCatchService.HandleCatch(ex);
        return Ok(NotificationService.Error());
      }
    }
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] MovPropostasModel model)
    {
      try
      {
        var isDone = await _service.UpdateAsync(model);
        return Ok(isDone);
      }
      catch (Exception ex)
      {
        _handleCatchService.HandleCatch(ex);
        return Ok(NotificationService.Error());
      }
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
      try
      {
        var isDone = await _service.DeleteAsync(id);
        return Ok(isDone);
      }
      catch (Exception ex)
      {
        _handleCatchService.HandleCatch(ex);
        return Ok(NotificationService.Error());
      }
    }
    [HttpGet("NewProposal")]
    public async Task<IActionResult> NewProposal([FromQuery] int productID, [FromQuery] string insc)
    {
      try
      {
        var proposal = await _service.NewProposal(productID, insc);
        if (proposal.serasa.StatusConsulta == 401)
        {
          return Ok(NotificationService.Validation("Você não tem saldo de consultas suficiente, realize a compra de novas consultas"));

        }
        await _movPropostasBeneficiariosService.NewProposalClientSerasa(proposal.proposalId, proposal.serasa);

        if (proposal.proposalId == 0)
          return Ok(NotificationService.NoContent());

        await _service.UpdateMovPropostas(proposal.proposalId);
        return Ok(HandleJsonService.GetJson(proposal.proposalId));
      }
      catch (Exception ex)
      {
        _handleCatchService.HandleCatch(ex);
        return Ok(NotificationService.Error());
      }
    }
    [HttpGet("AddClient")]
    public async Task<IActionResult> AddClient([FromQuery] int proposalId, [FromQuery] string insc)
    {
      try
      {
        var serasa = await _service.QuerySerasa(insc);
        if (serasa.StatusConsulta == 401)
        {
          return Ok(NotificationService.Validation("Você não tem saldo de consultas suficiente, realize a compra de novas consultas"));
        }
        var result = await _movPropostasBeneficiariosService.NewProposalClientSerasa(proposalId, serasa);
        await _service.UpdateMovPropostas(proposalId);
        if (result == null)
          return Ok(NotificationService.NoContent());

        return Ok(HandleJsonService.GetJson(result));
      }
      catch (Exception ex)
      {
        _handleCatchService.HandleCatch(ex);
        return Ok(NotificationService.Error());
      }
    }
    [HttpPut("UpdateTotals")]
    public async Task<IActionResult> UpdateTotals([FromBody] MovPropostasModel model)
    {
      try
      {
        var isDone = await _service.UpdateTotals(model);
        return Ok(isDone);
      }
      catch (Exception ex)
      {
        _handleCatchService.HandleCatch(ex);
        return Ok(NotificationService.Error());
      }
    }
    [HttpGet("UpdateStatus")]
    public async Task<IActionResult> UpdateStatus([FromQuery] int proposalId, [FromQuery] int status)
    {
      try
      {
        var proposal = await _service.GetByIdAsync(proposalId); 
        var result = await _service.UpdateStatus(proposalId, status);
        if(proposal.ValorAprovado == 0 && status == 3) // Em caso de aprovação o valor aprovado sempre deve ser preenchido
          await _service.UpdateValorAprovado(proposal.Id, proposal.ValorContrato);
        if (result == null)
          return Ok(NotificationService.NoContent());

        if (status == 3 && proposal.CadProdutos.LimpaNome == 1) // Caso seja aprovado deverá criar o contrato
          await _movContratosService.NewFromProposal(proposalId);

        return Ok(HandleJsonService.GetJson(result));
      }
      catch (Exception ex)
      {
        _handleCatchService.HandleCatch(ex);
        return Ok(NotificationService.Error());
      }
    }
    [HttpGet("UpdateVendedor")]
    public async Task<IActionResult> UpdateVendedor([FromQuery] int proposalId, [FromQuery] long equipeId)
    {
      try
      {
        var result = await _service.UpdateVendedor(proposalId, equipeId);
        if (result == null)
          return Ok(NotificationService.NoContent());

        return Ok(HandleJsonService.GetJson(result));
      }
      catch (Exception ex)
      {
        _handleCatchService.HandleCatch(ex);
        return Ok(NotificationService.Error());
      }
    }
    [HttpGet("UpdateOwner")]
    public async Task<IActionResult> UpdateOwner([FromQuery] int proposalId, [FromQuery] long clientId)
    {
      try
      {
        var result = await _service.UpdateOwner(proposalId, clientId);
        if (result == null)
          return Ok(NotificationService.NoContent());

        return Ok(HandleJsonService.GetJson(result));
      }
      catch (Exception ex)
      {
        _handleCatchService.HandleCatch(ex);
        return Ok(NotificationService.Error());
      }
    }
    [HttpGet("UpdateValorAvulso")]
    public async Task<IActionResult> UpdateValorAvulso([FromQuery] int proposalId, [FromQuery] decimal valorAvulso)
    {
      try
      {
        var result = await _service.UpdateValorAvulso(proposalId, valorAvulso);
        if (result == null)
          return Ok(NotificationService.NoContent());

        return Ok(HandleJsonService.GetJson(result));
      }
      catch (Exception ex)
      {
        _handleCatchService.HandleCatch(ex);
        return Ok(NotificationService.Error());
      }
    }
    [HttpGet("UpdateOrigem")]
    public async Task<IActionResult> UpdateOrigem([FromQuery] int proposalId, [FromQuery] int origemId)
    {
      try
      {
        var result = await _service.UpdateOrigem(proposalId, origemId);
        if (result == null)
          return Ok(NotificationService.NoContent());

        return Ok(HandleJsonService.GetJson(result));
      }
      catch (Exception ex)
      {
        _handleCatchService.HandleCatch(ex);
        return Ok(NotificationService.Error());
      }
    }
    [HttpGet("UpdateDate")]
    public async Task<IActionResult> UpdateDate([FromQuery] int proposalId, [FromQuery] string date)
    {
      try
      {
        var result = await _service.UpdateDate(proposalId, date);
        if (result == null)
          return Ok(NotificationService.NoContent());

        return Ok(HandleJsonService.GetJson(result));
      }
      catch (Exception ex)
      {
        _handleCatchService.HandleCatch(ex);
        return Ok(NotificationService.Error());
      }
    }
    [HttpGet("GetOrigens")]
    public async Task<IActionResult> GetOrigens()
    {
      try
      {
        var result = await _service.GetOrigens();
        if (result == null)
          return Ok(NotificationService.NoContent());

        return Ok(HandleJsonService.GetJson(result));
      }
      catch (Exception ex)
      {
        _handleCatchService.HandleCatch(ex);
        return Ok(NotificationService.Error());
      }
    }
    [HttpGet("GetProposalPdf")]
    public async Task<IActionResult> GetProposalPdf([FromQuery] int proposalId)
    {
      try
      {
        var pdfBytes = await _proposalService.GenerateProposal(proposalId);
        return File(pdfBytes, "application/pdf", $"Proposta.pdf");

      }
      catch (Exception ex)
      {
        _handleCatchService.HandleCatch(ex);
        return Ok(NotificationService.Error());
      }
    }
    [HttpGet("GetDashboard")]
    public async Task<IActionResult> GetDashboard([FromQuery] string dataInicial, [FromQuery] string dataFinal, [FromQuery] long vendedor, [FromQuery] long productId)
    {
      try
      {
        var result = await _service.GetDashboard(dataInicial, dataFinal, vendedor, productId);
        if (result == null)
          return Ok(NotificationService.NoContent());

        return Ok(HandleJsonService.GetJson(result));
      }
      catch (Exception ex)
      {
        _handleCatchService.HandleCatch(ex);
        return Ok(NotificationService.Error());
      }
    }
    [HttpPost("NewCustomSale")]
    public async Task<IActionResult> NewCustomSale([FromBody] object json)
    {
      try
      {
        var model = new MovPropostasModel();
        HandleJsonService.SetByJson(json.ToString() ?? "", model);

        var isDone = await _service.NewCustomSale(model);
        return Ok(isDone);
      }
      catch (Exception ex)
      {
        _handleCatchService.HandleCatch(ex);
        return Ok(NotificationService.Error());
      }
    }
  }
}