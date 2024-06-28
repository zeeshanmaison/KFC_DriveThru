using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveThru.Integration.Core.Configuration
{
    public class AppConfiguration
    {
        public string JwtSecretKey { get; set; }
        public string StoragePath { get; set; }
        public bool EnableAuditing { get; set; }
        public bool EnableCaching { get; set; }
    }
}
