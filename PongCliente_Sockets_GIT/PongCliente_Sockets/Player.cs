using PongCliente_Sockets.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PongCliente_Sockets
{
    class Player
    {
        // Is the cealing and the floor limit for the player
        public int maxY { get; set; }
        public int minY { get; set; }

        Point position;

        // The total size of the players is sizeY + 1 + sizeY; 
        public int sizeY { get; set; }

        public ConsoleKey keyUp { get; set; }
        public ConsoleKey keyDown { get; set; }

        /// <summary> Configs the user key input </summary
        public Player(ConsoleKey keyUp, ConsoleKey keyDown)
        {
            this.keyUp = keyUp;
            this.keyDown = keyDown;
        }

        public Player(int maxY, int minY, Point position, int sizeY)
        {
            this.maxY = maxY;
            this.minY = minY;
            this.position = position;
            this.sizeY = sizeY;
        }


        /// <summary>  Checks if the player is out of bounds and moves it </summary>
        public void userUpdate(ConsoleKey s)
        {
            if ((s == keyUp) && (position.y < maxY - sizeY)) position.y++;
            if ((s == keyDown) && (position.y > minY + sizeY)) position.y--;

        }
        
    }
}
