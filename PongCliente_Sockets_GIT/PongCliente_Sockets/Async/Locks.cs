using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PongCliente_Sockets
{
    class Locks
    {
        public static bool DRAWING { get; set; } = false;
        public static bool READING { get; set; } = false;
    }
}
