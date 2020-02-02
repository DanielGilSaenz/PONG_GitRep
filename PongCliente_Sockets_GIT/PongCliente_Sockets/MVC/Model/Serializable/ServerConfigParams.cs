using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PongCliente_Sockets.MVC.Model.Serializable
{
    class ServerConfigParams
    {
        public ServerConfigParams(string iP, Mode mode)
        {
            IP = iP;
            this.mode = mode;
        }

        public enum Mode
        {
            ONLINE, OFFLINE
        }

        public string IP { get; set; }
        public Mode mode { get; set; } = Mode.OFFLINE;
    }
}
