using System.Collections.Generic;
using Insurance.Domain.Entities.BaseEntity;

namespace Insurance.Application.Common.Users.Dtos
{
    public class GetUserResponse<TUser, TRole>
        where TUser : User
        where TRole : Role
    {
        public TUser User { get; set; }
        public IEnumerable<TRole> Roles { get; set; }
        public IEnumerable<Permission> Permissions { get; set; }

        public static GetUserResponse<TUser, TRole> FromEntity(TUser user) => new GetUserResponse<TUser, TRole> { User = user };
    }
}