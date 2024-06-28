using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveThru.Integration.DTO.Request
{
    public class AccountRegisterRequest
    {
        //public string Email { get; set; }
        //public string Password { get; set; }
        //public string ConfirmPassword { get; set; }

        public int longitude { get; set; }
        public int latitude { get; set; }
    }
}
