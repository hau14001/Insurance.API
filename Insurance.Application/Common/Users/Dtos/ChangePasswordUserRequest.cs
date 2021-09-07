using System;

namespace Insurance.Application.Common.Users.Dtos
{
    public class ChangePasswordUserRequest
    {
        public Guid Id { get; set; }

        public string Password { get; set; }
    }
}