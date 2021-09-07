using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Insurance.API.Controllers.Common.Models.Roles
{
    public class CreateRoleModel
    {
        [Required]
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public IEnumerable<Guid> Permissions { get; set; }
    }
}