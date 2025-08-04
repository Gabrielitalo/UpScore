using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecomeceAPI.Services.Common;
using RecomeceAPI.Services;

namespace RecomeceAPI.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class MovLotesAssociacaoController : ControllerBase
  {
    private readonly MovLotesAssociacaoService _service;
    private readonly HandleCatchService _handleCatchService;

    public MovLotesAssociacaoController(MovLotesAssociacaoService service, HandleCatchService handleCatchService)
    {
      _service = service;
      _handleCatchService = handleCatchService;
    }

    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll(string? membro, string? dataInicial, string? dataFinal, int page)
    {
      try
      {
        var pagination = await _service.GetAllAsync(membro, dataInicial, dataFinal, page);

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
    [HttpGet("GetAllAvailable")]
    public async Task<IActionResult> GetAllAvailable()
    {
      try
      {
        var pagination = await _service.GetAllAvailable();

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
    [HttpPost("NewBatch")]
    public async Task<IActionResult> NewBatch([FromBody] string[] batchs)
    {
      try
      {
        await _service.NewBatch(batchs);
        return Ok(true);
      }
      catch (Exception ex)
      {
        _handleCatchService.HandleCatch(ex);
        return Ok(NotificationService.Error());
      }
    }
    [HttpGet("GetDashboard")]
    public async Task<IActionResult> GetDashboard([FromQuery] string dataInicial, [FromQuery] string dataFinal)
    {
      try
      {
        var result = await _service.GetDashboard(dataInicial, dataFinal);
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
  }
}
