using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Insurance.Application.Services;
using Insurance.Domain.Common;
using Insurance.Domain.Entities.BaseEntity;
using Insurance.Infrastructure.Data.Repository;

namespace Insurance.Application.Common.Permissions
{
    public class PermissionService : Service, IPermissionService
    {
        private readonly IRepository _repository;

        public PermissionService(IRepository repository)
        {
            _repository = repository;
        }

        public virtual ServiceResponse GetPredefinePermissions()
        {
            var permissions = new List<string>(GemPermissionProvider.GetAll());

            return Ok(permissions);
        }

        public virtual async Task<ServiceResponse> FindAsync(SearchRequest request)
        {
            var query = _repository.GetQueryable<Permission>();
            if (!request.FilterBy.IsNullOrEmpty())
            {
                query = query.Where(r => r.Name.Contains(request.FilterBy));
            }

            return Ok(await _repository.FindPagedAsync(query, request.PageIndex, request.PageSize));
        }
    }
}