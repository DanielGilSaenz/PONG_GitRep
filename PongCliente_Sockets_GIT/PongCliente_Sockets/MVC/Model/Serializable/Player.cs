using PongCliente_Sockets.Interfaces;
using PongCliente_Sockets.MVC.Model.Math_Objects;
using System;
using System.Windows.Input;

namespace PongCliente_Sockets.MVC.Model.Serializable
{
    [Serializable]
    class Player : Mostrar, ICloneable, ICompareBool
    {
        // Is the cealing and the floor limit for the player
        public int maxY { get; set; }
        public int minY { get; set; }

        // Positional info
        public Point pos { get; set; }
        public Point top { get; set; }
        public Point bottom { get; set; }

        private int size { get; set; }

        // Directional info: pixel per second, direction +1 == up -1 == down
        public int pps { get; set; } = 3;
        public int direction { get; set; } = 0;

        // The configured keys
        public Key keyUp { get; set; }
        public Key keyDown { get; set; }

        // The keypress minimal delay to move the player in milliseconds
        public int keyPressMinDelay_milli = 12;

        public Player() { }

        /// <summary> Configs the player key input and the player parameters</summary>
        public Player(Key keyUp, Key keyDown, int maxY, int minY, Point pos, int size)
        {
            this.keyUp = keyUp;
            this.keyDown = keyDown;
            this.maxY = maxY;
            this.minY = minY;
            this.pos = pos;
            this.size = size;

            top = new Point(pos.x, pos.y + size);
            bottom = new Point(pos.x, pos.y - 1 - size);

            if (top.y > maxY) throw new Exception("The position.y is too high and the object cannot be created");
            if (top.y < minY) throw new Exception("The position.y is too low and the object cannot be created");
        }

        public void reconstruct(Key keyUp, Key keyDown, int maxY, int minY, Point pos, int size)
        {
            this.keyUp = keyUp;
            this.keyDown = keyDown;
            this.maxY = maxY;
            this.minY = minY;
            this.pos = pos;
            this.size = size;

            top = new Point(pos.x, pos.y + size);
            bottom = new Point(pos.x, pos.y - 1 - size);

            if (top.y > maxY) throw new Exception("The position.y is too high and the object cannot be created");
            if (top.y < minY) throw new Exception("The position.y is too low and the object cannot be created");
        }

        public void changeSize(int newSize)
        {
            reconstruct(this.keyUp, this.keyDown, this.maxY, this.minY, this.pos, newSize);
        }

        /// <summary> Returns a line that represents the player</summary>
        public Line toLine()
        {
            return new Line(top, bottom);
        }

        /// <summary>  Checks if the player is out of bounds and moves it. </summary>
        public void updatePos(Key s)
        {
            int distanceToWall;
            if ((s == keyUp) && (bottom.y > minY))
            {
                if (direction == 0) { direction--; pps = 1; }
                else if (direction < 0) pps++;

                distanceToWall = bottom.y - minY;

                if (bottom.y - minY < pps)
                {
                    pos.y -= distanceToWall;
                    top.y -= distanceToWall;
                    bottom.y -= distanceToWall;
                }
                else
                {
                    pos.y -= pps;
                    top.y -= pps;
                    bottom.y -= pps;
                }
                
            }

            else if ((s == keyDown) && (top.y < maxY))
            {
                if (direction == 0) { direction++;}
                else if (direction > 0) pps++;

                distanceToWall = maxY - top.y;

                if (maxY - top.y < pps)
                {
                    pos.y += distanceToWall;
                    top.y += distanceToWall;
                    bottom.y += distanceToWall;
                }
                else
                {
                    pos.y += pps;
                    top.y += pps;
                    bottom.y += pps;
                }
            }
            this.size = (top.y - bottom.y) / 2;
        }

        public void resetMomentum()
        {
            this.direction = 0;
            this.pps = 1;
        }

        /// <summary> Checks if the player is out of bounds and moves it several times</summary>
        public void updatePos(Key s, int times)
        {
            for(int i=0; i< times; i++) updatePos(s);
        }

        public bool Compare(object obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != typeof(Player)) return false;
            Player o = (Player)obj;

            if (o.maxY == maxY && o.minY == minY 
                && o.pos.Compare(pos) && o.top.Compare(top) && o.bottom.Compare(bottom) 
                && o.size == size && o.keyUp == keyUp && o.keyDown == keyDown
                && o.pps == pps && o.direction == direction) return true;
            else return false;
        }

        public object Clone()
        {
            return new Player(this.keyUp, this.keyDown, this.maxY, this.minY, (Point)pos.Clone(), this.size);
        }
    }
}
