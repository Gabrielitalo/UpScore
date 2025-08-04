using DocumentFormat.OpenXml.EMMA;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecomeceAPI.Models.ASAAS;
using RecomeceAPI.Services;
using RecomeceAPI.Services.Common;

namespace RecomeceAPI.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class AsaasController : ControllerBase
  {
    private readonly MovContaCorrenteService _service;
    private readonly MovCadClientesSerasaService _movCadClientesSerasaService;
    private readonly HandleCatchService _handleCatchService;
    private readonly CadEmpresasService _cadEmpresasService;

    public AsaasController(MovContaCorrenteService service, HandleCatchService handleCatchService, MovCadClientesSerasaService movCadClientesSerasaService, CadEmpresasService cadEmpresasService)
    {
      _service = service;
      _handleCatchService = handleCatchService;
      _movCadClientesSerasaService = movCadClientesSerasaService;
      _cadEmpresasService = cadEmpresasService;
    }

    [HttpPost("PaymentwebHook")]
    public async Task<IActionResult> PaymentwebHook(object json)
    {
      string tipoEvento = "PAYMENT_RECEIVED";
      try
      {
        var entity = new PaymentWebHookModel();
        HandleJsonService.SetByJson(json.ToString() ?? "", entity);
        tipoEvento = entity.Event;
        if (tipoEvento == "PAYMENT_RECEIVED")
          await _service.ReceivePayment(entity);
        return Ok(true);
      }
      catch (Exception ex)
      {
        _handleCatchService.HandleCatch(ex);
        return Ok(NotificationService.Error());
      }
      finally
      {
        await _service.SaveLogSerasa(tipoEvento, json.ToString() ?? "");
      }
    }
  }
}
