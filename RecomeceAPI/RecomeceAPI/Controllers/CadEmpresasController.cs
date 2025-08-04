using Microsoft.AspNetCore.Mvc;
using RecomeceAPI.Services;
using RecomeceAPI.Services.Common;
using RecomeceAPI.Models;
using RecomeceAPI.Server;
using RecomeceAPI.Interfaces;

namespace RecomeceAPI.Controllers
{
  [Route("api/[controller]")]
  public class CadEmpresasController : ControllerBase
  {
    private readonly CadEmpresasService _service;
    private readonly HandleCatchService _handleCatchService;
    private readonly ICryptService _cyptService;

    public CadEmpresasController(CadEmpresasService service, HandleCatchService handleCatchService, ICryptService cyptService)
    {
      _service = service;
      _handleCatchService = handleCatchService;
      _cyptService = cyptService;
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
    public async Task<IActionResult> GetAllCompany()
    {
      try
      {
        var pagination = await _service.GetAllCompany();

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
        var model = new CadEmpresasModel();
        HandleJsonService.SetByJson(json.ToString() ?? "", model);
        if(model.WhiteLabelConfig.ValorNomeLote == 0)
        {
          return Ok(NotificationService.Validation("Necessário escolher o valor do custo do nome no lote"));
        }
        var isExists = await _service.GetByInscricaoAsync(model.Inscricao);
        if(isExists.Id > 0)
        {
          return Ok(NotificationService.Validation("Esta empresa já existe em nossa base de dados"));
        }
        model.Servidor = byte.Parse(Configs.Servers().First().Key.ToString());
        model.BancoDados = byte.Parse(Configs.Databases().First().Key.ToString());
        var isDone = await _service.SaveAsync(model);
        return Ok(HandleJsonService.GetJson(isDone));
      }
      catch (Exception ex)
      {
        _handleCatchService.HandleCatch(ex);
        return Ok(NotificationService.Error());
      }
    }
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] CadEmpresasModel model)
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
    [HttpPost("UpdateWhiteLabelConfig")]
    public async Task<IActionResult> UpdateWhiteLabelConfig([FromBody] WhiteLabelConfigModel config)
    {
      try
      {
        await _service.UpdateWhiteLabelConfig(config);
        return Ok(true);
      }
      catch (Exception ex)
      {
        _handleCatchService.HandleCatch(ex);
        return Ok(NotificationService.Error());
      }
    }
    [HttpGet("Decifer")]
    public IActionResult Decifer(string content)
    {
      var item = _cyptService.Decrypt(content);
      return Ok(true);
    }
  }
}