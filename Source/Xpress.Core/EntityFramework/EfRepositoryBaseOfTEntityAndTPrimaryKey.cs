using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xpress.Core.Domain.Entities;
using Xpress.Core.Repositories;

namespace Xpress.Core.EntityFramework
{
    /// <inheritdoc />
    public class EFRepositoryBase<TEntity, TIdentity> : AbsRepositoryBase<TEntity, TIdentity>
        where TEntity : class, IEntity<TIdentity>
    {
        private readonly IEfDbContextProvider _dbContextProvider;

        /// <inheritdoc />
        public EFRepositoryBase(IEfDbContextProvider dbContextProvider)
        {
            _dbContextProvider = dbContextProvider;
        }

        /// <summary>
        /// Gets EF DbContext object.
        /// </summary>
        public virtual DbContext DbContext => _dbContextProvider.GetDbContext();

        /// <summary>
        /// Gets DbSet for given entity.
        /// </summary>
        public virtual DbSet<TEntity> Table => DbContext.Set<TEntity>();

        #region Select

        /// <inheritdoc />
        public override async Task<TEntity> GetAsync(TIdentity id)
        {
            var entity = await Table.FindAsync(id);
            if (entity == null)
            {
                throw new Exception($"There is no such an entity. Entity type: {typeof(TEntity).FullName}, id: {id}");
            }
            return entity;
        }

        /// <inheritdoc />
        public override async Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await Table.SingleAsync(predicate);
        }

        /// <inheritdoc />
        public override async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await Table.FirstOrDefaultAsync(predicate);
        }

        /// <inheritdoc />
        public override async Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate = null)
        {
            var query = Table.AsQueryable();
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            return await query.ToListAsync();
        }

        #endregion

        #region Insert

        /// <inheritdoc />
        public override async Task<TEntity> InsertEntityAsync(TEntity entity)
        {
            var entityEntry = await Table.AddAsync(entity);
            await DbContext.SaveChangesAsync();
            return entityEntry.Entity;
        }

        #endregion

        #region Update

        /// <inheritdoc />
        public override async Task<TEntity> UpdateEntityAsync(TEntity entity)
        {
            AttachIfNot(entity);
            var entityEntry = DbContext.Entry(entity);
            if (!DbContext.ChangeTracker.AutoDetectChangesEnabled)
            {
                entityEntry.State = EntityState.Modified;
            }
            await DbContext.SaveChangesAsync();
            return entity;
        }

        #endregion

        #region Delete

        /// <inheritdoc />
        public override async Task DeleteEntityAsync(TEntity entity)
        {
            AttachIfNot(entity);
            Table.Remove(entity);
            await DbContext.SaveChangesAsync();
        }

        #endregion

        #region Aggregates

        /// <inheritdoc />
        public override async Task<long> CountAsync(Expression<Func<TEntity, bool>> predicate = null)
        {
            var query = Table.AsQueryable();
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            return await query.LongCountAsync();
        }

        #endregion

        /// <inheritdoc />
        protected virtual void AttachIfNot(TEntity entity)
        {
            var entry = DbContext.ChangeTracker.Entries().FirstOrDefault(ent => ent.Entity == entity);
            if (entry != null)
            {
                return;
            }

            Table.Attach(entity);
        }
    }
}

