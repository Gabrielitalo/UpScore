using Microsoft.AspNetCore.Mvc;
using RecomeceAPI.Services;
using RecomeceAPI.Services.Common;
using RecomeceAPI.Models;

namespace RecomeceAPI.Controllers
{
  [Route("api/[controller]")]
  public class CadEquipeController : ControllerBase
  {
    private readonly CadEquipeService _service;
    private readonly HandleCatchService _handleCatchService;

    public CadEquipeController(CadEquipeService service, HandleCatchService handleCatchService)
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
        var model = new CadEquipeModel();
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
        var model = new CadEquipeModel();
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
    [HttpGet("GetAllCurrentCompanyAsync")]
    public async Task<IActionResult> GetAllCurrentCompanyAsync()
    {
      try
      {
        var pagination = await _service.GetAllCurrentCompanyAsync();

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
    [HttpGet("CreateFromCompany")]
    public async Task<IActionResult> CreateFromCompany(long companyId, int userType)
    {
      try
      {
        var pagination = await _service.CreateFromCompany(companyId, userType);

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