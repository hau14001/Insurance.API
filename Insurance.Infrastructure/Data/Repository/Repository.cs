using Insurance.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Insurance.Infrastructure.Data.Repository
{
    public class Repository<TContext> : IRepository
     where TContext : DbContext
    {
        protected readonly ICurrentUser CurrentUser;
        protected readonly TContext DbContext;
        protected readonly ILogger<IRepository> Logger;

        public Repository(ICurrentUser currentUser, TContext dbContext, ILogger<IRepository> logger)
        {
            CurrentUser = currentUser;
            DbContext = dbContext;
            Logger = logger;
        }

        public virtual Task AddAsync<TEntity>(TEntity entity)
            where TEntity : class, IEntity<Guid>
        {
            return AddAsync<TEntity, Guid>(entity);
        }

        public virtual Task AddAsync<TEntity, TKey>(TEntity entity)
            where TEntity : class, IEntity<TKey>
            where TKey : IEquatable<TKey>
        {
            return DbContext.AddAsync(entity).AsTask();
        }

        public virtual Task AddRangeAsync<TEntity>(IEnumerable<TEntity> entities)
            where TEntity : class, IEntity<Guid>
        {
            return AddRangeAsync<TEntity, Guid>(entities);
        }

        public virtual Task AddRangeAsync<TEntity, TKey>(IEnumerable<TEntity> entities)
            where TEntity : class, IEntity<TKey>
            where TKey : IEquatable<TKey>
        {
            return DbContext.AddRangeAsync(entities);
        }

        public virtual Task DeleteAsync<TEntity>(TEntity entity)
            where TEntity : class, IEntity<Guid>
        {
            return DeleteAsync<TEntity, Guid>(entity);
        }

        public virtual Task DeleteAsync<TEntity, TKey>(TEntity entity)
            where TEntity : class, IEntity<TKey>
            where TKey : IEquatable<TKey>
        {
            if (entity is ISoftDelete deleteEntity)
            {
                deleteEntity.IsDeleted = true;
                DbContext.Update(entity);

                return Task.CompletedTask;
            }

            DbContext.Remove(entity);

            return Task.CompletedTask;
        }

        public virtual Task DeleteAsync<TEntity>(params object[] ids)
            where TEntity : class, IEntity<Guid>
        {
            return DeleteAsync<TEntity, Guid>(ids);
        }

        public virtual async Task DeleteAsync<TEntity, TKey>(params object[] ids)
            where TEntity : class, IEntity<TKey>
            where TKey : IEquatable<TKey>
        {
            var entity = await DbContext.FindAsync<TEntity>(ids);

            await DeleteAsync<TEntity, TKey>(entity);
        }

        public virtual Task DeleteRangeAsync<TEntity>(IEnumerable<TEntity> entities)
            where TEntity : class, IEntity<Guid>
        {
            if (entities != null)
            {
                return DeleteRangeAsync<TEntity, Guid>(entities);
            }

            return Task.CompletedTask;
        }

        public virtual Task DeleteRangeAsync<TEntity, TKey>(IEnumerable<TEntity> entities)
            where TEntity : class, IEntity<TKey>
            where TKey : IEquatable<TKey>
        {
            return DeleteRangeAsync<TEntity, TKey>(entities.Select(e => (object)e.Id));
        }

        public virtual Task DeleteRangeAsync<TEntity>(IEnumerable<object> ids)
            where TEntity : class, IEntity<Guid>
        {
            return DeleteRangeAsync<TEntity, Guid>(ids);
        }

        public virtual async Task DeleteRangeAsync<TEntity, TKey>(IEnumerable<object> ids)
            where TEntity : class, IEntity<TKey>
            where TKey : IEquatable<TKey>
        {
            if (ids != null)
            {
                foreach (var id in ids)
                {
                    await DeleteAsync<TEntity, TKey>(new object[] { id });
                }
            }
        }

        public virtual Task<IEnumerable<TEntity>> FindAllAsync<TEntity>(Expression<Func<TEntity, bool>> where = null, Expression<Func<TEntity, TEntity>> selector = null, IEnumerable<string> includes = null)
            where TEntity : class, IEntity<Guid>
        {
            return FindAllAsync<TEntity, Guid>(where, selector, includes);
        }

        public virtual Task<IEnumerable<TEntity>> FindAllAsync<TEntity, TKey>(Expression<Func<TEntity, bool>> where = null, Expression<Func<TEntity, TEntity>> selector = null, IEnumerable<string> includes = null)
            where TEntity : class, IEntity<TKey>
            where TKey : IEquatable<TKey>
        {
            var query = BuildIncludeQuery<TEntity, TKey>(includes);
            query = ExcludeSoftDeletedEntity<TEntity, TKey>(query);

            if (where != null)
            {
                query = query.Where(where);
            }

            return FindAllAsync<TEntity, TKey>(query, selector);
        }

        public virtual Task<IEnumerable<TEntity>> FindAllAsync<TEntity>(
            IQueryable<TEntity> query,
            Expression<Func<TEntity, TEntity>> selector = null)
            where TEntity : class, IEntity<Guid>
        {
            return FindAllAsync<TEntity, Guid>(query, selector);
        }

        public virtual async Task<IEnumerable<TEntity>> FindAllAsync<TEntity, TKey>(
            IQueryable<TEntity> query,
            Expression<Func<TEntity, TEntity>> selector = null)
            where TEntity : class, IEntity<TKey>
            where TKey : IEquatable<TKey>
        {
            query = ExcludeSoftDeletedEntity<TEntity, TKey>(query);
            query = ApplyDefaultOrderBy<TEntity, TKey>(query);
            query = query.AsNoTracking();

            return selector == null ? await query.ToListAsync() : await query.Select(selector).ToListAsync();
        }

        public virtual Task<TEntity> FindAsync<TEntity>(params object[] ids)
            where TEntity : class, IEntity<Guid>
        {
            return FindAsync<TEntity, Guid>(ids);
        }

        public virtual Task<TEntity> FindAsync<TEntity, TKey>(params object[] ids)
            where TEntity : class, IEntity<TKey>
            where TKey : IEquatable<TKey>
        {
            return DbContext.FindAsync<TEntity>(ids).AsTask();
        }

        public virtual Task<TEntity> FindAsync<TEntity>(Guid id, IEnumerable<string> includes = null)
            where TEntity : class, IEntity<Guid>
        {
            return FindAsync<TEntity, Guid>(id, includes);
        }

        public virtual async Task<TEntity> FindAsync<TEntity, TKey>(TKey id, IEnumerable<string> includes = null)
            where TEntity : class, IEntity<TKey>
            where TKey : IEquatable<TKey>
        {
            var query = BuildIncludeQuery<TEntity, TKey>(includes);
            query = ExcludeSoftDeletedEntity<TEntity, TKey>(query);

            return await query.FirstOrDefaultAsync(e => e.Id.Equals(id));
        }

        public virtual Task<TEntity> FindAsync<TEntity>(Expression<Func<TEntity, bool>> where = null, IEnumerable<string> includes = null)
            where TEntity : class, IEntity<Guid>
        {
            return FindAsync<TEntity, Guid>(where, includes);
        }

        public virtual async Task<TEntity> FindAsync<TEntity, TKey>(Expression<Func<TEntity, bool>> where = null, IEnumerable<string> includes = null)
            where TEntity : class, IEntity<TKey>
            where TKey : IEquatable<TKey>
        {
            var query = BuildIncludeQuery<TEntity, TKey>(includes);
            query = ExcludeSoftDeletedEntity<TEntity, TKey>(query);

            return await query.FirstOrDefaultAsync(where);
        }

        public virtual async Task<bool> CheckContains<TEntity, TKey>(Expression<Func<TEntity, bool>> where = null, IEnumerable<string> includes = null)
            where TEntity : class, IEntity<TKey>
            where TKey : IEquatable<TKey>
        {
            var query = BuildIncludeQuery<TEntity, TKey>(includes);
            query = ExcludeSoftDeletedEntity<TEntity, TKey>(query);

            return await query.AnyAsync(where);
        }

        public virtual Task<bool> CheckContains<TEntity>(Expression<Func<TEntity, bool>> where = null)
            where TEntity : class, IEntity<Guid>
        {
            var query = BuildIncludeQuery<TEntity, Guid>(null);
            query = ExcludeSoftDeletedEntity<TEntity, Guid>(query);
            return query.AnyAsync(where);
        }

        public virtual Task<PagedResult<TEntity>> FindPagedAsync<TEntity>(
            Expression<Func<TEntity, bool>> where = null,
            int pageIndex = 1,
            int pageSize = 10,
            Expression<Func<TEntity, TEntity>> selector = null,
            IEnumerable<string> includes = null)
            where TEntity : class, IEntity<Guid>
        {
            return FindPagedAsync<TEntity, Guid>(where, pageIndex, pageSize, selector, includes);
        }

        public virtual Task<PagedResult<TEntity>> FindPagedAsync<TEntity, TKey>(
            Expression<Func<TEntity, bool>> where = null,
            int pageIndex = 1,
            int pageSize = 10,
            Expression<Func<TEntity, TEntity>> selector = null,
            IEnumerable<string> includes = null)
            where TEntity : class, IEntity<TKey>
            where TKey : IEquatable<TKey>
        {
            var query = BuildIncludeQuery<TEntity, TKey>(includes);

            if (where != null)
            {
                query = query.Where(where);
            }

            query = ApplyDefaultOrderBy<TEntity, TKey>(query);

            return FindPagedAsync<TEntity, TKey>(query, pageIndex, pageSize, selector);
        }

        public virtual Task<PagedResult<TEntity>> FindPagedAsync<TEntity>(
            IQueryable<TEntity> query,
            int pageIndex = 1,
            int pageSize = 10,
            Expression<Func<TEntity, TEntity>> selector = null)
            where TEntity : class, IEntity<Guid>
        {
            return FindPagedAsync<TEntity, Guid>(query, pageIndex, pageSize, selector);
        }

        public virtual async Task<PagedResult<TEntity>> FindPagedAsync<TEntity, TKey>(
            IQueryable<TEntity> query,
            int pageIndex = 1,
            int pageSize = 10,
            Expression<Func<TEntity, TEntity>> selector = null)
            where TEntity : class, IEntity<TKey>
            where TKey : IEquatable<TKey>
        {
            pageIndex = pageIndex <= 0 ? 1 : pageIndex;
            pageSize = pageSize <= 0 ? 10 : pageSize;

            pageIndex -= 1;
            var skip = pageIndex * pageSize;

            query = ExcludeSoftDeletedEntity<TEntity, TKey>(query);
            query = query.AsNoTracking();
            var totalCount = await query.CountAsync();
            query = skip == 0 ? query.Take(pageSize) : query.Skip(skip).Take(pageSize);

            return new PagedResult<TEntity>
            {
                PageIndex = pageIndex + 1,
                PageSize = pageSize,
                Items = selector == null ? await query.ToListAsync() : await query.Select(selector).ToListAsync(),
                TotalCount = totalCount
            };
        }

        public virtual IQueryable<TEntity> GetQueryable<TEntity>()
            where TEntity : class, IEntity<Guid>
        {
            return GetQueryable<TEntity, Guid>();
        }

        public virtual IQueryable<TEntity> GetQueryable<TEntity, TKey>()
            where TEntity : class, IEntity<TKey>
            where TKey : IEquatable<TKey>
        {
            return DbContext.Set<TEntity>();
        }

        public virtual Task SaveChangeAsync()
        {
            var currentDetectChangesSetting = DbContext.ChangeTracker.AutoDetectChangesEnabled;

            try
            {
                DbContext.ChangeTracker.AutoDetectChangesEnabled = false;
                DbContext.ChangeTracker.DetectChanges();

                var modifiedEntries = DbContext.ChangeTracker.Entries<IAuditedEntity>()
                    .Where(x => x.State == EntityState.Added || x.State == EntityState.Modified || x.State == EntityState.Deleted);

                AuditEntity(modifiedEntries);

                DbContext.ChangeTracker.DetectChanges();

                return DbContext.SaveChangesAsync();
            }
            finally
            {
                DbContext.ChangeTracker.AutoDetectChangesEnabled = currentDetectChangesSetting;
            }
        }

        public virtual Task UpdateAsync<TEntity>(TEntity entity)
            where TEntity : class, IEntity<Guid>
        {
            return UpdateAsync<TEntity, Guid>(entity);
        }

        public virtual Task UpdateAsync<TEntity, TKey>(TEntity entity)
            where TEntity : class, IEntity<TKey>
            where TKey : IEquatable<TKey>
        {
            DbContext.Update(entity);

            return Task.CompletedTask;
        }

        private static IQueryable<TEntity> ApplyDefaultOrderBy<TEntity, TKey>(IQueryable<TEntity> query)
            where TEntity : class, IEntity<TKey>
            where TKey : IEquatable<TKey>
        {
            if (typeof(TEntity).GetInterfaces().Contains(typeof(IAuditedEntity)))
            {
                query = query.OrderByDescending(e => ((IAuditedEntity)e).CreatedAt);
            }
            else
            {
                query = query.OrderBy(e => e.Id);
            }

            return query;
        }

        private static IQueryable<TEntity> ExcludeSoftDeletedEntity<TEntity, TKey>(IQueryable<TEntity> query)
            where TEntity : class, IEntity<TKey>
            where TKey : IEquatable<TKey>
        {
            if (typeof(TEntity).GetInterfaces().Contains(typeof(ISoftDelete)))
            {
                query = query.Where(e => !((ISoftDelete)e).IsDeleted);
            }

            return query;
        }

        private void AuditEntity(IEnumerable<EntityEntry<IAuditedEntity>> modifiedEntries)
        {
            var now = DateTime.UtcNow;
            var userId = CurrentUser.GetId();

            foreach (var entry in modifiedEntries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = now;
                        entry.Entity.CreatedBy = userId;
                        AuditUpdatingField(entry);

                        Logger.LogInformation("Added entity {@Entity} by user {UserId}", entry.Entity, CurrentUser.GetId());
                        break;

                    case EntityState.Modified:
                        AuditUpdatingField(entry);

                        Logger.LogInformation("Updated entity to {@Entity} by user {UserId}", entry.Entity, CurrentUser.GetId());
                        break;

                    case EntityState.Deleted:
                        Logger.LogInformation("Deleted entity {@Entity} by user {UserId}", entry.Entity, CurrentUser.GetId());
                        break;
                }
            }

            void AuditUpdatingField(EntityEntry<IAuditedEntity> entry)
            {
                entry.Entity.UpdatedAt = now;
                entry.Entity.UpdatedBy = userId;
            }
        }

        private IQueryable<TEntity> BuildIncludeQuery<TEntity, TKey>(IEnumerable<string> includes)
            where TEntity : class, IEntity<TKey>
            where TKey : IEquatable<TKey>
        {
            var query = DbContext.Set<TEntity>().AsQueryable();
            if (includes?.Any() != true)
            {
                return query;
            }

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return query;
        }
    }
}