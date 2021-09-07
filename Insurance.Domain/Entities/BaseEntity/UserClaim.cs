using Microsoft.AspNetCore.Identity;
using System;

namespace Insurance.Domain.Entities.BaseEntity
{
    public class UserClaim : IdentityUserClaim<Guid>
    {
    }
}