using MediatR;
using DriveThru.Integration.Infrastructure.Model;

namespace DriveThru.Integration.Application.Commands
{

    public class CreateUserTypeCommand : IRequest<UserType>
    {
        public string Name { get; set; }
    }
}
