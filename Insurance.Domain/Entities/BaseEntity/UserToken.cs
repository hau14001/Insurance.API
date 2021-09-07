using Microsoft.AspNetCore.Identity;
using System;

namespace Insurance.Domain.Entities.BaseEntity
{
    public class UserToken : IdentityUserToken<Guid>
    {
    }
}