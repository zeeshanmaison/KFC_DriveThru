using Microsoft.Extensions.Configuration;
using DriveThru.Integration.Application.Services.Abstraction;
using DriveThru.Integration.Core.Helpers;
using DriveThru.Integration.DTO.Request;
using DriveThru.Integration.DTO.Response;

namespace DriveThru.Integration.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _configuration;

        public UserService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<IList<UserResponse>> GetUserList()
        {

            string url = _configuration.GetSection("ApiUrls:Domain").Value;

            url = url + "api/Employee/GetEmployeeList";

            List<UserResponse> response = new List<UserResponse>();
            try
            {
                IDictionary<string, object> formData = new Dictionary<string, object>();


                string apiResponse = await HttpHelper.Get(url, formData);
                response = JsonHelper.Deserialize<List<UserResponse>>(apiResponse);

            }
            catch (Exception ex)
            {
                string returnMsg = ex.Message;
            }
            return response;
        }

        public async Task<IList<UserResponse>> GetUserByEmail(string email)
        {

            string? url = _configuration.GetSection("ApiUrls:Domain").Value;

            url = url + "api/Employee?Email=" + email;
            //url = url + "api/Login?username="+email+"&password={password}"+email+"";

            IList<UserResponse> tokenResponse = new List<UserResponse>();
            try
            {
                IDictionary<string, object> formData = new Dictionary<string, object>();


                string response = await HttpHelper.Get(url, formData);
                tokenResponse = JsonHelper.Deserialize<IList<UserResponse>>(response);

            }
            catch (Exception ex)
            {
                string returnMsg = ex.Message;
            }
            return tokenResponse;
        }

        public async Task<bool> AccountRegisterAsync(IList<AccountRegisterRequest> request)
        {

            string url = _configuration.GetSection("ApiUrls:Domain").Value;

            url = url + "api/Customer/Location";
            //url = url + "api/Login?username="+email+"&password={password}"+email+"";

            bool isCreated = false;
            try
            {
                IDictionary<string, object> formData = new Dictionary<string, object>();

                foreach (var item in request)
                {
                    AccountRegisterRequest accountRegister = new AccountRegisterRequest
                    {
                        longitude = item.longitude,
                        latitude = item.latitude
                    };
                }


                string payload = JsonHelper.Serialize(request);

                string response = await HttpHelper.PostJson(url, payload);
                isCreated = JsonHelper.Deserialize<bool>(response);

            }
            catch (Exception ex)
            {
                string returnMsg = ex.Message;
            }
            return isCreated;
        }
    }
}
