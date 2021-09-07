using System;
using System.Threading.Tasks;
using Insurance.Application.Common.Users.Dtos;
using Insurance.Domain.Common;

namespace Insurance.Application.Common.Users
{
    public interface IUserService
    {
        Task<ServiceResponse> AddAsync(CreateUserRequest request);

        Task<ServiceResponse> UpdateAsync(CreateUserRequest request);

        Task<ServiceResponse> DeleteAsync(Guid id);

        Task<ServiceResponse> ResetPasswordAsync(Guid id);

        Task<ServiceResponse> SetAdmin(Guid id);

        Task<ServiceResponse> ChangePasswordUser(ChangePasswordUserRequest request);

        Task<ServiceResponse> FindAsync(SearchRequest request);

        Task<ServiceResponse> GetAsync(Guid id);

        Task<ServiceResponse> GetPermissionsAsync(Guid userId);

        Task<ServiceResponse> GetFullPermissionsAsync(Guid userId);

        Task<ServiceResponse> GetPermissionsNotInUserAsync(Guid userId);

        Task<ServiceResponse> GetRolesAsync(Guid userId);

        Task<ServiceResponse> GetRolesNotInUserAsync(Guid userId);

        Task<ServiceResponse> UpdateEmailAsync(UpdateEmailRequest request);

        Task<ServiceResponse> UpdatePasswordAsync(UpdatePasswordRequest request);

        Task<ServiceResponse> UpdatePermissionsAsync(UpdatePermissionsForUserRequest request);

        Task<ServiceResponse> UpdateRolesAsync(UpdateRolesForUserRequest request);

        Task<ServiceResponse> UpdateOnePermissionsAsync(Guid idUser, Guid idPermission);
    }
}