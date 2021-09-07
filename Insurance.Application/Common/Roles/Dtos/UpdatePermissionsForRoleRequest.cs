using System;
using System.Collections.Generic;
using Insurance.Domain.Common;

namespace Insurance.Application.Common.Roles.Dtos
{
    public class UpdatePermissionsForRoleRequest : EntityDto
    {
        public IEnumerable<Guid> PermissionIds { get; set; }
        public IEnumerable<Guid> DeletePermissionIds { get; set; }
    }
}