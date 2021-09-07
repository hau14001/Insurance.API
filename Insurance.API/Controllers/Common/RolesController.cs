using System;
using System.Threading.Tasks;
using Insurance.API.Controllers.Common.Models.Roles;
using Insurance.Application.Common.Roles;
using Insurance.Application.Common.Roles.Dtos;
using Insurance.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Insurance.API.Controllers.Common
{
    [ApiController]
    [Route("v1/api/roles")]
    [AllowAnonymous]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService) => _roleService = roleService;

        [HttpPost]
        public async Task<ServiceResponse> AddRole(CreateRoleModel model) => await _roleService.AddAsync(new CreateRoleRequest
        {
            Name = model.Name,
            DisplayName = model.DisplayName,
            Permissions = model.Permissions
        });

        [HttpDelete]
        public async Task<ServiceResponse> DeleteRole(Guid id) => await _roleService.DeleteAsync(id);

        [HttpGet("{id}/permissions")]
        public async Task<ServiceResponse> GetPermissions(Guid id) => await _roleService.GetPermissionsAsync(id);

        [HttpGet("{id}/permissions-not-in")]
        public async Task<ServiceResponse> GetPermissionsNotInRole(Guid id) => await _roleService.GetPermissionsNotInRoleAsync(id);

        [HttpGet("{id}")]
        public async Task<ServiceResponse> GetRole(Guid id, [FromQuery] GetUsersInRoleModel model) => await _roleService.GetAsync(new GetUsersInRoleRequest
        {
            RoleId = id,
            UserSearch = model.UserSearch ?? new SearchRequest()
        });

        [HttpGet("search")]
        public async Task<ServiceResponse> GetRoles([FromQuery] SearchRequest request)
        {
            return await _roleService.FindAsync(request ?? new SearchRequest() { PageSize = 1000 });
        }

        [HttpGet("all")]
        public async Task<ServiceResponse> GetAll()
        {
            return await _roleService.GetAll();
        }

        [HttpGet("{id}/users")]
        public async Task<ServiceResponse> GetUsers(Guid id, [FromQuery] GetUsersInRoleModel model) => await _roleService.GetUsersAsync(new GetUsersInRoleRequest
        {
            RoleId = id,
            UserSearch = model.UserSearch ?? new SearchRequest()
        });

        [HttpGet("{id}/users-not-in")]
        public async Task<ServiceResponse> GetUsersNotInRole(Guid id, [FromQuery] GetUsersInRoleModel model) => await _roleService.GetUsersNotInRoleAsync(new GetUsersInRoleRequest
        {
            RoleId = id,
            UserSearch = model.UserSearch ?? new SearchRequest()
        });

        [HttpPut("{id}/name")]
        public async Task<ServiceResponse> UpdateRoleName(Guid id, UpdateRoleNameModel model) => await _roleService.UpdateNameAsync(new UpdateNameRequest
        {
            Id = id,
            Name = model.Name,
            DisplayName = model.DisplayName
        });

        [HttpPut("{id}/permissions")]
        public async Task<ServiceResponse> UpdateRolePermissions(Guid id, UpdateRolePermisisonsModel model) => await _roleService.UpdatePermissionsAsync(new UpdatePermissionsForRoleRequest
        {
            Id = id,
            PermissionIds = model.PermissionIds,
            DeletePermissionIds = model.DeletePermissionIds
        });

        [HttpPut("{id}/users")]
        public async Task<ServiceResponse> UpdateRoleUsers(Guid id, UpdateUsersModel model) => await _roleService.UpdateUsersAsync(new UpdateUsersForRoleRequest
        {
            Id = id,
            UserIds = model.UserIds,
            DeleteUserIds = model.DeleteUserIds
        });
    }
}