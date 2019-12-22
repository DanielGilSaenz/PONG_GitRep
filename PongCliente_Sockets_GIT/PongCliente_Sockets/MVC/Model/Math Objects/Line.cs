using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PongCliente_Sockets
{
    class Line : ICloneable, ICompareBool
    {
        public Point p1 { get; set; }
        public Point p2 { get; set; }

        public Line(Point p1, Point p2)
        {
            this.p1 = p1;
            this.p2 = p2;
        }

        public Vector getVector()
        {
            return new Vector(p2.x - p1.x, p2.y - p2.y);
        }

        public double Slope()
        {
            return ((double)p2.y - (double)p1.y) / ((double)p2.x - (double)p1.x);
        }

        public double Length()
        {
            return getVector().Length();
        }

        public int getFromEcuation_Y(int x, double m)
        {
            int y = (int)((m * (x - p1.x)) + p1.y);
            return y;
        }

        public int getFromEcuation_X(int y, double m)
        {
            int x = (int)((y - p1.y + (m * p1.x)) / m);
            return x;
        }

        public bool Compare(object obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != typeof(Line)) return false;
            Line o = (Line)obj;

            if (o.Compare(p1) && o.Compare(p2)) return true;
            else return false;
        }

        public object Clone()
        {
            return new Line((Point)p1.Clone(), (Point)p2.Clone());
        }
    }
}
