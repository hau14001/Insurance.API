using System;
using System.Threading.Tasks;
using Insurance.Domain.Common;
using Microsoft.AspNetCore.Http;

namespace Insurance.Application.UploadFiles
{
    public interface IUploadFileService
    {
        Task<ServiceResponse> GetAllAsync();

        Task<ServiceResponse> GetById(Guid id);

        Task<ServiceResponse> CreateAsync(IFormFile file);

        Task<ServiceResponse> Delete(Guid id);
    }
}