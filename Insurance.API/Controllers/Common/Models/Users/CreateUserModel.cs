using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Insurance.API.Controllers.Common.Models.Users
{
    public class CreateUserModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public string PhoneNumber { get; set; }
        public string Code { get; set; }

        public string FullName { get; set; }
        public bool IsLock { get; set; }
        public DateTime? ExpiredDate { get; set; }

        public IEnumerable<Guid> RoleIds { get; set; }
        public IEnumerable<Guid> PermissionIds { get; set; }
    }
}