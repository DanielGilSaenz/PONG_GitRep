using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PongCliente_Sockets.Menus
{
    class UserCursor
    {
        public string wordInside { get; set; }

        public UserCursor(string wordInside)
        {
            this.wordInside = wordInside;
        }

        public string getSelected()
        {
            return "=> " + wordInside + " <=";
        }

    }
}
