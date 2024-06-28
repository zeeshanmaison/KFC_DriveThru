using MediatR;
using Microsoft.EntityFrameworkCore;
using KIOS.Integration.Application.Queries;
using KIOS.Integration.Infrastructure.Database;
using KIOS.Integration.Infrastructure.Model;
using KIOS.Integration.Infrastructure.Repository.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KIOS.Integration.Application.Handlers
{
    public class GetAllUserTypeQueryHandler : IRequestHandler<GetAllUSerTypeQuery, List<UserType>>
    {
        /// <summary>
        //private readonly IUserTypeRepository _userTypeRepository;
        /// </summary>
        /// <param name="productsRepo"></param>
        /// 
        private readonly AppDbContext _appDbContext;

        public GetAllUserTypeQueryHandler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext; 
        }

        public async Task<List<UserType>> Handle(GetAllUSerTypeQuery request, CancellationToken cancellationToken)
        {
            List<UserType> UserTypes = await _appDbContext.UserTypes.AsQueryable<UserType>().ToListAsync();

            return UserTypes;
        }
    }
}
