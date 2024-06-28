using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveThru.Integration.DTO.Response
{
    public class MZNPOSTERMINALINFOResponse
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
        public string Url { get; set; }
        public int Type { get; set; }
        public string PosId { get; set; }
    }
}
