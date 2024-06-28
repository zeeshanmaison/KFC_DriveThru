using DriveThru.Integration.Core.Infrastucture;
using DriveThru.Integration.Core.Repository;
using DriveThru.Integration.Infrastructure.Model;
using DriveThru.Integration.Infrastructure.Repository.Abstraction;

namespace DriveThru.Integration.Infrastructure.Repository
{
    public class RetailTransactionSalesTransRepository : BaseRepository<RetailTransactionSalesTrans, long>, IRetailTransactionSalesTransRepository
    {
        private readonly IApplicationCoreContext _applicationCoreContext;

        public RetailTransactionSalesTransRepository(IApplicationCoreContext applicationCoreContext)
           : base(applicationCoreContext)
        {
            _applicationCoreContext = applicationCoreContext;
        }

    }
}
