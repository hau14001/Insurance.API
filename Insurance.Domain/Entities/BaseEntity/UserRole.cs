using Insurance.Domain.Common;
using Microsoft.AspNetCore.Identity;
using System;

namespace Insurance.Domain.Entities.BaseEntity
{
    public class UserRole : IdentityUserRole<Guid>, IEntity<Guid>
    {
        public Guid Id { get; set; }
    }
}