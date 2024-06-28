using MediatR;
using DriveThru.Integration.DTO.Response;
using DriveThru.Integration.Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveThru.Integration.Application.Queries
{
    public class GetAllUSerTypeQuery : IRequest<List<UserTypeResponse>>
    {
        public string Name { get; set; }
        public long Id { get; set; }
    }
}
