using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DriveThru.Integration.Core.Repository.Abstraction
{
    public interface IBaseRepository<TEntity, in TKey> where TEntity : class
    {
        IQueryable<TEntity> GetAll();
        Task<TEntity> GetAsync(Guid globalId);
        Task<TEntity> GetAsyncById(long id);
        TEntity Get(Guid globalId);
        Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate);
        TEntity Find(Expression<Func<TEntity, bool>> predicate);
        Task<IQueryable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate);
        IQueryable<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> CreateAsync(TEntity entity);
        TEntity Create(TEntity entity);
        Task<IList<TEntity>> CreateRangeAsync(IList<TEntity> range);
        IList<TEntity> CreateRange(IList<TEntity> range);
        Task<TEntity> UpdateAsync(TEntity entity);
        TEntity Update(TEntity entity);
        Task<IList<TEntity>> UpdateRangeAsync(IList<TEntity> range);
        IList<TEntity> UpdateRange(IList<TEntity> range);
        Task<bool> DeleteAsync(TEntity entity);
        bool Delete(TEntity entity);
        Task<bool> DeleteRangeAsync(IList<TEntity> range);
        bool DeleteRange(IList<TEntity> range);
        Task<bool> DeletePermanentlyAsync(TEntity entity);
        bool DeletePermanently(TEntity entity);
        Task<bool> DeletePermanentlyRangeAsync(IList<TEntity> range);
        bool DeletePermanentlyRange(IList<TEntity> range);
        bool Active(TEntity entity);
        bool ActiveRange(IList<TEntity> range);
        Task<bool> ActiveAsync(TEntity entity);
        Task<bool> ActiveRangeAsync(IList<TEntity> range);
        bool InActive(TEntity entity);
        bool InActiveRange(IList<TEntity> range);
        Task<bool> InActiveAsync(TEntity entity);
        Task<bool> InActiveRangeAsync(IList<TEntity> range);
    }
}
