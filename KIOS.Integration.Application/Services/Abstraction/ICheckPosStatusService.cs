using DriveThru.Integration.Core.Response;
using DriveThru.Integration.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveThru.Integration.Application.Services.Abstraction
{
    public interface ICheckPosStatusService
    {
        Task<ResponseModelWithClass<CheckPOSStatusReposne>> CheckPosStatusAsync(string storeId);
    }
}
