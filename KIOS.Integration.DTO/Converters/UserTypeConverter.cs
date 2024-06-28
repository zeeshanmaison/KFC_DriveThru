using DriveThru.Integration.Core.Converter;
using DriveThru.Integration.DTO.Response;
using DriveThru.Integration.Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveThru.Integration.DTO.Converters
{
    public static class UserTypeConverter
    {
        public static UserTypeResponse ConvertToResponse(this UserType entity)
        {
            if (entity != null && entity.IsActive == true && entity.IsDeleted == false)
            {
                UserTypeResponse response = new UserTypeResponse();

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
