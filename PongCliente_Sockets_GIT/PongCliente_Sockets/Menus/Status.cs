using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PongCliente_Sockets
{
    class Status
    {
        public string Message { get; set; }
        public string StatusName { get; set; }

        public Status(string statusName, string message)
        {
            this.Message = message;
            StatusName = statusName;
        }
    }
}
