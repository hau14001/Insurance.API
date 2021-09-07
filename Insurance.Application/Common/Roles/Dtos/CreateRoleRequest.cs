using System;
using System.Collections.Generic;

namespace Insurance.Application.Common.Roles.Dtos
{
    public class CreateRoleRequest
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public IEnumerable<Guid> Permissions { get; set; }
    }
}