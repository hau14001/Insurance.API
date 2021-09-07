using System.Collections.Generic;

namespace Insurance.Application.Common.Permissions
{
    public static class GemPermissionProvider
    {
        public const string ManageUser = nameof(ManageUser);
        public const string ManageRole = nameof(ManageRole);
        public const string ManagePermission = nameof(ManagePermission);
        public const string ReadUser = nameof(ReadUser);
        public const string CreateUser = nameof(CreateUser);
        public const string UpdateUser = nameof(UpdateUser);
        public const string DeleteUser = nameof(DeleteUser);
        public const string ReadRole = nameof(ReadRole);
        public const string CreateRole = nameof(CreateRole);
        public const string UpdateRole = nameof(UpdateRole);
        public const string DeleteRole = nameof(DeleteRole);

        public static IReadOnlyList<string> GetAll() => new List<string>
        {
            ManageUser,
            ManageRole,
            ReadUser,
            CreateUser,
            UpdateUser,
            DeleteUser,
            ReadRole,
            CreateRole,
            UpdateRole,
            DeleteRole
        };
    }
}