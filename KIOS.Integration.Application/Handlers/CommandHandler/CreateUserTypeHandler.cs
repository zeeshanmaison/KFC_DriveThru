using MediatR;
using DriveThru.Integration.Application.Commands;
using DriveThru.Integration.Infrastructure.Database;
using DriveThru.Integration.Infrastructure.Model;

namespace DriveThru.Integration.Application.Handlers.CommandHandler
{
    public class CreateUserTypeHandler : IRequestHandler<CreateUserTypeCommand, UserType>
    {
        private readonly AppDbContext _appDbContext;

        public CreateUserTypeHandler (AppDbContext appDbContext) 
        {
            _appDbContext = appDbContext;
        }

        public async Task<UserType> Handle(CreateUserTypeCommand request, CancellationToken cancellationToken)
        {
            UserType userType = new UserType 
            { 
                Name = request.Name, 
                CreatedOn = DateTime.Now,
                IsActive = true, 
                IsDeleted = false 
            };
            await _appDbContext.UserTypes.AddAsync(userType);
            await _appDbContext.SaveChangesAsync();
            return userType;
        }
    }
}
