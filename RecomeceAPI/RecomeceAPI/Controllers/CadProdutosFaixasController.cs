using Microsoft.AspNetCore.Mvc;
using RecomeceAPI.Services;
using RecomeceAPI.Services.Common;
using RecomeceAPI.Models;
using RecomeceAPI.DTOs;

namespace RecomeceAPI.Controllers
{
  [Route("api/[controller]")]
  public class CadProdutosFaixasController : ControllerBase
  {
    private readonly CadProdutosFaixasService _service;
    private readonly CadProdutosFaixasPagamentoService _cadProdutosFaixasPagamentoService;
    private readonly HandleCatchService _handleCatchService;

    public CadProdutosFaixasController(CadProdutosFaixasService service, HandleCatchService handleCatchService, CadProdutosFaixasPagamentoService cadProdutosFaixasPagamentoService)
    {
      _service = service;
      _handleCatchService = handleCatchService;
      _cadProdutosFaixasPagamentoService = cadProdutosFaixasPagamentoService;
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
        var model = new CadProdutosFaixasModel();
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
        var model = new CadProdutosFaixasModel();
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
    [HttpGet("GetProductRangeDebit")]
    public async Task<IActionResult> GetProductRangeDebit([FromQuery] long productId, [FromQuery] decimal value)
    {
      try
      {
        var pagination = await _service.GetProductRangeDebit(productId, value);

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
    [HttpPost("NewRange")]
    public async Task<IActionResult> NewRange([FromBody] object json)
    {
      try
      {
        var model = new NewProductRangeDTO();
        HandleJsonService.SetByJson(json.ToString() ?? "", model);
        var range = await _service.SaveAsync(model.CadProdutosFaixas);
        if(range is long)
        {
          await _cadProdutosFaixasPagamentoService.ProcessNewRange((long)range, model.CadProdutosFaixasPagamento);
        }
        return Ok(true);
      }
      catch (Exception ex)
      {
        _handleCatchService.HandleCatch(ex);
        return Ok(NotificationService.Error());
      }
    }
  }
}