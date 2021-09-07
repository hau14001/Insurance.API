using System.Threading.Tasks;
using Insurance.Application.Common.Permissions;
using Insurance.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Insurance.API.Controllers.Common
{
    [ApiController]
    [Route("v1/api/permissions")]
    [AllowAnonymous]
    public class PermissionsController : ControllerBase
    {
        private readonly IPermissionService _permissionService;

        public PermissionsController(IPermissionService permissionService) => _permissionService = permissionService;

        [HttpGet("predefine")]
        public ServiceResponse GetPredefinePermissions() => _permissionService.GetPredefinePermissions();

        [HttpGet("search")]
        public async Task<ServiceResponse> GetPermissions([FromQuery] SearchRequest request)
        {
            return await _permissionService.FindAsync(request ?? new SearchRequest());
        }

        [HttpGet("all")]
        public async Task<ServiceResponse> GetAll()
        {
            return await _permissionService.FindAsync(new SearchRequest() { PageSize = 1000 });
        }
    }
}