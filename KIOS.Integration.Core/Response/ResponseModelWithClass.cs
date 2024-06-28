using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveThru.Integration.Core.Response
{
    public class ResponseModelWithClass<T> where T : class
    {
        public int MessageType { get; set; }
        public string Message { get; set; }
        public T Result { get; set; }
        public int HttpStatusCode { get; set; }
        public IList<string> Errors;

        public ResponseModelWithClass()
        {
            //
        }
    }
}
