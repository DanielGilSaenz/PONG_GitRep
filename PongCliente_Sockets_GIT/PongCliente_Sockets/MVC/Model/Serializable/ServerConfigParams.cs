using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
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

        public ServerConfigParams()
        {
        }

        public enum Mode
        {
            ONLINE, OFFLINE
        }

        public string IP { get; set; } = "127.0.0.1";
        public Int32 PORT { get; set; } = 8080;
        public Mode mode { get; set; } = Mode.OFFLINE;

        public TcpClient tcpClient { get; set; }
    }
}
