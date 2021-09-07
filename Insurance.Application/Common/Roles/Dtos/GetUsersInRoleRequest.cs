using System;
using Insurance.Domain.Common;

namespace Insurance.Application.Common.Roles.Dtos
{
    public class GetUsersInRoleRequest
    {
        public Guid RoleId { get; set; }
        public SearchRequest UserSearch { get; set; }
    }
}