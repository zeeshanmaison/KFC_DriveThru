using DriveThru.Integration.Core.Infrastucture;
using DriveThru.Integration.Infrastructure.Repository.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveThru.Integration.Infrastructure.Database
{
    public class UnitOfWork : BaseUnitOfWork, IUnitOfWork
    {
        private readonly IUserTypeRepository _userTypeRepository;
        private readonly IRetailTransactionRepository _retailTransactionRepository;
        private readonly IRetailTransactionSalesTransRepository _retailTransactionSalesTransRepository;

        
         public IUserTypeRepository UserTypeRepository { get { return _userTypeRepository; } }
         public IRetailTransactionRepository RetailTransactionRepository { get { return _retailTransactionRepository; } }
         public IRetailTransactionSalesTransRepository RetailTransactionSalesTransRepository { get { return _retailTransactionSalesTransRepository; } }


        public UnitOfWork(
            IApplicationCoreContext applicationCoreContext,
            IUserTypeRepository userTypeRepository
            ) :base(applicationCoreContext)
        {
            _userTypeRepository = userTypeRepository;
            _retailTransactionRepository = RetailTransactionRepository;
            _retailTransactionSalesTransRepository = RetailTransactionSalesTransRepository;
        }

        public TReturn GetModelEntity<T, TReturn>(Guid globalId) where TReturn : class
        {
            throw new NotImplementedException();
        }
    }
}
