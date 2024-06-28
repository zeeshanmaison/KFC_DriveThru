using DriveThru.Integration.DTO.Request;
using DriveThru.Integration.DTO.Response;

namespace DriveThru.Integration.Application.Services.Abstraction
{
    public interface IUserService
    {
        Task<IList<UserResponse>> GetUserList();
        Task<IList<UserResponse>> GetUserByEmail(string email);
        Task<bool> AccountRegisterAsync(IList<AccountRegisterRequest> request);
    }
}









