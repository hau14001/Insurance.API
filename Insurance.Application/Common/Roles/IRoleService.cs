using System;
using System.Threading.Tasks;
using Insurance.Application.Common.Roles.Dtos;
using Insurance.Domain.Common;

namespace Insurance.Application.Common.Roles
{
    public interface IRoleService
    {
        Task<ServiceResponse> AddAsync(CreateRoleRequest request);

        Task<ServiceResponse> DeleteAsync(Guid id);

        Task<ServiceResponse> DeleteAsync(string name);

        Task<ServiceResponse> FindAsync(SearchRequest request);

        Task<ServiceResponse> GetAll();

        Task<ServiceResponse> GetAsync(GetUsersInRoleRequest request);

        Task<ServiceResponse> GetPermissionsAsync(Guid roleId);

        Task<ServiceResponse> GetPermissionsNotInRoleAsync(Guid roleId);

        Task<ServiceResponse> GetUsersAsync(GetUsersInRoleRequest request);

        Task<ServiceResponse> GetUsersNotInRoleAsync(GetUsersInRoleRequest request);

        Task<ServiceResponse> UpdateNameAsync(UpdateNameRequest request);

        Task<ServiceResponse> UpdatePermissionsAsync(UpdatePermissionsForRoleRequest request);

        Task<ServiceResponse> UpdateUsersAsync(UpdateUsersForRoleRequest request);
    }
}