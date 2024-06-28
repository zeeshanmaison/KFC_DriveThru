using MediatR;
using Microsoft.EntityFrameworkCore;
using DriveThru.Integration.Application.Queries;
using DriveThru.Integration.DTO.Converters;
using DriveThru.Integration.DTO.Response;
using DriveThru.Integration.Infrastructure.Database;
using DriveThru.Integration.Infrastructure.Model;
using System.Net;

namespace DriveThru.Integration.Application.Handlers.QueryHandler
{
    public class GetAllUserTypeQueryHandler : IRequestHandler<GetAllUSerTypeQuery, List<UserTypeResponse>>
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

        public async Task<List<UserTypeResponse>> Handle(GetAllUSerTypeQuery request, CancellationToken cancellationToken)
        {
            HttpStatusCode httpStatusCode = HttpStatusCode.Accepted;
            List<UserTypeResponse> userTypeResponses = null;
            List<UserType> UserTypes = await _appDbContext.UserTypes.AsQueryable<UserType>().ToListAsync();

            if (UserTypes != null && UserTypes.Any())
            {
                userTypeResponses = UserTypes.Select(x => x.ConvertToResponse()).ToList();
                httpStatusCode = HttpStatusCode.OK;
            }
            else
            {
                httpStatusCode = HttpStatusCode.NoContent;
            }

            return userTypeResponses;
        }
    }
}
