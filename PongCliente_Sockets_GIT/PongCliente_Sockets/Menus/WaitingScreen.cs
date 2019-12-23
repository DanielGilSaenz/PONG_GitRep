using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PongCliente_Sockets.Menus
{
    class WaitingScreen
    {
        Status current;

        public WaitingScreen(Status current)
        {
            this.current = current;
        }
    }
}
