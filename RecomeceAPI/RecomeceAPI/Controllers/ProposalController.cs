using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecomeceAPI.Services.Common;
using RecomeceAPI.Services.PDFs;

namespace RecomeceAPI.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class ProposalController : ControllerBase
  {
    private readonly ProposalService _service;
    private readonly HandleCatchService _handleCatchService;

    public ProposalController(ProposalService service, HandleCatchService handleCatchService)
    {
      _service = service;
      _handleCatchService = handleCatchService;
    }

    [HttpGet("ProposalNew")]
    public async Task<IActionResult> ProposalNew([FromQuery] long proposalId, [FromQuery] long productId, [FromQuery] string insc, [FromQuery] bool forceNew = false)
    {
      try
      {
        var serasa = await _service.ProposalNew(productId, proposalId, insc, forceNew);
        //  return Ok(NotificationService.NoContent());

        return Ok(HandleJsonService.GetJson(serasa));
      }
      catch (Exception ex)
      {
        string param = $"proposalId: {proposalId} - productId: {productId} - insc: {insc} - forceNew: {forceNew}";
        _handleCatchService.HandleCatch(ex);
        return Ok(NotificationService.Error());
      }
    }
  }
}
