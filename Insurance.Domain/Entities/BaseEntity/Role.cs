using Insurance.Domain.Common;
using Microsoft.AspNetCore.Identity;
using System;

namespace Insurance.Domain.Entities.BaseEntity
{
    public class Role : IdentityRole<Guid>, IAuditedEntity, IEntity<Guid>
    {
        public bool IsSystemRole { get; set; }
        public string DisplayName { get; set; }
        public DateTime CreatedAt { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime UpdatedAt { get; set; }

        public Guid UpdatedBy { get; set; }
    }
}