using Microsoft.AspNetCore.Identity;
using System;

namespace Insurance.Domain.Entities.BaseEntity
{
    public class RoleClaim : IdentityRoleClaim<Guid>
    {
    }
}