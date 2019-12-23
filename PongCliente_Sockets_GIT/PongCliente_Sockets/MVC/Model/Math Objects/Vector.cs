using System;

namespace PongCliente_Sockets
{
    class Vector : ICloneable, ICompareBool
    {
        public int x { get; set; }
        public int y { get; set; }

        public Vector(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public double Length()
        {
            return Math.Sqrt(x * x + y * y);
        }

        /// <summary> Gets a random vector wih values betwen -10 and +10</summary>
        public static Vector getRandom()
        {
            Random rnd = new Random();
            return new Vector(
                Resources.rndVectorValues[rnd.Next(Resources.rndVectorValues.Length)], 
                Resources.rndVectorValues[rnd.Next(Resources.rndVectorValues.Length)]);
        }

        public bool Compare(object obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != typeof(Vector)) return false;
            Vector o = (Vector)obj;

            if (o.x == this.x && o.y == y) return true;
            else return false;
        }

        public object Clone()
        {
            return new Vector(x, y);
        }
    }
}
