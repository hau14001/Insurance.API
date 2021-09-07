using Microsoft.AspNetCore.Authorization;

namespace Insurance.Application.Common.Auth
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public PermissionRequirement(string permission) => Permission = permission;

        public string Permission { get; }
    }
}