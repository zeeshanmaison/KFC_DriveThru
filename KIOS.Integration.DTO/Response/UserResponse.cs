using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveThru.Integration.DTO.Response
{
    public class UserResponse
    {
        public string? Password { get; set; }
        public int? Id { get; set; }
        public int? EmpId { get; set; }
        public string? EmpName { get; set; }
        public string? Email { get; set; }
        public int? UserTypeId { get; set; }
        public bool IsADUser { get; set; }
        public int? ReportToUserId { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public int? Version { get; set; }
    }
}
