using DriveThru.Integration.Application.Commands;
using DriveThru.Integration.Core.Response;
using DriveThru.Integration.DTO.Request;
using DriveThru.Integration.DTO.Response;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveThru.Integration.Application.Services.Abstraction
{
    public interface ICreateOrderService
    {
        Task<ResponseModelWithClass<CreateOrderResponse>> CreateOrderKFC(DriveThru.Integration.Application.Commands.CreateRetailTransactionCommand request);
        Task<ResponseModelWithClass<CreateOrderResponse>> CreateOrderKFCA(DriveThru.Integration.Application.Commands.CreateRetailTransactionCommand request);
    }
}
