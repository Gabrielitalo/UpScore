using Microsoft.AspNetCore.Mvc;
using RecomeceAPI.Services;
using RecomeceAPI.Services.Common;
using RecomeceAPI.Models;

namespace RecomeceAPI.Controllers
{
  [Route("api/[controller]")]
  public class MovCadClientesSerasaController : ControllerBase
  {
    private readonly MovCadClientesSerasaService _service;
    private readonly HandleCatchService _handleCatchService;

    public MovCadClientesSerasaController(MovCadClientesSerasaService service, HandleCatchService handleCatchService)
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
    [HttpGet("GetAllClients")]
    public async Task<IActionResult> GetAllClients([FromQuery] long clientId)
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
    public async Task<IActionResult> Save([FromBody] MovCadClientesSerasaModel model)
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
    public async Task<IActionResult> Update([FromBody] MovCadClientesSerasaModel model)
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
    [HttpGet("NewClient")]
    public async Task<IActionResult> NewClient([FromQuery] string insc, [FromQuery] string tel, [FromQuery] string email)
    {
      try
      {
        var response = await _service.NewClient(insc, tel, email);

        if (response == null)
          return Ok(NotificationService.NoContent());

        return Ok(HandleJsonService.GetJson(response));
      }
      catch (Exception ex)
      {
        _handleCatchService.HandleCatch(ex);
        return Ok(NotificationService.Error());
      }
    }
    [HttpGet("ProcessMarkID")]
    public async Task<IActionResult> ProcessMarkID([FromQuery] string markID)
    {
      try
      {
        await _service.ProcessMarkID(markID);
        return Ok(true);
      }
      catch (Exception ex)
      {
        _handleCatchService.HandleCatch(ex);
        return Ok(NotificationService.Error());
      }
    }
    [HttpGet("GetConcentreMarkID")]
    public async Task<IActionResult> GetConcentreMarkID([FromQuery] string markID)
    {
      try
      {
        return Ok(HandleJsonService.GetJson(await _service.GetConcentreMarkID(markID)));
      }
      catch (Exception ex)
      {
        _handleCatchService.HandleCatch(ex);
        return Ok(NotificationService.Error());
      }
    }
    [HttpGet("QuerySerasa")]
    public async Task<IActionResult> QuerySerasa(int page, string nome, string insc, string dataInicial, string dataFinal)
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
  }
}