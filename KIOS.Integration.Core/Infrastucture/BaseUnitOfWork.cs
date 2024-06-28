using DriveThru.Integration.Core.Repository.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DriveThru.Integration.Core.Infrastucture
{
    public abstract class BaseUnitOfWork
    {
        private readonly IApplicationCoreContext _applicationCoreContext;
        private readonly IDbContext _dbContext;

        public virtual IApplicationCoreContext ApplicationCoreContext { get { return _applicationCoreContext; } }
        public virtual IDbContext DbContext { get { return _dbContext; } }

        public BaseUnitOfWork(IApplicationCoreContext applicationCoreContext)
        {
            _applicationCoreContext = applicationCoreContext;
            _dbContext = _applicationCoreContext.DbContext;
        }

        public virtual async Task<int> SaveAsync()
        {
            return await DbContext.SaveChangesAsync();
        }

        public virtual int Save()
        {
            return DbContext.SaveChanges();
        }

        public IBaseRepository<TEntity, TKey> GetRepository<TEntity, TKey>() where TEntity : class
        {
            string propertyName = typeof(TEntity).Name + "Repository";
            PropertyInfo propertyInfo = GetType().GetProperty(propertyName);
            IBaseRepository<TEntity, TKey> repository = (IBaseRepository<TEntity, TKey>)propertyInfo?.GetValue(this, null);

            return repository;
        }

        //TODO:
        //public TReturn GetModelEntity<T, TReturn>(Guid globalId) where TReturn : class
        //{
        //    //T repositoryInterface = IoC.Resolve<T>();
        //    var repository = null;

        //    TReturn modelEntity = repository.Get(globalId);

        //    return modelEntity;
        //}
    }
}
