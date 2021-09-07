using System;
using System.Collections.Generic;

namespace Insurance.API.Controllers.Common.Models.Users
{
    public class UpdateUserRolesModel
    {
        public IEnumerable<Guid> RoleIds { get; set; }
        public IEnumerable<Guid> DeleteRoleIds { get; set; }
    }
}