using System;
using System.Threading.Tasks;
using Insurance.Application.UploadFiles;
using Insurance.Domain.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Insurance.API.Controllers.Common
{
    [Route("v1/api/[controller]")]
    [ApiController]
    public class UploadFileController : ControllerBase
    {
        private readonly IUploadFileService _service;

        public UploadFileController(IUploadFileService service)
        {
            _service = service;
        }

        [HttpGet("download")]
        public async Task<IActionResult> Download()
        {
            var b = await System.IO.File.ReadAllBytesAsync("Uploads/Command/CAD.png");
            return File(b, "application/pdf", "CAD.png");
        }

        [HttpGet]
        public async Task<ServiceResponse> GetAll()
        {
            return await _service.GetAllAsync();
        }

        [HttpGet("{id}")]
        public async Task<ServiceResponse> Get(Guid id)
        {
            return await _service.GetById(id);
        }

        [HttpPost]
        public async Task<ServiceResponse> Post(IFormFile file)
        {
            return await _service.CreateAsync(file);
        }

        [HttpDelete("{id}")]
        public async Task<ServiceResponse> Delete(Guid id)
        {
            return await _service.Delete(id);
        }
    }
}