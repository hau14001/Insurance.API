using Insurance.Domain.Common;

namespace Insurance.Application.Common.Users.Dtos
{
    public class UpdatePasswordRequest : EntityDto
    {
        public string Password { get; set; }
    }
}