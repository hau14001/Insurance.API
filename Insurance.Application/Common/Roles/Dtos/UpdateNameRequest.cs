using Insurance.Domain.Common;

namespace Insurance.Application.Common.Roles.Dtos
{
    public class UpdateNameRequest : EntityDto
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
    }
}