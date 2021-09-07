using System.Threading.Tasks;
using Insurance.Domain.Common;

namespace Insurance.Application.Common.Permissions
{
    public interface IPermissionService
    {
        Task<ServiceResponse> FindAsync(SearchRequest request);

        ServiceResponse GetPredefinePermissions();
    }
}