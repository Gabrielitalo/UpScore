using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Utilities;
using RecomeceAPI.Models;
using RecomeceAPI.Services;
using RecomeceAPI.Services.Common;

namespace RecomeceAPI.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class MovLotesEmpresasBeneficiariosController : ControllerBase
  {
    private readonly MovLotesEmpresasBeneficiariosService _service;
    private readonly HandleCatchService _handleCatchService;

    public MovLotesEmpresasBeneficiariosController(MovLotesEmpresasBeneficiariosService service, HandleCatchService handleCatchService)
    {
      _service = service;
      _handleCatchService = handleCatchService;
    }


    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll(string batchId)
    {
      try
      {
        var pagination = await _service.GetAllByBatch(batchId);

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
    [HttpGet("Update")]
    public async Task<IActionResult> Update(string id, string inscricao, string nome)
    {
      try
      {
        var isDone = await _service.UpdateAsync(id, inscricao, nome);
        return Ok(true);
      }
      catch (Exception ex)
      {
        _handleCatchService.HandleCatch(ex);
        return Ok(NotificationService.Error());
      }
    }
    [HttpGet("DownloadExcel")]
    public async Task<IActionResult> DownloadExcel([FromQuery] string batchId)
    {
      try
      {
        var members = await _service.GetAllMembers(batchId);
        var excel = _service.GetPlan(members);

        return File(excel, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "lote.xlsx");
      }
      catch (Exception ex)
      {
        _handleCatchService.HandleCatch(ex);
        return Ok(NotificationService.Error());
      }
    }
  }
}
