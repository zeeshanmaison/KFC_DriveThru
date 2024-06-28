using DriveThru.Integration.Core.Infrastucture;
using DriveThru.Integration.Core.Repository;
using DriveThru.Integration.Infrastructure.Model;
using DriveThru.Integration.Infrastructure.Repository.Abstraction;

namespace DriveThru.Integration.Infrastructure.Repository
{
    public class RetailTransactionRepository : BaseRepository<RetailTransaction, long>, IRetailTransactionRepository
    {
        private readonly IApplicationCoreContext _applicationCoreContext;

        public RetailTransactionRepository(IApplicationCoreContext applicationCoreContext)
           : base(applicationCoreContext)
        {
            _applicationCoreContext = applicationCoreContext;
        }

    }
}
