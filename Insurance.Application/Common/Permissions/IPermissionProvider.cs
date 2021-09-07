using System.Collections.Generic;

namespace Insurance.Application.Common.Permissions
{
    public interface IPermissionProvider
    {
        IReadOnlyList<string> GetAll();
    }
}