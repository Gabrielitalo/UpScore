using Microsoft.AspNetCore.Mvc;
using RecomeceAPI.Services;
using RecomeceAPI.Services.Common;
using RecomeceAPI.Models;
using System.IO;
using RecomeceAPI.DTOs;
using RecomeceAPI.Server.Auth;

namespace RecomeceAPI.Controllers
{
  [Route("api/[controller]")]
  public class MovLotesEmpresasController : ControllerBase
  {
    private readonly MovLotesEmpresasService _service;
    private readonly HandleCatchService _handleCatchService;

    public MovLotesEmpresasController(MovLotesEmpresasService service, HandleCatchService handleCatchService)
    {
      _service = service;
      _handleCatchService = handleCatchService;
    }

    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll(string membro, string dataInicial, string dataFinal, int page, string? associacaoId)
    {
      try
      {
        var pagination = await _service.GetAllAsync(membro, dataInicial, dataFinal, page, associacaoId);

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
    public async Task<IActionResult> GetById(string id)
    {
      try
      {
        var isFind = await _service.GetByIdAsync(id);

        if (isFind == null)
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
    public async Task<IActionResult> Save([FromBody] MovLotesEmpresasModel model)
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
    public async Task<IActionResult> Update([FromBody] MovLotesEmpresasModel model)
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
    public async Task<IActionResult> Delete(string id)
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
    [HttpGet("GetAllContractsAvailables")]
    public async Task<IActionResult> GetAllContractsAvailables()
    {
      try
      {
        var pagination = await _service.GetAllContractsAvailables();

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
    [HttpGet("UpdateStatus")]
    public async Task<IActionResult> UpdateStatus(string id, int situacao)
    {
      try
      {
        var pagination = await _service.UpdateStatus(id, situacao);

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
    [HttpPost("NewCompanyBatch")]
    public async Task<IActionResult> NewCompanyBatch([FromBody] long[] contractIds)
    {
      try
      {
        await _service.NewLot(contractIds);
        return Ok(true);
      }
      catch (Exception ex)
      {
        _handleCatchService.HandleCatch(ex);
        return Ok(NotificationService.Error());
      }
    }
    [HttpGet("GetBeneficiarios")]
    public async Task<IActionResult> GetBeneficiarios(long batchId)
    {
      try
      {
        var pagination = await _service.GetBeneficiarios(batchId);

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
    [HttpPost("NewBatchXlsx")]
    public async Task<IActionResult> NewBatchXlsx([FromBody] UploadDTO upload, [FromQuery] long companyId)
    {
      try
      {
        if (companyId == 0)
          companyId = AppIdentity.GetIdCompanyValue();

        return Ok(await _service.ProcessXlxs(upload, companyId));
      }
      catch (Exception ex)
      {
        _handleCatchService.HandleCatch(ex);
        return Ok(NotificationService.Error());
      }
    }
    [HttpGet("GenerateWord")]
    public async Task<IActionResult> GenerateWord(long batchId)
    {
      try
      {
        var word = await _service.GenerateWord(batchId);

        if (word == null)
          return Ok(NotificationService.NoContent());

        return File(word,
                 "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                 "TabelaPessoas.docx");
      }
      catch (Exception ex)
      {
        _handleCatchService.HandleCatch(ex);
        return Ok(NotificationService.Error());
      }
    }
    [HttpGet("DownloadXlsModel")]
    public IActionResult DownloadXlsModel()
    {
      try
      {
        var file = _service.DownloadXls();

        if (file == null)
          return Ok(NotificationService.NoContent());

        return File(file,
                 "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                 "ModeloLote.xlsx");
      }
      catch (Exception ex)
      {
        _handleCatchService.HandleCatch(ex);
        return Ok(NotificationService.Error());
      }
    }
    [HttpGet("GenerateZipFiles")]
    public async Task<IActionResult> GenerateZipFiles(long batchId)
    {
      try
      {
        var zip = await _service.GenerateZipFiles(batchId);

        if (zip == null)
          return Ok(NotificationService.NoContent());

        return File(zip,
              "application/zip",
            $"lote_{DateTime.Now:yyyyMMddHHmmss}.zip");
      }
      catch (Exception ex)
      {
        _handleCatchService.HandleCatch(ex);
        return Ok(NotificationService.Error());
      }
    }
  }
}