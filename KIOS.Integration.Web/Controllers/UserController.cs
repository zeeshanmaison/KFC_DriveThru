using Microsoft.AspNetCore.Mvc;
using DriveThru.Integration.Application.Services.Abstraction;
using DriveThru.Integration.DTO.Request;
using DriveThru.Integration.DTO.Response;
using System.Threading.Tasks;

namespace DriveThru.Integration.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;

        public UserController(ILogger<UserController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        /*

        [HttpGet]
        [Route("get-user-list")]
        public async Task<IList<UserResponse>> GetUserResponsesAsync()
        {
            return await _userService.GetUserList();
        }

        [HttpGet]
        [Route("user-by-email")]
        public async Task<IList<UserResponse>> GetUserResponsesByEmailAsync(string email)
        {
            return await _userService.GetUserByEmail(email);
        }

        [HttpPost]
        [Route("account-register")]
        public async Task<bool> AccountRegisterAsync(IList<AccountRegisterRequest> request)
        {
            return await _userService.AccountRegisterAsync(request); 
        }

        */
    }
}