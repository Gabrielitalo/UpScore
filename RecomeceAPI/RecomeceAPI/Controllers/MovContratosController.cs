using Microsoft.AspNetCore.Mvc;
using RecomeceAPI.Services;
using RecomeceAPI.Services.Common;
using RecomeceAPI.Models;
using QuestPDF.Fluent;
using RecomeceAPI.Services.PDFs;
using System.IO.Compression;
using System.Data;

namespace RecomeceAPI.Controllers
{
  [Route("api/[controller]")]
  public class MovContratosController : ControllerBase
  {
    private readonly MovContratosService _service;
    private readonly MovPropostasService _movPropostasService;
    private readonly CadClientesService _cadClientesService;
    private readonly MovPropostasBeneficiariosService _movPropostasBeneficiariosService;
    private readonly MovPropostasArquivosService _movPropostasArquivosService;
    private readonly CadEmpresasService _cadEmpresasService;
    private readonly MovPropostasDuplicatasService _movPropostasDuplicatasService;
    private readonly HandleCatchService _handleCatchService;
    private readonly ContractService _contratoService;

    public MovContratosController(MovContratosService service, HandleCatchService handleCatchService, MovPropostasService movPropostasService,
      MovPropostasBeneficiariosService movPropostasBeneficiariosService, CadClientesService cadClientesService, CadEmpresasService cadEmpresasService,
      MovPropostasDuplicatasService movPropostasDuplicatasService, MovPropostasArquivosService movPropostasArquivosService, ContractService contratoService)
    {
      _service = service;
      _handleCatchService = handleCatchService;
      _movPropostasService = movPropostasService;
      _movPropostasBeneficiariosService = movPropostasBeneficiariosService;
      _cadClientesService = cadClientesService;
      _cadEmpresasService = cadEmpresasService;
      _movPropostasDuplicatasService = movPropostasDuplicatasService;
      _movPropostasArquivosService = movPropostasArquivosService;
      _contratoService = contratoService;
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
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
      try
      {
        var isFind = await _service.GetByIdAsync(id);

        if (isFind == null || isFind.Id == 0)
          return Ok(NotificationService.NoExists());

        return Ok(isFind);
      }
      catch (Exception ex)
      {
        _handleCatchService.HandleCatch(ex);
        return Ok(NotificationService.Error());
      }
    }
    [HttpPost]
    public async Task<IActionResult> Save([FromBody] MovContratosModel model)
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
    public async Task<IActionResult> Update([FromBody] MovContratosModel model)
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
    [HttpGet("UpdateStatus")]
    public async Task<IActionResult> UpdateStatus([FromQuery] int contractId, [FromQuery] int status)
    {
      try
      {
        var contract = await _service.GetByIdAsync(contractId);
        contract.MovPropostasModel = await _movPropostasService.GetByIdAsync(contract.MovPropostasModel.Id);
        contract.MovPropostasModel.CadClientes = await _cadClientesService.GetByIdAsync(contract.MovPropostasModel.CadClientes.Id);
        if (status == 3) // Aprovado
        {
          var notifications = await _cadClientesService.IsFullClienAsync(contract.MovPropostasModel.CadClientes.Id);
          if (notifications.Count > 1)
          {
            var json = HandleJsonService.GetJson(notifications);
            return Ok(json);
          }
        }
        else if(status == 4)
        {
          var repoval = await _service.UpdateStatus(contractId, status);
          return Ok(repoval);
        }

        DataTable hasContract = (DataTable)await _movPropostasArquivosService.GetContractProposal(contract.MovPropostasModel.Id);
        if (hasContract.Rows.Count == 0)
          return Ok(HandleJsonService.GetJson(NotificationService.Validation("Necessário enviar o contrato assinado")));

        DataTable fichas = (DataTable) await _movPropostasArquivosService.GetFichasProposal(contract.MovPropostasModel.Id);
        var beneficiarios = await _movPropostasBeneficiariosService.GetAllByProposal(contract.MovPropostasModel.Id);
        if(fichas.Rows.Count != beneficiarios.Count)
          return Ok(HandleJsonService.GetJson(NotificationService.Validation("Necessário enviar todas as fichas associativas do contrato")));

        var result = await _service.UpdateStatus(contractId, status);
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
    [HttpGet("DownloadContrato")]
    public async Task<IActionResult> DownloadContrato(long contractId)
    {
      try
      {
        var contract = await _service.GetByIdAsync(contractId);
        contract.MovPropostasModel = await _movPropostasService.GetByIdAsync(contract.MovPropostasModel.Id);
        contract.MovPropostasModel.CadClientes = await _cadClientesService.GetByIdAsync(contract.MovPropostasModel.CadClientes.Id);
        var empresa = await _cadEmpresasService.GetCompanyUserLogged();
        var payments = await _movPropostasDuplicatasService.GetAllPropossalAsync(contract.MovPropostasModel.Id);
        var beneficiarios = await _movPropostasBeneficiariosService.GetAllByProposal(contract.MovPropostasModel.Id);  

        var stream = await _contratoService.GenerateContract(contract, empresa, payments, beneficiarios);
        return File(stream, "application/pdf", "Contrato.pdf");
      }
      catch (Exception ex)
      {
        _handleCatchService.HandleCatch(ex);
        return Ok(NotificationService.Error());
      }
    }
    [HttpGet("DownloadFichasAssociativas")]
    public async Task<IActionResult> DownloadFichasAssociativas(long proposalId)
    {
      try
      {
        var company = await _cadEmpresasService.GetCompanyUserLogged();
        var cidade = $"{company.Cidades.NomeCidade} - {company.Cidades.Estados.UF}";
        var beneficiarios = await _movPropostasBeneficiariosService.GetAllByProposal(proposalId);

        // 2. Cria o stream do ZIP (esse NÃO deve ser descartado antes do return)
        using var zipStream = new MemoryStream();

        // 3. Cria o ZIP deixando o zipStream aberto após o 'using'
        using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, leaveOpen: true))
        {
          int index = 1;

          foreach (var item in beneficiarios)
          {
            // 4. Gera o PDF para o item
            var pdf = new AssociativeFormPdf(
                item.CadClientes.Nome,
                ExtensionService.AplicarMascaraCpfCnpj(item.CadClientes.Inscricao),
                DateTime.Today,
                cidade
            );

            using var pdfStream = new MemoryStream(); // stream do PDF
            pdf.GeneratePdf(pdfStream);
            pdfStream.Position = 0;

            // 5. Adiciona o PDF ao ZIP
            var entry = archive.CreateEntry($"FichaAssociativa_{item.CadClientes.Nome}.pdf");
            using var entryStream = entry.Open();
            pdfStream.CopyTo(entryStream);

            index++;
          }
        } // ⛔ ZipArchive fechado aqui, MAS zipStream continua disponível

        // 6. Reposiciona ponteiro e retorna o arquivo
        zipStream.Position = 0;
        var zipBytes = zipStream.ToArray();
        return File(zipBytes, "application/zip", $"Fichas_{proposalId}.zip");
      }
      catch (Exception ex)
      {
        _handleCatchService.HandleCatch(ex);
        return Ok(NotificationService.Error());
      }
    }
  }
}