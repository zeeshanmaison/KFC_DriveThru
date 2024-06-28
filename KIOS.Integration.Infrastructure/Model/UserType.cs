using DriveThru.Integration.Core.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveThru.Integration.Infrastructure.Model
{
    [Table("UserType")]
    public class UserType : BasicEntity
    {
        public string Name { get; set; }
    }
}
