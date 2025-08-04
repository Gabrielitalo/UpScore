using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecomeceAPI.Services;
using RecomeceAPI.Services.Common;
using RecomeceAPI.Services.Serasa;

namespace RecomeceAPI.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class LoginController : ControllerBase
  {
    private readonly LoginService _service;
    private readonly HandleCatchService _handleCatchService;
    private readonly RestSerasaService _restSerasaService;

    public LoginController(LoginService service, HandleCatchService handleCatchService, RestSerasaService restSerasaService)
    {
      _service = service;
      _handleCatchService = handleCatchService;
      _restSerasaService = restSerasaService;
    }

    [HttpGet()]
    public async Task<IActionResult> Login(int userType, string email, string password)
    {
      try
      {
        var isFind = await _service.Login(userType, email, password);

        if (isFind.Active == 0)
          return Ok(NotificationService.NoExists());

        return Ok(isFind);
      }
      catch (Exception ex)
      {
        _handleCatchService.HandleCatch(ex);
        return Ok(NotificationService.Error());
      }
    }
    [HttpGet("NovoSerasa")]
    public async Task<IActionResult> NovoSerasa(string inscricao, int modelo)
    {
      try
      {
        var isFind = await _restSerasaService.Get(inscricao, modelo);
        return Ok(isFind);
      }
      catch (Exception ex)
      {
        _handleCatchService.HandleCatch(ex);
        return Ok(NotificationService.Error());
      }
    }
  }
}
