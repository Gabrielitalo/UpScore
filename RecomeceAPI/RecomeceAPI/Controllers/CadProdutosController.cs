using Microsoft.AspNetCore.Mvc;
using RecomeceAPI.Services;
using RecomeceAPI.Services.Common;
using RecomeceAPI.Models;

namespace RecomeceAPI.Controllers
{
  [Route("api/[controller]")]
  public class CadProdutosController : ControllerBase
  {
    private readonly CadProdutosService _service;
    private readonly HandleCatchService _handleCatchService;

    public CadProdutosController(CadProdutosService service, HandleCatchService handleCatchService)
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
    [HttpGet("GelAllActive")]
    public async Task<IActionResult> GelAllActive()
    {
      try
      {
        var pagination = await _service.GelAllActive();

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
    [HttpGet("GelAllRanges")]
    public async Task<IActionResult> GelAllRanges(long id)
    {
      try
      {
        var pagination = await _service.GelAllRanges(id);

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
    public async Task<IActionResult> Save([FromBody] object json)
    {
      try
      {
        var model = new CadProdutosModel();
        HandleJsonService.SetByJson(json.ToString() ?? "", model);
        model.DataFinal = DateTime.Now;
        model.Situacao = 1;
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
        var model = new CadProdutosModel();
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
  }
}