using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PongCliente_Sockets
{
    class fVector : ICloneable, ICompareBool
    {
        public float x { get; set; }
        public float y { get; set; }

        public fVector(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public double Length()
        {
            return Math.Sqrt(x * x + y * y);
        }

        /// <summary> Gets a random vector wih values betwen -10 and +10</summary>
        public static fVector getRandom()
        {
            Random rnd = new Random();
            int[] vals = new int[] { -4, -3, -2, -1, 1, 2, 3, 4 };

            return new fVector(vals[rnd.Next(vals.Length)], vals[rnd.Next(vals.Length)]);
        }

        public bool Compare(object obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != typeof(fVector)) return false;
            fVector o = (fVector)obj;

            if (o.x == this.x && o.y == y) return true;
            else return false;
        }

        public object Clone()
        {
            return new fVector(x, y);
        }

    }
}
