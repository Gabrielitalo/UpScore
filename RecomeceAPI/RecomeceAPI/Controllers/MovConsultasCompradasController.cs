using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecomeceAPI.Services.Common;
using RecomeceAPI.Services;
using RecomeceAPI.Server.Auth;
using RecomeceAPI.Models;
using RecomeceAPI.Server;

namespace RecomeceAPI.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class MovConsultasCompradasController : ControllerBase
  {
    private readonly MovConsultasCompradasService _service;
    private readonly MovCadClientesSerasaService _movCadClientesSerasaService;
    private readonly HandleCatchService _handleCatchService;
    private readonly CadEmpresasService _cadEmpresasService;

    public MovConsultasCompradasController(MovConsultasCompradasService service, HandleCatchService handleCatchService, MovCadClientesSerasaService movCadClientesSerasaService, CadEmpresasService cadEmpresasService)
    {
      _service = service;
      _handleCatchService = handleCatchService;
      _movCadClientesSerasaService = movCadClientesSerasaService;
      _cadEmpresasService = cadEmpresasService;
    }
    [HttpGet("GetAvailable")]
    public async Task<IActionResult> GetAvailable()
    {
      try
      {
        long totalQuerys = await _movCadClientesSerasaService.GetQueryCountCompany(Convert.ToInt64(AppIdentity.GetClaimValue("IdCompany")));
        long totalBuy = await _service.GetQueryQtdLimit(Convert.ToInt64(AppIdentity.GetClaimValue("IdCompany")));
        long saldo = totalBuy - totalQuerys;
        var dto = new { totalQuerys, totalBuy, saldo };
        return Ok(dto);
      }
      catch (Exception ex)
      {
        _handleCatchService.HandleCatch(ex);
        return Ok(NotificationService.Error());
      }
    }
    [HttpGet("Plans")]
    public IActionResult Plans()
    {
      try
      {
        return Ok(HandleJsonService.GetJson(_service.Plans()));
      }
      catch (Exception ex)
      {
        _handleCatchService.HandleCatch(ex);
        return Ok(NotificationService.Error());
      }
    }
    [HttpGet("GetLast10")]
    public async Task<IActionResult> GetLast10()
    {
      try
      {
        var response = await _service.GetAllAsync(0, 10);
        return Ok(HandleJsonService.GetJson(response));
      }
      catch (Exception ex)
      {
        _handleCatchService.HandleCatch(ex);
        return Ok(NotificationService.Error());
      }
    }
    [HttpGet("BuyPlan")]
    public async Task<IActionResult> BuyPlan(long planId)
    {
      try
      {
        var entity = new MovConsultasCompradasModel();
        entity = _service.GetChoosePlan(planId);
        entity.CadEmpresas = await _cadEmpresasService.GetByIdAsync(Convert.ToInt64(AppIdentity.GetClaimValue("IdCompany")));
        var pay = await _service.CreateAsaas(entity);
        if (!string.IsNullOrEmpty(pay.Id))
        {
          entity.IdExtPagto = pay.Id;
          await _service.SaveAsync(entity);
          return Ok(true);
        }
        else
          return Ok(false);
      }
      catch (Exception ex)
      {
        _handleCatchService.HandleCatch(ex);
        return Ok(NotificationService.Error());
      }
    }

    [HttpGet("GetAsaasId")]
    public async Task<IActionResult> GetAsaasId(string payId)
    {
      try
      {
        var response = await _service.GetByAsaasId(payId);
        return Ok(HandleJsonService.GetJson(response));
      }
      catch (Exception ex)
      {
        _handleCatchService.HandleCatch(ex);
        return Ok(NotificationService.Error());
      }
    }
  }
}
