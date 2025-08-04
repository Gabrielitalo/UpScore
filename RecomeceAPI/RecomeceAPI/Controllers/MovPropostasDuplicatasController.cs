using Microsoft.AspNetCore.Mvc;
using RecomeceAPI.Services;
using RecomeceAPI.Services.Common;
using RecomeceAPI.Models;
using RecomeceAPI.DTOs;

namespace RecomeceAPI.Controllers
{
  [Route("api/[controller]")]
  public class MovPropostasDuplicatasController : ControllerBase
  {
    private readonly MovPropostasDuplicatasService _service;
    private readonly HandleCatchService _handleCatchService;

    public MovPropostasDuplicatasController(MovPropostasDuplicatasService service, HandleCatchService handleCatchService)
    {
      _service = service;
      _handleCatchService = handleCatchService;
    }

    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll([FromQuery] long proposalId)
    {
      try
      {
        var pagination = await _service.GetAllAsync(proposalId);

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
    public async Task<IActionResult> GetById(string id)
    {
      try
      {
        var isFind = await _service.GetByIdAsync(id);

        if (string.IsNullOrEmpty(isFind.Id))
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
    public async Task<IActionResult> Save([FromBody] MovPropostasDuplicatasModel model)
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
    public async Task<IActionResult> Update([FromBody] MovPropostasDuplicatasModel model)
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
    public async Task<IActionResult> Delete(string id)
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
    [HttpPost("GeneratePayments")]
    public async Task<IActionResult> GeneratePayments([FromQuery] long proposalId, [FromBody] object obj)
    {
      try
      {
        CondPagtoDTO dto = new CondPagtoDTO();
        HandleJsonService.SetByJson(obj.ToString() ?? "", dto);
        var result = await _service.GeneratePayments(proposalId, dto);
        return Ok(HandleJsonService.GetJson(result));
      }
      catch (Exception ex)
      {
        _handleCatchService.HandleCatch(ex);
        return Ok(NotificationService.Error());
      }
    }
    [HttpGet("CheckPayments")]
    public async Task<IActionResult> CheckPayments([FromQuery] long proposalId)
    {
      try
      {
        return Ok(HandleJsonService.GetJson(await _service.CheckPayments(proposalId)));
      }
      catch (Exception ex)
      {
        _handleCatchService.HandleCatch(ex);
        return Ok(NotificationService.Error());
      }
    }
  }
}