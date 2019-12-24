using PongCliente_Sockets.Interfaces;
using PongCliente_Sockets.MVC.Model.Math_Objects;
using System;
using System.Threading;

namespace PongCliente_Sockets.MVC.Model.Serializable
{
    [Serializable]
    class Ball : Mostrar, ICloneable, ICompareBool
    {
        public fPoint pos { get; set; }
        public fVector vector { get; set; }

        public Ball()
        {
        }

        public Ball(fPoint pos, fVector vector)
        {
            this.pos = pos;
            this.vector = vector;
            // Initializes the vector of the ball if it is 0
            if (this.vector.x == 0 && this.vector.y == 0)
            {
                this.vector = fVector.getRandom();
            }
        }

        public Ball(fPoint pos, fVector vector, bool beep) : this(pos, vector)
        {
            // Initializes the vector of the ball if it is 0
            if (this.vector.x == 0 && this.vector.y == 0)
            {
                this.vector = fVector.getRandom();
            }
        }

        /// <summary> Creates a thread to make a beep</summary>
        public void Beep()
        {
            Thread th = new Thread(() => Console.Beep());
            th.Start();
        }

        /// <summary> Shitty method, shouldn't exist </summary>
        public bool shouldMove(int n1, int n2)
        {
            if (n1 % n2 == 0) return true;
            else return false;
        }

        public object Clone()
        {
            return new Ball((fPoint)pos.Clone(), (fVector)vector.Clone());
        }

        public bool Compare(object obj)
        {
            if (obj == null) return false;
            if (obj == null) return false;
            if (obj.GetType() != typeof(Ball)) return false;
            Ball o = (Ball)obj;

            if (o.pos.Compare(pos) && o.vector.Compare(vector)) return true;
            else return false;
        }
    }
}
