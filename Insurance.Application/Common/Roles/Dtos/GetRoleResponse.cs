using System.Collections.Generic;
using Insurance.Domain.Entities.BaseEntity;
using Insurance.Infrastructure.Data;

namespace Insurance.Application.Common.Roles.Dtos
{
    public class GetRoleResponse<TUser, TRole>
        where TUser : User
        where TRole : Role
    {
        public TRole Role { get; set; }
        public PagedResult<TUser> Users { get; set; }
        public IEnumerable<Permission> Permissions { get; set; }

        public static GetRoleResponse<TUser, TRole> FromEntity(TRole role) => new GetRoleResponse<TUser, TRole> { Role = role };
    }
}