using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Be.Library;
using Insurance.Application.Common.Users.Dtos;
using Insurance.Application.Services;
using Insurance.Domain.Common;
using Insurance.Domain.Entities.BaseEntity;
using Insurance.Infrastructure.Data.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Insurance.Application.Common.Users
{
    public class UserService<TUser, TRole, TUserRole> : Service, IUserService
        where TUser : User, new()
        where TRole : Role, new()
        where TUserRole : UserRole, new()
    {
        private readonly IRepository _repository;

        public UserService(IRepository repository) => _repository = repository;

        public virtual async Task<ServiceResponse> AddAsync(CreateUserRequest request)
        {
            var currentUsers = await _repository.FindAllAsync<TUser>(u => u.NormalizedUserName == request.Email.ToUpper());
            if (currentUsers.FirstOrDefault() != null)
            {
                return BadRequest("username_already_exist", "This username is already exist!");
            }

            var user = new TUser
            {
                UserName = request.Email,
                Email = request.Email,
                NormalizedEmail = request.Email.ToUpper(),
                NormalizedUserName = request.Email.ToUpper(),
                FullName = request.FullName,
                IsLock = request.IsLock,
                Code = request.Code,
                PhoneNumber = request.PhoneNumber,
                ExpiredDate = request.ExpiredDate,
                BPassword = BCrypt.Net.BCrypt.HashPassword(request.Password),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };

            try
            {
                await _repository.AddAsync(user);
                await _repository.SaveChangeAsync();
            }
            catch (Exception e)
            {
                var a = e.Message;
            }

            if (request.RoleIds != null && request.RoleIds.Any())
            {
                await AddRoleToUserAsync(user.Id, request.RoleIds);
            }

            if (request.PermissionIds != null && request.PermissionIds?.Any() == true)
            {
                await AddPermissionsToUserAsync(user.Id, request.PermissionIds);
            }

            await _repository.SaveChangeAsync();

            return Ok(GetUserResponse<TUser, TRole>.FromEntity(user));
        }

        public async Task<ServiceResponse> UpdateAsync(CreateUserRequest request)
        {
            var user = await _repository.FindAsync<User>(x => x.NormalizedEmail == request.Email.ToUpper());

            if (user == null)
            {
                return NotFound("", "Không tồn tại user");
            }

            user.FullName = request.FullName;
            user.IsLock = request.IsLock;
            user.ExpiredDate = request.ExpiredDate;
            user.PhoneNumber = request.PhoneNumber;
            user.Code = request.Code;

            await _repository.UpdateAsync(user);
            await _repository.SaveChangeAsync();

            return Ok();
        }

        public virtual async Task<ServiceResponse> DeleteAsync(Guid id)
        {
            var user = await _repository.FindAsync<TUser>(id);

            if (user == null)
            {
                return BadRequest("user_not_found", "User not found.");
            }

            user.UserName += user.Id;
            user.NormalizedUserName += user.Id;
            user.Email += user.Id;
            user.NormalizedEmail += user.Id;
            await _repository.UpdateAsync(user);

            if (user.IsSystemUser)
            {
                return BadRequest("", "Cannot delete system user.");
            }

            await _repository.DeleteAsync<TUser>(user.Id);
            await _repository.SaveChangeAsync();

            return Ok();
        }

        public async Task<ServiceResponse> ResetPasswordAsync(Guid id)
        {
            var user = await _repository.FindAsync<User>(id);
            if (user == null)
            {
                return NotFound();
            }

            user.BPassword = BCrypt.Net.BCrypt.HashPassword("123456");
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456");

            await _repository.UpdateAsync(user);
            await _repository.SaveChangeAsync();

            return Ok();
        }

        public async Task<ServiceResponse> SetAdmin(Guid id)
        {
            var user = await _repository.FindAsync<User>(id);
            if (user == null)
            {
                return NotFound();
            }

            user.IsSystemUser = true;

            await _repository.UpdateAsync(user);
            await _repository.SaveChangeAsync();

            return Ok();
        }

        public async Task<ServiceResponse> ChangePasswordUser(ChangePasswordUserRequest request)
        {
            var user = await _repository.FindAsync<User>(request.Id);
            if (user == null)
            {
                return NotFound("", "Không tồn tại user");
            }

            user.BPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            await _repository.UpdateAsync(user);
            await _repository.SaveChangeAsync();

            return Ok();
        }

        public virtual async Task<ServiceResponse> FindAsync(SearchRequest request)
        {
            var query = _repository.GetQueryable<TUser>();

            if (!request.FilterBy.IsNullOrEmpty())
            {
                query = query.Where(u => u.NormalizedUserName.Contains(request.FilterBy.ToUpper()) || u.NormalizedEmail.Contains(request.FilterBy.ToUpper()) || u.PhoneNumber.Contains(request.FilterBy.ToUpper()));
            }

            query = query.OrderBy(x => x.UserName);

            var result = await _repository.FindPagedAsync(query, request.PageIndex, request.PageSize);

            var users = result.Items;
            var datas = (from user in users
                         join up in _repository.GetQueryable<UserPermission>() on user.Id equals up.UserId
                             into ups
                         from up in ups.DefaultIfEmpty()
                         join p in _repository.GetQueryable<Permission>() on up?.PermissionId equals p.Id
                             into ps
                         select new UserResponse()
                         {
                             Id = user.Id,
                             FullName = user.FullName,
                             Code = user.Code,
                             Email = user.Email,
                             PhoneNumber = user.PhoneNumber,
                             Permissions = ps.Select(x => new BaseDto() { Id = x.Id, Name = x.Name }).ToList()
                         }).ToList();

            return Ok(result.ChangeData(datas.ToList()));
        }

        public virtual async Task<ServiceResponse> GetAsync(Guid id)
        {
            var queryUser = _repository.GetQueryable<TUser>()
                .Where(user => user.Id == id)
                .Select(user => new GetUserResponse<TUser, TRole>
                {
                    User = user,
                    Roles = GetQueryableRole(id),
                    Permissions = GetQueryablePermission(id)
                });

            var u = await queryUser.FirstOrDefaultAsync();
            if (u == null)
            {
                return UserNotFound();
            }

            return Ok(u);
        }

        public virtual async Task<ServiceResponse> GetPermissionsAsync(Guid userId) => Ok(await GetQueryablePermission(userId).ToListAsync());

        public async Task<ServiceResponse> GetFullPermissionsAsync(Guid userId)
        {
            try
            {
                return Ok(await GetQueryableFullPermission(userId));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        public virtual async Task<ServiceResponse> GetPermissionsNotInUserAsync(Guid userId) => Ok(await GetQueryablePermissionsNotInUser(userId).ToListAsync());

        public virtual async Task<ServiceResponse> GetRolesAsync(Guid userId) => base.Ok(await GetQueryableRole(userId).ToListAsync());

        public virtual async Task<ServiceResponse> GetRolesNotInUserAsync(Guid userId) => Ok(await GetQueryableRolesNotInUser(userId).ToListAsync());

        public virtual async Task<ServiceResponse> UpdateEmailAsync(UpdateEmailRequest request)
        {
            var user = await _repository.FindAsync<TUser>(request.Id);
            if (user == null)
            {
                return UserNotFound();
            }

            user.Email = request.Email;
            user.NormalizedEmail = user.Email.ToUpper();
            user.SecurityStamp = Guid.NewGuid().ToString();
            await _repository.UpdateAsync(user);
            await _repository.SaveChangeAsync();

            return Ok(user);
        }

        public virtual async Task<ServiceResponse> UpdatePasswordAsync(UpdatePasswordRequest request)
        {
            var user = await _repository.FindAsync<TUser>(request.Id);
            if (user == null)
            {
                return UserNotFound();
            }

            user.PasswordHash = new PasswordHasher<TUser>().HashPassword(user, request.Password);
            user.SecurityStamp = Guid.NewGuid().ToString();
            await _repository.UpdateAsync(user);
            await _repository.SaveChangeAsync();

            return Ok(user);
        }

        public virtual async Task<ServiceResponse> UpdatePermissionsAsync(UpdatePermissionsForUserRequest request)
        {
            var user = await _repository.FindAsync<TUser>(request.Id);
            if (user == null)
            {
                return UserNotFound();
            }

            if (request.PermissionIds?.Any() == true)
            {
                await AddPermissionsToUserAsync(user.Id, request.PermissionIds);
            }

            if (request.DeletePermissionIds?.Any() == true)
            {
                await DeletePermissionsFromUser(request);
            }

            await _repository.UpdateAsync(user);
            await _repository.SaveChangeAsync();

            return Ok(GetUserResponse<TUser, TRole>.FromEntity(user));
        }

        public virtual async Task<ServiceResponse> UpdateRolesAsync(UpdateRolesForUserRequest request)
        {
            var user = await _repository.FindAsync<TUser>(request.Id);
            if (user == null)
            {
                return UserNotFound();
            }

            if (request.RoleIds?.Any() == true)
            {
                await AddRoleToUserAsync(user.Id, request.RoleIds);
            }

            if (request.DeleteRoleIds?.Any() == true)
            {
                await DeleteRolesFromUser(request);
            }

            await _repository.UpdateAsync(user);
            await _repository.SaveChangeAsync();

            return Ok();
        }

        public async Task<ServiceResponse> UpdateOnePermissionsAsync(Guid idUser, Guid idPermission)
        {
            var user = await _repository.FindAsync<User>(idUser);
            if (user == null)
            {
                return NotFound();
            }

            var up = await _repository.FindAllAsync<UserPermission>(x => x.UserId == idUser);
            await _repository.DeleteRangeAsync(up);
            await _repository.SaveChangeAsync();
            await _repository.AddAsync(new UserPermission()
            {
                UserId = idUser,
                PermissionId = idPermission
            });

            await _repository.SaveChangeAsync();

            return Ok();
        }

        private async Task AddPermissionsToUserAsync(Guid userId, IEnumerable<Guid> permissionIds)
        {
            var currentPermissionIds = _repository.GetQueryable<UserPermission>()
                .Where(userPermission => userPermission.UserId == userId)
                .Select(userPermission => userPermission.PermissionId);
            var newPermissionIds = permissionIds.Where(permisisonId => currentPermissionIds.All(currentPermissionId => currentPermissionId != permisisonId));

            var userPermissions = newPermissionIds.Select(permissionId => new UserPermission { PermissionId = permissionId, UserId = userId });
            await _repository.AddRangeAsync(userPermissions);
        }

        private async Task AddRoleToUserAsync(Guid userId, IEnumerable<Guid> roleIds)
        {
            var currentRoleIds = _repository.GetQueryable<TUserRole>()
                .Where(useRole => useRole.UserId == userId)
                .Select(userRole => userRole.RoleId);
            var newRoleIds = roleIds.Where(roleId => currentRoleIds.All(currentRoleId => currentRoleId != roleId));

            var userRoles = newRoleIds.Select(roleId => new TUserRole { RoleId = roleId, UserId = userId });
            await _repository.AddRangeAsync(userRoles);
        }

        private async Task DeletePermissionsFromUser(UpdatePermissionsForUserRequest request)
        {
            var currentUserPermissions = await _repository
                .FindAllAsync<UserPermission>(userPermission => userPermission.UserId == request.Id && request.DeletePermissionIds.Contains(userPermission.PermissionId));
            await _repository.DeleteRangeAsync(currentUserPermissions);
        }

        private async Task DeleteRolesFromUser(UpdateRolesForUserRequest request)
        {
            foreach (var deleteRoleId in request.DeleteRoleIds)
            {
                await _repository.DeleteAsync<TUserRole>(new object[] { request.Id, deleteRoleId });
            }
        }

        private IQueryable<Permission> GetQueryablePermission(Guid userId)
        {
            var permissions = _repository.GetQueryable<Permission>();
            var userPermissions = _repository.GetQueryable<UserPermission>();

            return userPermissions
                .Where(up => up.UserId == userId)
                .Join(permissions, userPermission => userPermission.PermissionId, permission => permission.Id, (userPermission, permission) => permission);
        }

        private async Task<List<string>> GetQueryableFullPermission(Guid userId)
        {
            var permissions = await GetQueryablePermission(userId).ToListAsync();

            var roles = await GetQueryableRole(userId).ToListAsync();
            var list = new List<string>();
            list.AddRange(permissions.Select(x => x.Name));
            foreach (var role in roles)
            {
                var permissionIds =
                    (await _repository.FindAllAsync<RolePermission>(x => x.RoleId == role.Id)).Select(x =>
                        x.PermissionId);
                var pers = await _repository.FindAllAsync<Permission>(x => permissionIds.Contains(x.Id));
                list.AddRange(pers.Select(x => x.Name));
            }

            list = list.Distinct().OrderBy(x => x).ToList();

            return list;
        }

        private IQueryable<Permission> GetQueryablePermissionsNotInUser(Guid userId)
        {
            var permissions = _repository.GetQueryable<Permission>();
            var userPermissions = _repository.GetQueryable<UserPermission>();

            var permissionIdsInRole = userPermissions
                .Where(userPermission => userPermission.UserId == userId)
                .Join(permissions, userPermission => userPermission.PermissionId, permission => permission.Id, (userPermission, permission) => permission.Id)
                .Distinct();

            return permissions.Where(permission => permissionIdsInRole.All(permisisonInUserId => permisisonInUserId != permission.Id));
        }

        private IQueryable<TRole> GetQueryableRole(Guid userId)
        {
            var roles = _repository.GetQueryable<TRole>();
            var userRoles = _repository.GetQueryable<TUserRole>();

            return userRoles
                .Where(userRole => userRole.UserId == userId)
                .Join(roles, userRole => userRole.RoleId, role => role.Id, (userRole, role) => role);
        }

        private IQueryable<TRole> GetQueryableRolesNotInUser(Guid userId)
        {
            var roles = _repository.GetQueryable<TRole>();
            var userRoles = _repository.GetQueryable<TUserRole>();

            var roleIdsInUser = userRoles
                .Where(userRole => userRole.UserId == userId)
                .Join(roles, userRole => userRole.RoleId, role => role.Id, (userRole, role) => role.Id);

            return roles.Where(role => roleIdsInUser.All(roleInUserId => roleInUserId != role.Id));
        }

        private ServiceResponse UserNotFound() => BadRequest("user_not_found", "User is not existed!");
    }
}