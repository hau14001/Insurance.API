using System;
using System.Collections.Generic;
using Insurance.Domain.Common;

namespace Insurance.Application.Common.Users.Dtos
{
    public class UpdateRolesForUserRequest : EntityDto
    {
        public IEnumerable<Guid> RoleIds { get; set; }
        public IEnumerable<Guid> DeleteRoleIds { get; set; }
    }
}