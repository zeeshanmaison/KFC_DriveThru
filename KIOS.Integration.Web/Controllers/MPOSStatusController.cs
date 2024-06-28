using Microsoft.AspNetCore.Mvc;
using DriveThru.Integration.Application.Commands;
using MediatR;
using DriveThru.Integration.Infrastructure.Model;
using DriveThru.Integration.Application.Services.Abstraction;
using DriveThru.Integration.Core.Response;
using DriveThru.Integration.DTO.Response;
using DriveThru.Integration.Core.Enums;
using System.Net;

namespace DriveThru.Integration.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MPOSStatusController : ControllerBase
    {
        private readonly ICheckPosStatusService _checkPosStatusService;

        public MPOSStatusController(ICheckPosStatusService checkPosStatusService)
        {
            _checkPosStatusService = checkPosStatusService;
        }
               
         //http://localhost:8082/api/MPOSStatus/check-pos-status?storeId=0072
        [HttpGet]
        [Route("check-pos-status")]
        public async Task<ResponseModelWithClass<CheckPOSStatusReposne>> CheckPosStatusAsync(string storeId)
        {
            ResponseModelWithClass<CheckPOSStatusReposne> response = new ResponseModelWithClass<CheckPOSStatusReposne>();

            try
            {
                return await _checkPosStatusService.CheckPosStatusAsync(storeId);

            }
            catch (Exception ex)
            {
                response.Result = null;
                response.Message = ex.Message;
                response.HttpStatusCode = (int)HttpStatusCode.InternalServerError;
                response.MessageType = (int)MessageType.Error;
                return response;
            }
        }
        
    }
}