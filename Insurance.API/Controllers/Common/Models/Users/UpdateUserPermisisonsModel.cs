using System;
using System.Collections.Generic;

namespace Insurance.API.Controllers.Common.Models.Users
{
    public class UpdateUserPermisisonsModel
    {
        public IEnumerable<Guid> PermissionIds { get; set; }
        public IEnumerable<Guid> DeletePermissionIds { get; set; }
    }
}