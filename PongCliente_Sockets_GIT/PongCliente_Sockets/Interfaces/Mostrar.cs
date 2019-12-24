using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PongCliente_Sockets.Interfaces
{
    class Mostrar
    {
        public string getAttr(object obj)
        {
            return JsonSerializer.Serialize(obj);
        }
    }
}
