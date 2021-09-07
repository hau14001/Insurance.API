using System;
using System.Collections.Generic;
using Be.Library;

namespace Insurance.Application.Common.Users.Dtos
{
    public class UserResponse
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Code { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public List<BaseDto> Permissions { get; set; }
    }
}