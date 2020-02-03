using PongCliente_Sockets.MVC.Model.Serializable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PongCliente_Sockets.MVC.Controller
{
    class InputHandler
    {
        public static bool P1_KEY_UP   { get; set; }
        public static bool P1_KEY_DOWN { get; set; }
        public static bool P2_KEY_UP   { get; set; }
        public static bool P2_KEY_DOWN { get; set; }

        public static bool DEBUG_KEY { get; set; }
        public static bool ESCAPE_KEY { get; set; }

        public static Player player1 { get; set; }
        public static Player player2 { get; set; }

        public InputHandler(Player Player1, Player Player2)
        {
            player1 = Player1;
            player2 = Player2;
        }

        [STAThread]
        public void handleKeyboard()
        {
            P1_KEY_UP = Keyboard.IsKeyDown(player1.keyUp);
            P1_KEY_DOWN = Keyboard.IsKeyDown(player1.keyDown);
            P2_KEY_UP = Keyboard.IsKeyDown(player2.keyUp);
            P2_KEY_DOWN = Keyboard.IsKeyDown(player2.keyDown);

            DEBUG_KEY = Keyboard.IsKeyDown(Key.F3);

            ESCAPE_KEY = Keyboard.IsKeyDown(Key.Escape);
        }

        public static bool isKeyDown(Key key)
        {
            if(player1 != null)
            {
                if (key == player1.keyUp) return P1_KEY_UP;
                if (key == player1.keyDown) return P1_KEY_DOWN;
            }

            if (player2 != null)
            {
                if (key == player2.keyUp) return P2_KEY_UP;
                if (key == player2.keyDown) return P2_KEY_DOWN;
            }

            if (key == Key.F3) return DEBUG_KEY;
            if (key == Key.Escape) return ESCAPE_KEY;
            else return false;
        }
    }
}
