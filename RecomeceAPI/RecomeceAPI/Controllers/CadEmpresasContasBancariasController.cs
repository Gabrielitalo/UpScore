using Microsoft.AspNetCore.Mvc;
using RecomeceAPI.Services;
using RecomeceAPI.Services.Common;
using RecomeceAPI.Models;

namespace RecomeceAPI.Controllers
{
  [Route("api/[controller]")]
  public class CadEmpresasContasBancariasController : ControllerBase
  {
    private readonly CadEmpresasContasBancariasService _service;
    private readonly HandleCatchService _handleCatchService;

    public CadEmpresasContasBancariasController(CadEmpresasContasBancariasService service, HandleCatchService handleCatchService)
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
    [HttpGet("GetAllCompany")]
    public async Task<IActionResult> GetAllCompany([FromQuery] long empresaId)
    {
      try
      {
        var pagination = await _service.GetAllAsync(empresaId);

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
        var isFind = await _service.GetById(id);

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
    public async Task<IActionResult> Save([FromBody] CadEmpresasContasBancariasModel model)
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
    public async Task<IActionResult> Update([FromBody] CadEmpresasContasBancariasModel model)
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
  }
}