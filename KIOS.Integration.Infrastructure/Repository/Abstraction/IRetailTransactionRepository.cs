using DriveThru.Integration.Core.Repository;
using DriveThru.Integration.Core.Repository.Abstraction;
using DriveThru.Integration.Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveThru.Integration.Infrastructure.Repository.Abstraction
{
    public interface IRetailTransactionRepository : IBaseRepository<RetailTransaction, long>
    {
        //
    }
}
