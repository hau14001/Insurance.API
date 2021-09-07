using Insurance.Domain.Common;

namespace Insurance.Application.Common.Users.Dtos
{
    public class UpdateEmailRequest : EntityDto
    {
        public string Email { get; set; }
    }
}