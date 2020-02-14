using PongCliente_Sockets.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PongCliente_Sockets.MVC.Model.Math_Objects
{
    class fPoint: ICloneable, ICompareBool
    {
      
        public float x { get; set; }
        public float y { get; set; }

        public fPoint(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public fPoint()
        {
        }

        public bool bothZero()
        {
            if ((x == 0) && (y == 0)) return true;
            else return false;
        }

        public object Clone()
        {
            return new fPoint(x, y);
        }

        public bool Compare(object obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != typeof(fPoint)) return false;
            fPoint o = (fPoint)obj;

            if (o.x == this.x && o.y == y) return true;
            else return false;
        }

        public static fPoint Cast(Point p1)
        {
            return new fPoint(p1.x, p1.y);
        }
        
    }
}
