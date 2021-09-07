using System;
using System.Collections.Generic;
using Insurance.Domain.Common;

namespace Insurance.Application.Common.Users.Dtos
{
    public class UpdatePermissionsForUserRequest : EntityDto
    {
        public IEnumerable<Guid> PermissionIds { get; set; }
        public IEnumerable<Guid> DeletePermissionIds { get; set; }
    }
}