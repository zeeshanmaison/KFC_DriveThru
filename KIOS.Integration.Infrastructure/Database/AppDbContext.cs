using Microsoft.EntityFrameworkCore;
using DriveThru.Integration.Core.Infrastucture;
using DriveThru.Integration.Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveThru.Integration.Infrastructure.Database
{
    public class AppDbContext : DbContext, IDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
                : base(options)
        {
            //
        }

        public DbSet<UserType> UserTypes { get; set; }
        public DbSet<RetailTransaction> RetailTransactions { get; set; }
        public DbSet<RetailTransactionSalesTrans> RetailTransactionSalesTrans { get; set; }
    }
}
