using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveThru.Integration.Core.Infrastucture
{
    public interface IApplicationCoreContext
    {
        IDbContext DbContext { get; }
        string AccessToken { get; }
    }
}