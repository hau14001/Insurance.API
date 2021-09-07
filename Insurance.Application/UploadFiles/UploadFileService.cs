using System;
using System.Threading.Tasks;
using Insurance.Application.Services;
using Insurance.Domain.Common;
using Insurance.Infrastructure.Data.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Insurance.Application.UploadFiles
{
    public class UploadFileService : Service, IUploadFileService
    {
        private readonly ILogger<UploadFileService> _logger;
        private readonly IRepository _repository;

        public UploadFileService(IRepository repository, ILogger<UploadFileService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<ServiceResponse> CreateAsync(IFormFile file)
        {
            if (file != null)
            {
                var entity = await Be.Library.FileHelper.UploadFile("Uploads/File", file);
                var fileModel = new BaseFileEntity
                {
                    FileExtension = entity.FileExtension,
                    FileName = entity.FileName,
                    OriginalFileName = entity.OriginalFileName,
                    Path = entity.Path,
                    Size = entity.Size
                };

                await _repository.AddAsync(fileModel);
                await _repository.SaveChangeAsync();
                return Ok(new
                {
                    fileModel.FileName,
                    Url = fileModel.Path.ToUrl(),
                    Uploaded = 1
                });
            }

            return BadRequest("Error", "Null");
        }

        public async Task<ServiceResponse> Delete(Guid id)
        {
            await _repository.DeleteAsync<Domain.Common.BaseFileEntity>(id);
            await _repository.SaveChangeAsync();
            return Ok();
        }

        public async Task<ServiceResponse> GetAllAsync()
        {
            return base.Ok(await _repository.FindAllAsync<Domain.Common.BaseFileEntity>());
        }

        public async Task<ServiceResponse> GetById(Guid id)
        {
            return base.Ok(await _repository.FindAsync<Domain.Common.BaseFileEntity>(id));
        }
    }
}