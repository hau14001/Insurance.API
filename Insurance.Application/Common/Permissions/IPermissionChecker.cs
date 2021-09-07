using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Insurance.Application.Common.Permissions
{
    public interface IPermissionChecker
    {
        Task<bool> HasPermission(string permission);

        Task<bool> IsGrantedAsync(Guid userId, string permission, string ip);

        List<string> Permissions { get; set; }
    }
}