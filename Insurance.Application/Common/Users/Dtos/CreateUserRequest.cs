using System;
using System.Collections.Generic;

namespace Insurance.Application.Common.Users.Dtos
{
    public class CreateUserRequest
    {
        public string Code { get; set; }
        public string FullName { get; set; }
        public bool IsLock { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public IEnumerable<Guid> RoleIds { get; set; }
        public IEnumerable<Guid> PermissionIds { get; set; }
    }
}