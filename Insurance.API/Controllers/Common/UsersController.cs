using System;
using System.Threading.Tasks;
using Insurance.API.Controllers.Common.Models.Users;
using Insurance.Application.Common.Users;
using Insurance.Application.Common.Users.Dtos;
using Insurance.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Insurance.API.Controllers.Common
{
    [ApiController]
    [Route("v1/api/users")]
    [AllowAnonymous]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService) => _userService = userService;

        [HttpPost]
        public virtual async Task<ServiceResponse> AddUser(CreateUserModel model) => await _userService.AddAsync(new CreateUserRequest
        {
            Email = model.Email,
            Password = model.Password,
            PermissionIds = model.PermissionIds,
            PhoneNumber = model.PhoneNumber,
            RoleIds = model.RoleIds,
            FullName = model.FullName,
            IsLock = model.IsLock,
            ExpiredDate = model.ExpiredDate,
            Code = model.Code
        });

        [HttpPut]
        public virtual async Task<ServiceResponse> UpdateUser(CreateUserModel model) => await _userService.UpdateAsync(new CreateUserRequest
        {
            Email = model.Email,
            Password = model.Password,
            PermissionIds = model.PermissionIds,
            PhoneNumber = model.PhoneNumber,
            RoleIds = model.RoleIds,
            FullName = model.FullName,
            IsLock = model.IsLock,
            ExpiredDate = model.ExpiredDate,
            Code = model.Code
        });

        [HttpDelete("{id}")]
        public async Task<ServiceResponse> DeleteUser(Guid id)
        {
            return await _userService.DeleteAsync(id);
        }

        [HttpPut("{id}/reset-password")]
        public async Task<ServiceResponse> ResetPassword(Guid id)
        {
            return await _userService.ResetPasswordAsync(id);
        }

        [HttpPut("change-password")]
        public async Task<ServiceResponse> ChangePassword(ChangePasswordUserRequest request)
        {
            return await _userService.ChangePasswordUser(request);
        }

        [HttpGet("{id}/permissions")]
        public async Task<ServiceResponse> GetPermissions(Guid id) => await _userService.GetPermissionsAsync(id);

        [HttpGet("{id}/full-permissions")]
        public async Task<ServiceResponse> GetFullPermissions(Guid id) => await _userService.GetFullPermissionsAsync(id);

        [HttpGet("{id}/permissions-not-in")]
        public async Task<ServiceResponse> GetPermissionsNotInUser(Guid id) => await _userService.GetPermissionsNotInUserAsync(id);

        [HttpGet("{id}/roles")]
        public async Task<ServiceResponse> GetRoles(Guid id) => await _userService.GetRolesAsync(id);

        [HttpGet("{id}/roles-not-in")]
        public async Task<ServiceResponse> GetRolesNotInUser(Guid id) => await _userService.GetRolesNotInUserAsync(id);

        [HttpGet("{id}")]
        public async Task<ServiceResponse> GetUser(Guid id) => await _userService.GetAsync(id);

        [HttpGet("search")]
        public async Task<ServiceResponse> GetUsers([FromQuery] SearchRequest request) => await _userService.FindAsync(request ?? new SearchRequest());

        [HttpPut("{id}/email")]
        public virtual async Task<ServiceResponse> UpdateUserEmail(Guid id, UpdateUserEmailModel model) => await _userService.UpdateEmailAsync(new UpdateEmailRequest
        {
            Id = id,
            Email = model.Email
        });

        [HttpPut("{id}/password")]
        public virtual async Task<ServiceResponse> UpdateUserPassword(Guid id, UpdateUserPasswordModel model) => await _userService.UpdatePasswordAsync(new UpdatePasswordRequest
        {
            Id = id,
            Password = model.Password
        });

        [HttpPut("{id}/permissions")]
        public virtual async Task<ServiceResponse> UpdateUserPermissions(Guid id, UpdateUserPermisisonsModel model) => await _userService.UpdatePermissionsAsync(new UpdatePermissionsForUserRequest
        {
            Id = id,
            PermissionIds = model.PermissionIds,
            DeletePermissionIds = model.DeletePermissionIds
        });

        [HttpPut("{id}/roles")]
        public virtual async Task<ServiceResponse> UpdateUserRoles(Guid id, UpdateUserRolesModel model) => await _userService.UpdateRolesAsync(new UpdateRolesForUserRequest
        {
            Id = id,
            RoleIds = model.RoleIds,
            DeleteRoleIds = model.DeleteRoleIds
        });

        [HttpPut("{idUser}/set-one-permission/{idPermission}")]
        public virtual async Task<ServiceResponse> UpdateUserRoles(Guid idUser, Guid idPermission)
        {
            return await _userService.UpdateOnePermissionsAsync(idUser, idPermission);
        }
    }
}