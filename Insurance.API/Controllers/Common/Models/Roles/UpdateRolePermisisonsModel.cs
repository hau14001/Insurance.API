using System;
using System.Collections.Generic;

namespace Insurance.API.Controllers.Common.Models.Roles
{
    public class UpdateRolePermisisonsModel
    {
        public IEnumerable<Guid> PermissionIds { get; set; }
        public IEnumerable<Guid> DeletePermissionIds { get; set; }
    }
}