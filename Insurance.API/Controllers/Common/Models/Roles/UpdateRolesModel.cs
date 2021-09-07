using System;
using System.Collections.Generic;

namespace Insurance.API.Controllers.Common.Models.Roles
{
    public class UpdateUsersModel
    {
        public IEnumerable<Guid> UserIds { get; set; }
        public IEnumerable<Guid> DeleteUserIds { get; set; }
    }
}