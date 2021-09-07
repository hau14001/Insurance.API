using Insurance.Domain.Common;

namespace Insurance.Domain.Entities.BaseEntity
{
    public class Permission : Entity
    {
        public string Name { get; set; }

        public string DisplayName { get; set; }
    }
}