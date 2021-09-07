using Microsoft.AspNetCore.Authorization;

namespace Be.Infrastructure.ConfigureAuth
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public PermissionRequirement(string permission) => Permission = permission;

        public string Permission { get; }
    }
}