using Insurance.Domain.Common;
using System;

namespace Insurance.Domain.Entities.BaseEntity
{
    public class RolePermission : Entity
    {
        public Guid RoleId { get; set; }

        public Guid PermissionId { get; set; }
    }
}