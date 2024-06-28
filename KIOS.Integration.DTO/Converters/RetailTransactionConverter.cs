using KIOS.Integration.Core.Converter;
using KIOS.Integration.DTO.Response;
using KIOS.Integration.Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KIOS.Integration.DTO.Converters
{
    public static class RetailTransactionConverter
    {
        public static CreateOrderResponse ConvertToResponse(this RetailTransaction entity)
        {
            if (entity != null && entity.IsActive == true && entity.IsDeleted == false)
            {
                CreateOrderResponse response = new CreateOrderResponse();

                CommonConverter.SetDefault(entity, response);
                //response.Id = 0;
                response.Name = entity.Name;

                return response;
            }
            else
            {
                return null;
            }
        }
    }
}
