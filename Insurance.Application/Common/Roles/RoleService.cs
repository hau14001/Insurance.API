using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Insurance.Application.Common.Roles.Dtos;
using Insurance.Application.Services;
using Insurance.Domain.Common;
using Insurance.Domain.Entities.BaseEntity;
using Insurance.Infrastructure.Data;
using Insurance.Infrastructure.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace Insurance.Application.Common.Roles
{
    public class RoleService<TUser, TRole, TUserRole> : Service, IRoleService
        where TUser : User, new()
        where TRole : Role, new()
        where TUserRole : UserRole, new()
    {
        private readonly IRepository _repository;

        public RoleService(IRepository repository) => _repository = repository;

        public virtual async Task<ServiceResponse> AddAsync(CreateRoleRequest request)
        {
            var currentRoles = await _repository.FindAllAsync<TRole>(r => r.NormalizedName == request.Name.ToUpper());
            if (currentRoles.FirstOrDefault() != null)
            {
                return BadRequest("role_name_already_exist", "This role name is already exists.");
            }

            var displayName = request.DisplayName.IsNullOrEmpty() ? request.Name : request.DisplayName;
            var role = new TRole { Name = request.Name, NormalizedName = request.Name.ToUpper(), DisplayName = displayName };

            await _repository.AddAsync(role);
            await _repository.SaveChangeAsync();

            if (request?.Permissions?.Any() == true)
            {
                await AddPermissionsToRoleAsync(role.Id, request.Permissions);
                await _repository.SaveChangeAsync();
            }

            return Ok(GetRoleResponse<TUser, TRole>.FromEntity(role));
        }

        public virtual async Task<ServiceResponse> DeleteAsync(Guid id)
        {
            var role = await _repository.FindAsync<TRole>(id);
            return await DeleteRoleInternalAsync(role);
        }

        public virtual async Task<ServiceResponse> DeleteAsync(string name)
        {
            if (name is null)
            {
                return RoleNotFound();
            }

            var roles = await _repository.FindAllAsync<TRole>(r => r.NormalizedName == name.ToUpper());
            return await DeleteRoleInternalAsync(roles?.FirstOrDefault());
        }

        public virtual async Task<ServiceResponse> FindAsync(SearchRequest request)
        {
            var query = _repository.GetQueryable<TRole>();
            if (!request.FilterBy.IsNullOrEmpty())
            {
                query = query.Where(r => r.Name.Contains(request.FilterBy));
            }

            var result = await _repository.FindPagedAsync(query, request.PageIndex, request.PageSize);

            return Ok(result.ChangeType(GetRoleResponse<TUser, TRole>.FromEntity));
        }

        public async Task<ServiceResponse> GetAll()
        {
            var roles = await _repository.FindAllAsync<Role>();
            return Ok(roles.Select(x => new { Id = x.Id, Name = x.Name }));
        }

        public virtual async Task<ServiceResponse> GetAsync(GetUsersInRoleRequest request)
        {
            var role = await GetRoleQuery(request.RoleId).FirstOrDefaultAsync();
            if (role == null)
            {
                return RoleNotFound();
            }

            var getUserResponse = await GetUsersAsync(request);
            role.Users = getUserResponse.GetData<PagedResult<TUser>>();

            return Ok(role);
        }

        public virtual async Task<ServiceResponse> GetPermissionsAsync(Guid roleId) => Ok(await GetPermissionQuery(roleId).ToListAsync());

        public virtual async Task<ServiceResponse> GetPermissionsNotInRoleAsync(Guid roleId) => Ok(await GetPermissionsNotInRoleQuery(roleId).ToListAsync());

        public virtual async Task<ServiceResponse> GetUsersAsync(GetUsersInRoleRequest request) => Ok(await GetPagedUserAsync(GetUsersInRoleQuery(request.RoleId), request.UserSearch));

        public virtual async Task<ServiceResponse> GetUsersNotInRoleAsync(GetUsersInRoleRequest request) => Ok(await GetPagedUserAsync(GetUsersNotInRoleQuery(request.RoleId), request.UserSearch));

        public virtual async Task<ServiceResponse> UpdateNameAsync(UpdateNameRequest request)
        {
            var role = await _repository.FindAsync<TRole>(request.Id);
            if (role == null)
            {
                return RoleNotFound();
            }

            role.Name = request.Name;
            role.NormalizedName = request.Name.ToUpper();
            if (!request.DisplayName.IsNullOrEmpty())
            {
                role.DisplayName = request.DisplayName;
            }

            await _repository.UpdateAsync(role);
            await _repository.SaveChangeAsync();

            return Ok(role);
        }

        public virtual async Task<ServiceResponse> UpdatePermissionsAsync(UpdatePermissionsForRoleRequest request)
        {
            var role = await _repository.FindAsync<TRole>(request.Id);
            if (role == null)
            {
                return RoleNotFound();
            }

            if (request.DeletePermissionIds?.Any() == true)
            {
                await DeletePermissionsFromRole(request);
            }

            if (request.PermissionIds?.Any() == true)
            {
                await AddPermissionsToRoleAsync(role.Id, request.PermissionIds);
            }

            await _repository.UpdateAsync(role);
            await _repository.SaveChangeAsync();

            return Ok(GetRoleResponse<TUser, TRole>.FromEntity(role));
        }

        public virtual async Task<ServiceResponse> UpdateUsersAsync(UpdateUsersForRoleRequest request)
        {
            var role = await _repository.FindAsync<TRole>(request.Id);
            if (role == null)
            {
                return RoleNotFound();
            }

            if (request.UserIds?.Any() == true)
            {
                await AddUsersToRole(request);
            }

            if (request.DeleteUserIds?.Any() == true)
            {
                await DeleteUsersFromRole(request);
            }

            await _repository.UpdateAsync(role);
            await _repository.SaveChangeAsync();

            return Ok(GetRoleResponse<TUser, TRole>.FromEntity(role));
        }

        private async Task AddPermissionsToRoleAsync(Guid roleId, IEnumerable<Guid> permissionIds)
        {
            var currentPermissionIds = _repository.GetQueryable<RolePermission>()
                   .Where(rolePermission => rolePermission.RoleId == roleId)
                   .Select(rolePermission => rolePermission.PermissionId);
            var newPermissionIds = permissionIds.Where(permisisonId => currentPermissionIds.All(currentPermissionId => currentPermissionId != permisisonId));

            var rolePermissions = newPermissionIds.Select(pId => new RolePermission { PermissionId = pId, RoleId = roleId });
            await _repository.AddRangeAsync(rolePermissions);
        }

        private async Task AddUsersToRole(UpdateUsersForRoleRequest request)
        {
            var currentUserIds = _repository.GetQueryable<TUserRole>()
                   .Where(useRole => useRole.RoleId == request.Id)
                   .Select(userRole => userRole.UserId);
            var newUserIds = request.UserIds.Where(userId => currentUserIds.All(currentRoleId => currentRoleId != userId));

            var userRoles = newUserIds.Select(userId => new TUserRole { RoleId = request.Id, UserId = userId });
            await _repository.AddRangeAsync(userRoles);
        }

        private async Task DeletePermissionsFromRole(UpdatePermissionsForRoleRequest request)
        {
            var currentRolePermissions = await _repository
                .FindAllAsync<RolePermission>(rolePermission => rolePermission.RoleId == request.Id && request.DeletePermissionIds.Contains(rolePermission.PermissionId));
            await _repository.DeleteRangeAsync(currentRolePermissions);
        }

        private async Task<ServiceResponse> DeleteRoleInternalAsync(TRole role)
        {
            if (role == null)
            {
                return RoleNotFound();
            }

            if (role.IsSystemRole)
            {
                return BadRequest("system_role_can_not_be_removed", "Cannot delete system role.");
            }

            await _repository.DeleteAsync(role);
            await _repository.SaveChangeAsync();

            return Ok();
        }

        private async Task DeleteUsersFromRole(UpdateUsersForRoleRequest request)
        {
            foreach (var deleteUserId in request.DeleteUserIds)
            {
                await _repository.DeleteAsync<TUserRole>(new object[] { deleteUserId, request.Id });
            }
        }

        private async Task<PagedResult<TUser>> GetPagedUserAsync(IQueryable<TUser> query, SearchRequest request)
        {
            if (!request.FilterBy.IsNullOrEmpty())
            {
                query = query.Where(user => user.UserName.Contains(request.FilterBy));
            }

            query = query.OrderBy(x => x.UserName);

            var pageIndex = request.PageIndex <= 0 ? 1 : request.PageIndex;
            var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;
            pageIndex -= 1;
            var skip = pageIndex * pageSize;

            query = query.AsNoTracking();
            query = skip == 0 ? query.Take(pageSize) : query.Skip(skip).Take(pageSize);

            return new PagedResult<TUser>
            {
                PageIndex = pageIndex + 1,
                PageSize = pageSize,
                Items = await query.ToListAsync(),
                TotalCount = await query.CountAsync()
            };
        }

        private IQueryable<Permission> GetPermissionQuery(Guid roleId)
        {
            var permissions = _repository.GetQueryable<Permission>();
            var rolePermissions = _repository.GetQueryable<RolePermission>();

            return rolePermissions
                .Where(rolePermission => rolePermission.RoleId == roleId)
                .Join(permissions, rolePermission => rolePermission.PermissionId, permission => permission.Id, (rolePermission, permission) => permission)
                .Distinct();
        }

        private IQueryable<Permission> GetPermissionsNotInRoleQuery(Guid roleId)
        {
            var permissions = _repository.GetQueryable<Permission>();
            var rolePermissions = _repository.GetQueryable<RolePermission>();

            var permissionIdsInRole = rolePermissions
                .Where(rolePermission => rolePermission.RoleId == roleId)
                .Join(permissions, rolePermission => rolePermission.PermissionId, permission => permission.Id, (rolePermission, permission) => permission.Id)
                .Distinct();

            return permissions.Where(permission => permissionIdsInRole.All(permisisonInRoleId => permisisonInRoleId != permission.Id));
        }

        private IQueryable<GetRoleResponse<TUser, TRole>> GetRoleQuery(Guid id)
        {
            return _repository.GetQueryable<TRole>()
                .Where(role => role.Id == id)
                .Select(role => new GetRoleResponse<TUser, TRole>
                {
                    Role = role,
                    Permissions = GetPermissionQuery(id)
                });
        }

        private IQueryable<TUser> GetUsersInRoleQuery(Guid roleId)
        {
            var users = _repository.GetQueryable<TUser>();
            var userRoles = _repository.GetQueryable<TUserRole>();

            return userRoles
                .Where(userRole => userRole.RoleId == roleId)
                .Join(users, userRole => userRole.UserId, user => user.Id, (userRole, user) => user);
        }

        private IQueryable<TUser> GetUsersNotInRoleQuery(Guid roleId)
        {
            var users = _repository.GetQueryable<TUser>();
            var userRoles = _repository.GetQueryable<TUserRole>();

            var userIdsInRole = userRoles
                .Where(userRole => userRole.RoleId == roleId)
                .Join(users, userRole => userRole.UserId, user => user.Id, (userRole, user) => user.Id);

            return users.Where(user => userIdsInRole.All(userInRoleId => userInRoleId != user.Id));
        }

        private ServiceResponse RoleNotFound() => BadRequest("role_not_found", "Role not found.");
    }
}