using PongCliente_Sockets.Interfaces;
using PongCliente_Sockets.MVC.Model.Math_Objects;
using System;


namespace PongCliente_Sockets.MVC.Model.Serializable
{
    [Serializable]
    class Player : ICloneable, ICompareBool
    {
        // Is the cealing and the floor limit for the player
        public int maxY { get; set; }
        public int minY { get; set; }

        public Point pos { get; set; }
        public Point top { get; set; }
        public Point bottom { get; set; }

        public int size { get; set; }

        public ConsoleKey keyUp { get; set; }
        public ConsoleKey keyDown { get; set; }

        /// <summary> Configs the player key input and the player parameters</summary>
        public Player(ConsoleKey keyUp, ConsoleKey keyDown, int maxY, int minY, Point pos, int size)
        {
            this.keyUp = keyUp;
            this.keyDown = keyDown;
            this.maxY = maxY;
            this.minY = minY;
            this.pos = pos;
            this.size = size;

            top = new Point(pos.x, pos.y + 1 + size);
            bottom = new Point(pos.x, pos.y - 1 - size);

            if (top.y > maxY) throw new Exception("The position.y is too high and the object cannot be created");
            if (top.y < minY) throw new Exception("The position.y is too low and the object cannot be created");
        }

        /// <summary> Returns a line that represents the player</summary>
        public Line toLine()
        {
            return new Line(top,bottom);
        }

        /// <summary>  Checks if the player is out of bounds and moves it. 
        /// The y coordinate is inverted because of the console </summary>
        public void userUpdate(ConsoleKey s)
        {
            if ((s == keyUp) && (bottom.y > minY))
            {
                pos.y--;
                top.y--;
                bottom.y--;
            }
            if ((s == keyDown) && (top.y < maxY))
            {
                pos.y++;
                top.y++;
                bottom.y++;
            }

        }

        public bool Compare(object obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != typeof(Player)) return false;
            Player o = (Player)obj;

            if (o.maxY == maxY && o.minY == minY 
                && o.pos.Compare(pos) && o.top.Compare(top) && o.bottom.Compare(bottom) 
                && o.size == size && o.keyUp == keyUp && o.keyDown == keyDown) return true;
            else return false;
        }

        public object Clone()
        {
            return new Player(this.keyUp, this.keyDown, this.maxY, this.minY, (Point)pos.Clone(), this.size);
        }
    }
}
