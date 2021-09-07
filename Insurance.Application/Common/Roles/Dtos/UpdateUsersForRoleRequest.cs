using System;
using System.Collections.Generic;
using Insurance.Domain.Common;

namespace Insurance.Application.Common.Roles.Dtos
{
    public class UpdateUsersForRoleRequest : EntityDto
    {
        public IEnumerable<Guid> UserIds { get; set; }
        public IEnumerable<Guid> DeleteUserIds { get; set; }
    }
}