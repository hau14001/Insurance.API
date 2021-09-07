using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Insurance.Domain.Entities.BaseEntity;
using Insurance.Infrastructure.Data;
using Insurance.Infrastructure.Data.Repository;
using Microsoft.Extensions.Caching.Distributed;

namespace Insurance.Application.Common.Permissions
{
    public class PermissionChecker<TUser, TUserRole> : IPermissionChecker
        where TUser : User
        where TUserRole : UserRole, new()
    {
        private const int DefaultCacheExpirationTimeInMinutes = 10;
        private readonly IRepository _repository;
        private readonly IDistributedCache _cache;
        public List<string> Permissions { get; set; }
        private readonly ICurrentUser _currentUser;

        public PermissionChecker(IRepository repository, IDistributedCache cache, ICurrentUser currentUser)
        {
            _currentUser = currentUser;
            _repository = repository;
            _cache = cache;
        }

        public async Task<bool> HasPermission(string permission)
        {
            var user = await _repository.FindAsync<TUser>(_currentUser.GetId());
            if (user == null)
            {
                return false;
            }

            if (user.IsSystemUser)
            {
                return true;
            }

            if (Permissions == null || Permissions.Count < 1)
            {
                await GetPermissions();
            }

            return Permissions != null && Permissions.Any() && Permissions.Contains(permission);
        }

        public async Task GetPermissions()
        {
            var user = await _repository.FindAsync<TUser>(_currentUser.GetId());

            var roles = await _repository.FindAllAsync<TUserRole>(ur => ur.UserId == user.Id, selector: ur => new TUserRole { RoleId = ur.RoleId });
            var roleIds = roles.Select(r => r.RoleId);
            var rolePermissions = _repository.GetQueryable<RolePermission>().Where(rp => roleIds.Contains(rp.RoleId)).Select(rp => rp.PermissionId);
            var userPermissions = _repository.GetQueryable<UserPermission>().Where(up => up.UserId == user.Id).Select(rp => rp.PermissionId);
            var permissionIds = rolePermissions.Union(userPermissions);
            var permissions = await _repository.FindAllAsync<Permission>(p => permissionIds.Contains(p.Id));

            Permissions = permissions.Select(x => x.Name).ToList();
        }

        public virtual async Task<bool> IsGrantedAsync(Guid userId, string permission, string ip)
        {
            var cachedValue = await _cache.GetStringAsync(userId + permission);
            if (!cachedValue.IsNullOrEmpty())
            {
                bool.TryParse(cachedValue, out bool isCachedValueGranted);
                return isCachedValueGranted;
            }

            var user = await _repository.FindAsync<TUser>(userId);
            if (user == null)
            {
                return false;
            }

            var currentIp = user.Ip;

            if (currentIp == null || currentIp != ip)
            {
                return false;
            }

            if (user.IsSystemUser)
            {
                return true;
            }

            var roles = await _repository.FindAllAsync<TUserRole>(ur => ur.UserId == user.Id, selector: ur => new TUserRole { RoleId = ur.RoleId });
            var roleIds = roles.Select(r => r.RoleId);
            var rolePermissions = _repository.GetQueryable<RolePermission>().Where(rp => roleIds.Contains(rp.RoleId)).Select(rp => rp.PermissionId);
            var userPermissions = _repository.GetQueryable<UserPermission>().Where(up => up.UserId == user.Id).Select(rp => rp.PermissionId);
            var permissionIds = rolePermissions.Union(userPermissions);
            var permissions = await _repository.FindAllAsync<Permission>(p => permissionIds.Contains(p.Id));

            var isGranted = permissions.Any(rp => rp.Name.Equals(permission, StringComparison.OrdinalIgnoreCase) || rp.Name.Equals("Admin", StringComparison.OrdinalIgnoreCase));

            await _cache.SetStringAsync(userId.ToString(), isGranted.ToString(), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(DefaultCacheExpirationTimeInMinutes)
            });

            return isGranted;
        }
    }
}