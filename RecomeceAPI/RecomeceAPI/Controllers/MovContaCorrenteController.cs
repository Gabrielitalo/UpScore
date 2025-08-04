using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecomeceAPI.Services.Common;
using RecomeceAPI.Services;

namespace RecomeceAPI.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class MovContaCorrenteController : ControllerBase
  {
    private readonly MovContaCorrenteService _service;
    private readonly HandleCatchService _handleCatchService;

    public MovContaCorrenteController(MovContaCorrenteService service, HandleCatchService handleCatchService)
    {
      _service = service;
      _handleCatchService = handleCatchService;
    }

    [HttpGet("AddAccountValue")]
    public async Task<IActionResult> AddAccountValue([FromQuery] decimal value)
    {
      try
      {
         await _service.AddAccountValue(value);
        return Ok(true);
      }
      catch (Exception ex)
      {
        _handleCatchService.HandleCatch(ex);
        return Ok(NotificationService.Error());
      }
    }

    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll([FromQuery] int page, [FromQuery] int itensPerPage, [FromQuery] int tipo, [FromQuery] string dataInicial, [FromQuery] string dataFinal)
    {
      try
      {
        var pagination = await _service.GetAllAsync(page, itensPerPage, tipo, dataInicial, dataFinal);

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
    [HttpGet("GetLastBuy10")]
    public async Task<IActionResult> GetLastBuy10()
    {
      try
      {
        var pagination = await _service.GetLastBuy10();

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
    [HttpGet("GetAvailableValue")]
    public async Task<IActionResult> GetAvailableValue()
    {
      try
      {
        var pagination = await _service.GetTotal();

        if (pagination == 0)
          return Ok(NotificationService.NoContent());

        return Ok(HandleJsonService.GetJson(pagination));
      }
      catch (Exception ex)
      {
        _handleCatchService.HandleCatch(ex);
        return Ok(NotificationService.Error());
      }
    }
  }
}
