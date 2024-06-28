using Microsoft.EntityFrameworkCore;
using DriveThru.Integration.Core.Infrastucture;
using DriveThru.Integration.Core.Model;
using DriveThru.Integration.Core.Model.Abstraction;
using DriveThru.Integration.Core.Repository.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DriveThru.Integration.Core.Repository
{
    public abstract class BaseRepository<TEntity, TKey> : IBaseRepository<TEntity, TKey> where TEntity : Entity<TKey>, IBase<TKey> where TKey : IEquatable<TKey>
    {
        private readonly IApplicationCoreContext _applicationCoreContext;

        public IDbContext DbContext
        {
            get { return _applicationCoreContext.DbContext; }
        }

        public DbSet<TEntity> DbSet
        {
            get { return _applicationCoreContext.DbContext.Set<TEntity>(); }
        }

        protected virtual IQueryable<TEntity> DefaultQuery
        {
            get
            {
                return _applicationCoreContext.DbContext.Set<TEntity>().AsQueryable();
            }
        }

        protected virtual IQueryable<TEntity> DefaultSingleQuery
        {
            get
            {
                return _applicationCoreContext.DbContext.Set<TEntity>().AsQueryable();
            }
        }

        public BaseRepository(IApplicationCoreContext applicationCoreContext)
        {
            _applicationCoreContext = applicationCoreContext;
        }

        //Audit operation attribute
        public virtual IQueryable<TEntity> GetAll()
        {
            return DefaultQuery;
        }

        //Audit operation attribute
        public virtual async Task<TEntity> GetAsync(Guid globalId)
        {
            return await DefaultSingleQuery.FirstOrDefaultAsync(x => x.GlobalId == globalId);
        }

        //Audit operation attribute
        public virtual async Task<TEntity> GetAsyncById(long id)
        {
            return await DefaultSingleQuery.FirstOrDefaultAsync(x => x.Id.Equals(id));
        }

        ////Audit operation attribute
        //public virtual async Task<TEntity> GetAsync(IList<long> id)
        //{
        //    return await DefaultQuery.Where(x => id.Contains(x.Id.Equals(id))).ToListAsync();
        //}

        //Audit operation attribute
        public virtual TEntity Get(Guid globalId)
        {
            return DefaultSingleQuery.SingleOrDefault(x => x.GlobalId == globalId);
        }

        //Audit operation attribute
       

        //Audit operation attribute
        public virtual async Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            TEntity entity = await DefaultSingleQuery.Where(predicate).SingleOrDefaultAsync();

            if (entity != null)
            {
                return entity;
            }

            return null;
        }

        //Audit operation attribute
        public virtual TEntity Find(Expression<Func<TEntity, bool>> predicate)
        {
            TEntity entity = DefaultSingleQuery.Where(predicate).SingleOrDefault();

            if (entity != null)
            {
                return entity;
            }

            return null;
        }

        //Audit operation attribute
        public async virtual Task<IQueryable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await Task.Run(() => { return DefaultQuery.Where(predicate); });
        }

        //Audit operation attribute
        public virtual IQueryable<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate)
        {
            return DefaultQuery.Where(predicate);
        }

        public virtual async Task<TEntity> CreateAsync(TEntity entity)
        {
            if (entity is IDelete)
            {
                ((IDelete)(entity)).IsDeleted = false;
            }

            await DbSet.AddAsync(entity);

            return entity;
        }

        public virtual TEntity Create(TEntity entity)
        {
            if (entity is IDelete)
            {
                ((IDelete)(entity)).IsDeleted = false;
            }

            DbSet.Add(entity);

            return entity;
        }

        public virtual async Task<IList<TEntity>> CreateRangeAsync(IList<TEntity> range)
        {
            if (range != null && range.Count > 0)
            {
                await DbSet.AddRangeAsync(range.Select(x =>
                {
                    ((IDelete)(x)).IsDeleted = false;

                    return x;
                }).ToList());
            }

            return range;
        }

        //[AuditOperation(OperationType.Create)]
        public virtual IList<TEntity> CreateRange(IList<TEntity> range)
        {
            if (range != null && range.Count > 0)
            {
                DbSet.AddRange(range.Select(x =>
                {
                    ((IDelete)(x)).IsDeleted = false;

                    return x;
                }).ToList());
            }

            return range;
        }

        //[AuditOperation(OperationType.Update)]
        public async virtual Task<TEntity> UpdateAsync(TEntity entity)
        {
            return await Task.Run(() =>
            {
                DbSet.Update(entity);

                return entity;
            });
        }

        //[AuditOperation(OperationType.Update)]
        public virtual TEntity Update(TEntity entity)
        {
            DbSet.Update(entity);

            return entity;
        }

        //[AuditOperation(OperationType.Update)]
        public async virtual Task<IList<TEntity>> UpdateRangeAsync(IList<TEntity> range)
        {
            return await Task.Run(() =>
            {
                DbSet.UpdateRange(range);

                return range;
            });
        }

        //[AuditOperation(OperationType.Update)]
        public virtual IList<TEntity> UpdateRange(IList<TEntity> range)
        {
            DbSet.UpdateRange(range);

            return range;
        }

        //[AuditOperation(OperationType.Delete)]
        public async virtual Task<bool> DeleteAsync(TEntity entity)
        {
            return await Task.Run(() =>
            {
                ((IActivate)(entity)).IsActive = false;
                ((IDelete)(entity)).IsDeleted = true;

                DbSet.Update(entity);

                return ((IDelete)(entity)).IsDeleted;
            });
        }

        //[AuditOperation(OperationType.Delete)]
        public virtual bool Delete(TEntity entity)
        {
            ((IActivate)(entity)).IsActive = false;
            ((IDelete)(entity)).IsDeleted = true;

            DbSet.Update(entity);

            return ((IDelete)(entity)).IsDeleted;
        }

        //[AuditOperation(OperationType.Delete)]
        public async virtual Task<bool> DeleteRangeAsync(IList<TEntity> range)
        {
            return await Task.Run(() =>
            {
                bool isDeleted = false;

                if (range != null && range.Count > 0)
                {
                    DbSet.UpdateRange(range.Select(x =>
                    {
                        ((IActivate)(x)).IsActive = false;
                        ((IDelete)(x)).IsDeleted = true;

                        return x;
                    }).ToList());

                    isDeleted = true;
                }

                return isDeleted;
            });
        }

        //[AuditOperation(OperationType.Delete)]
        public virtual bool DeleteRange(IList<TEntity> range)
        {
            bool isDeleted = false;

            if (range != null && range.Count > 0)
            {
                DbSet.UpdateRange(range.Select(x =>
                {
                    ((IActivate)(x)).IsActive = false;
                    ((IDelete)(x)).IsDeleted = true;

                    return x;
                }).ToList());

                isDeleted = true;
            }

            return isDeleted;
        }

        //[AuditOperation(OperationType.Delete)]
        public async virtual Task<bool> DeletePermanentlyAsync(TEntity entity)
        {
            return await Task.Run(() =>
            {
                bool isDeleted = false;

                DbSet.Remove(entity);

                isDeleted = true;

                return isDeleted;
            });
        }

        public virtual bool DeletePermanently(TEntity entity)
        {
            bool isDeleted = false;

            DbSet.Remove(entity);

            isDeleted = true;

            return isDeleted;
        }

        //[AuditOperation(OperationType.Delete)]
        public async virtual Task<bool> DeletePermanentlyRangeAsync(IList<TEntity> range)
        {
            return await Task.Run(() =>
            {
                bool isDeleted = false;

                if (range != null && range.Count > 0)
                {
                    DbSet.RemoveRange(range.ToArray());

                    isDeleted = true;
                }

                return isDeleted;
            });
        }

        //[AuditOperation(OperationType.Delete)]
        public virtual bool DeletePermanentlyRange(IList<TEntity> range)
        {
            bool isDeleted = false;

            if (range != null && range.Count > 0)
            {
                DbSet.RemoveRange(range.ToArray());

                isDeleted = true;
            }

            return isDeleted;
        }

        public virtual bool Active(TEntity entity)
        {
            ((IActivate)(entity)).IsActive = true;

            DbSet.Update(entity);

            return ((IActivate)(entity)).IsActive;
        }

        public virtual bool ActiveRange(IList<TEntity> range)
        {
            bool isActive = false;

            if (range != null && range.Count > 0)
            {
                DbSet.UpdateRange(range.Select(x =>
                {
                    ((IActivate)(x)).IsActive = true;
                    //((IDelete)(x)).IsDeleted = true;

                    return x;
                }).ToList());

                isActive = true;
            }

            return isActive;
        }

        public async virtual Task<bool> ActiveAsync(TEntity entity)
        {
            return await Task.Run(() =>
            {
                ((IActivate)(entity)).IsActive = true;

                DbSet.Update(entity);

                return ((IActivate)(entity)).IsActive;
            });
        }

        public async virtual Task<bool> ActiveRangeAsync(IList<TEntity> range)
        {
            return await Task.Run(() =>
            {
                bool isActive = false;

                if (range != null && range.Count > 0)
                {
                    DbSet.UpdateRange(range.Select(x =>
                    {
                        ((IActivate)(x)).IsActive = true;
                        ((IDelete)(x)).IsDeleted = false;

                        return x;
                    }).ToList());

                    isActive = true;
                }

                return isActive;
            });

        }

        public virtual bool InActive(TEntity entity)
        {
            ((IActivate)(entity)).IsActive = false;

            DbSet.Update(entity);

            return ((IActivate)(entity)).IsActive;
        }

        public virtual bool InActiveRange(IList<TEntity> range)
        {
            bool isActive = true;

            if (range != null && range.Count > 0)
            {
                DbSet.UpdateRange(range.Select(x =>
                {
                    ((IActivate)(x)).IsActive = false;
                    //((IDelete)(x)).IsDeleted = true;

                    return x;
                }).ToList());

                isActive = false;
            }

            return isActive;
        }

        public async virtual Task<bool> InActiveAsync(TEntity entity)
        {
            return await Task.Run(() =>
            {
                ((IActivate)(entity)).IsActive = false;
                //((IDelete)(entity)).IsDeleted = false;

                DbSet.Update(entity);

                return ((IActivate)(entity)).IsActive;
            });
        }

        public async virtual Task<bool> InActiveRangeAsync(IList<TEntity> range)
        {
            return await Task.Run(() =>
            {
                bool isActive = true;

                if (range != null && range.Count > 0)
                {
                    DbSet.UpdateRange(range.Select(x =>
                    {
                        ((IActivate)(x)).IsActive = false;
                        //((IDelete)(entity)).IsDeleted = false;

                        return x;
                    }).ToList());

                    isActive = false;
                }

                return isActive;
            });

        }
    }
}
