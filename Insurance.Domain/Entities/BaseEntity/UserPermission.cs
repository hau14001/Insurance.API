using Insurance.Domain.Common;
using System;

namespace Insurance.Domain.Entities.BaseEntity
{
    public class UserPermission : Entity
    {
        public Guid UserId { get; set; }

        public Guid PermissionId { get; set; }
    }
}