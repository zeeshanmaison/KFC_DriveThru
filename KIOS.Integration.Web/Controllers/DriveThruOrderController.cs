using Microsoft.AspNetCore.Mvc;
using DriveThru.Integration.Application.Services.Abstraction;
using DriveThru.Integration.DTO.Request;
using DriveThru.Integration.DTO.Response;
using System.Threading.Tasks;
using DriveThru.Integration.Application.Commands;
using System.Data;
using DriveThru.Integration.Core.Enums;
using System.Net;
using DriveThru.Integration.Core.Response;

namespace DriveThru.Integration.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriveThruOrderController : ControllerBase
    {
        private readonly ILogger<DriveThruOrderController> _logger;
        private readonly ICreateOrderService _createOrderService;

        public DriveThruOrderController(ILogger<DriveThruOrderController> logger, ICreateOrderService createOrderService)
        {
            _logger = logger;
            _createOrderService = createOrderService;
        }

        [HttpPost]
        [Route("create-order-pos")]
        public async Task<ResponseModelWithClass<CreateOrderResponse>> CreateOrderKFC(DriveThru.Integration.Application.Commands.CreateRetailTransactionCommand request)
        {
            ResponseModelWithClass<CreateOrderResponse> response = new ResponseModelWithClass<CreateOrderResponse>();

            try
            {
                return await _createOrderService.CreateOrderKFC(request);
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.HttpStatusCode = (int)HttpStatusCode.InternalServerError;
                response.MessageType = (int)MessageType.Error;
                response.Message = "server error msg: "+ ex.Message +" | Inner exception:  " + ex.InnerException;
                return response;
            }
        }

        [HttpPost]
        [Route("create-driveThru-order")]
        public async Task<ResponseModelWithClass<CreateOrderResponse>> CreateDriveThruKFC(DriveThru.Integration.Application.Commands.CreateRetailTransactionCommand request)
        {
            ResponseModelWithClass<CreateOrderResponse> response = new ResponseModelWithClass<CreateOrderResponse>();

            try
            {
                request.Payment_method = PaymentMethod.Cash;
                request.AmountCur = 0;
                request.TenderTypeId = null;
                request.TableNum = null;

                return await _createOrderService.CreateOrderKFC(request);
            }
            catch (Exception ex)
            {
                response.Result = null;
                response.HttpStatusCode = (int)HttpStatusCode.InternalServerError;
                response.MessageType = (int)MessageType.Error;
                response.Message = "server error msg: " + ex.Message + " | Inner exception:  " + ex.InnerException;
                return response;
            }
        }
    }
}