using Microsoft.AspNetCore.Mvc;
using RecomeceAPI.Services;
using RecomeceAPI.Services.Common;
using RecomeceAPI.Models;

namespace RecomeceAPI.Controllers
{
  [Route("api/[controller]")]
  public class CadClientesController : ControllerBase
  {
    private readonly CadClientesService _service;
    private readonly HandleCatchService _handleCatchService;

    public CadClientesController(CadClientesService service, HandleCatchService handleCatchService)
    {
      _service = service;
      _handleCatchService = handleCatchService;
    }

    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll([FromQuery] int page, [FromQuery] int itensPerPage, [FromQuery] string nome, [FromQuery] string insc)
    {
      try
      {
        if(itensPerPage > 50)
          return Ok(NotificationService.Validation("Somente é permitido 50 registros por página"));

        var pagination = await _service.GetAllAsync(page, itensPerPage, nome, insc);

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
        var isFind = await _service.GetByIdDS(id);

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
    public async Task<IActionResult> Save([FromBody] object json)
    {
      try
      {
        var model = new CadClientesModel();
        HandleJsonService.SetByJson(json.ToString() ?? "", model);
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
    public async Task<IActionResult> Update([FromBody] object json)
    {
      try
      {
        var model = new CadClientesModel();
        HandleJsonService.SetByJson(json.ToString() ?? "", model);
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
    [HttpGet("AutoComplete")]
    public async Task<IActionResult> AutoComplete([FromQuery] string texto)
    {
      try
      {
        var pagination = await _service.Autocomplete(texto);

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
  }
}