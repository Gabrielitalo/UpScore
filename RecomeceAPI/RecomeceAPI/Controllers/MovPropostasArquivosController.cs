using Microsoft.AspNetCore.Mvc;
using RecomeceAPI.Services;
using RecomeceAPI.Services.Common;
using RecomeceAPI.Models;
using QuestPDF.Fluent;
using Microsoft.AspNetCore.Http.HttpResults;

namespace RecomeceAPI.Controllers
{
    [Route("api/[controller]")]
    public class MovPropostasArquivosController : ControllerBase
    {
        private readonly MovPropostasArquivosService _service;
        private readonly HandleCatchService _handleCatchService;

        public MovPropostasArquivosController(MovPropostasArquivosService service, HandleCatchService handleCatchService)
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
        [HttpGet("GetAllProposal")]
        public async Task<IActionResult> GetAllProposal([FromQuery] int proposal)
        {
            try
            {
                var pagination = await _service.GetAllProposal(proposal);

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
        [HttpGet("GetContractProposal")]
        public async Task<IActionResult> GetContractProposal([FromQuery] int proposal)
        {
            try
            {
                var pagination = await _service.GetContractProposal(proposal);

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
                var model = new MovPropostasArquivosModel();
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
        public async Task<IActionResult> Update([FromBody] MovPropostasArquivosModel model)
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

        [HttpGet("DownloadArquivo")]
        public async Task<IActionResult> DownloadArquivo(long id)
        {
            try
            {
                var file = await _service.GetByIdAsync(id);
                if (file.Id > 0)
                {
                    var stream = new MemoryStream();
                    stream.Position = 0;
                    return File(file.ArquivoBytes, "application/pdf", file.NomeArquivo);
                }
                else
                    return Ok(NotificationService.Error());

            }
            catch (Exception ex)
            {
                _handleCatchService.HandleCatch(ex);
                return Ok(NotificationService.Error());
            }
        }
    }
}