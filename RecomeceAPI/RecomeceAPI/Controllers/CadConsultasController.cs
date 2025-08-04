using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecomeceAPI.Services;
using RecomeceAPI.Services.Common;

namespace RecomeceAPI.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class CadConsultasController : ControllerBase
  {
    private readonly CadConsultasService _service;
    private readonly HandleCatchService _handleCatchService;

    public CadConsultasController(CadConsultasService service, HandleCatchService handleCatchService)
    {
      _service = service;
      _handleCatchService = handleCatchService;
    }


    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll([FromQuery] int page, [FromQuery] int itensPerPage)
    {
      try
      {
        var pagination = await _service.GetAllAsync(page, itensPerPage);

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
  }
}
