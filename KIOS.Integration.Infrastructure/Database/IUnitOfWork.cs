using DriveThru.Integration.Core.Infrastucture;
using DriveThru.Integration.Core.Repository.Abstraction;
using DriveThru.Integration.Infrastructure.Repository.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveThru.Integration.Infrastructure.Database
{
    public interface IUnitOfWork
    {
        IApplicationCoreContext ApplicationCoreContext { get; }
        IDbContext DbContext { get; }
        Task<int> SaveAsync();
        int Save();
        IBaseRepository<TEntity, TKey> GetRepository<TEntity, TKey>() where TEntity : class;
        TReturn GetModelEntity<T, TReturn>(Guid globalId) where TReturn : class;

        IUserTypeRepository UserTypeRepository { get; }
        IRetailTransactionRepository RetailTransactionRepository { get; }
        IRetailTransactionSalesTransRepository RetailTransactionSalesTransRepository { get; }
    }
}
