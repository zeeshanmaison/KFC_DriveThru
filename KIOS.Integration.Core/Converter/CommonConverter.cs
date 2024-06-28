using DriveThru.Integration.Core.Model;
using DriveThru.Integration.Core.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveThru.Integration.Core.Converter
{
    public static class CommonConverter
    {
        public static void SetDefault(BasicEntity entity, BaseResponse response)
        {
            response.Id = entity.Id;
           
            response.GlobalId = entity.GlobalId;
           
            response.IsActive = entity.IsActive;
           
            response.IsDeleted = entity.IsDeleted;
            
            response.CreatedOn = entity.CreatedOn;
            
            response.CreatedBy = entity.CreatedBy;
            
            response.ModifiedOn = entity.ModifiedOn;
            
            response.ModifiedBy = entity.ModifiedBy;

        }
    }
}
