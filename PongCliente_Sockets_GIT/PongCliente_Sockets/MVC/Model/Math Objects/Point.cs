using PongCliente_Sockets.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PongCliente_Sockets.MVC.Model.Math_Objects
{
    class Point :ICloneable, ICompareBool
    {
        public int x { get; set; }
        public int y { get; set; }

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public bool bothZero()
        {
            if ((x == 0) && (y == 0)) return true;
            else return false;
        }

        public object Clone()
        {
            return new Point(x, y);
        }

        public bool Compare(object obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != typeof(Point)) return false;
            Point o = (Point)obj;

            if (o.x == this.x && o.y == y) return true;
            else return false;
        }

        public static Point Cast(fPoint p1)
        {
            return new Point((int)p1.x, (int)p1.y);
        }
    }
}
