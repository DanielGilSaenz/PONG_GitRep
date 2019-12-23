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

        /// <summary> Having [x] and the slope[m] of the line, gets a value of [y] that corresponds to the line</summary>
        public int getFromEquation_Y(int x, double m)
        {
            int y = (int)((m * (x - p1.x)) + p1.y);
            return y;
        }

        /// <summary> Having [y] and the slope[m] of the line, gets a value of [x] that corresponds to the line</summary>
        public int getFromEquation_X(int y, double m)
        {
            int x = (int)((y - p1.y + (m * p1.x)) / m);
            return x;
        }

        public List<Point> getPoints()
        {
            List<Point> pointsOfTheLine = new List<Point>();

            int diffX, diffY;
            int x1, y1;
            int x2, y2;

            // Gets the y values and the x values organized from smaller to bigger
            if (this.p1.y > this.p2.y)  { y1 = this.p2.y; y2 = this.p1.y; }
            else                        { y1 = this.p1.y; y2 = this.p2.y; }

            if (this.p1.x > this.p2.x)  { x1 = this.p2.x; x2 = this.p1.x; }
            else                        { x1 = this.p1.x; x2 = this.p2.x; }

            // To get the difference betwen them and execute different algorithm
            // depending on which one is bigger
            diffX = x2 - x1;
            diffY = y2 - y1;


            // If the line is completely vertical its slope is infinite
            double m = this.Slope();
            if (double.IsInfinity(m))
            {
                // Checks which one is bigger to go over the values using different method. 
                // It does this to know if it has to subtract or add to get to the last value
                if(this.p1.y > this.p2.y)
                {
                    for (int i = p1.y; i > p2.y; i--)
                    {
                        pointsOfTheLine.Add(new Point(x1, i));
                    }
                }
                else
                {
                    for (int i = p1.y; i < p2.y; i++)
                    {
                        pointsOfTheLine.Add(new Point(x1, i));
                    }
                }
                
            }
            else
            {
                // If the difference in X values is greater, for example,
                // it means there are a greater range of values in the [x], so it's wiser
                // to use the equation that gives [y] values, and use the [x] values in the for loop
                // in order to get a more accurate representation of the line
                if (diffX > diffY)
                {
                    // Checks which one is bigger to go over the values using different method. 
                    // It does this to know if it has to subtract or add to get to the last value
                    if (this.p1.x > this.p2.x)
                    {
                        for (int height, i = p1.x; i > p2.x; i--)
                        {
                            height = this.getFromEquation_Y(i, m);
                            pointsOfTheLine.Add(new Point(i, height));
                        }
                    }
                    else
                    {
                        for (int height, i = p1.x; i < p2.x; i++)
                        {
                            height = this.getFromEquation_Y(i, m);
                            pointsOfTheLine.Add(new Point(i, height));
                        }
                    }
                    
                }
                else
                {
                    if (this.p1.y > this.p2.y)
                    {
                        for (int left, i = p1.y; i > p2.y; i--)
                        {
                            left = (int)this.getFromEquation_X(i, m);
                            pointsOfTheLine.Add(new Point(left, i));
                        }
                    }
                    else
                    {
                        for (int left, i = p1.y; i < p2.y; i++)
                        {
                            left = (int)this.getFromEquation_X(i, m);
                            pointsOfTheLine.Add(new Point(left, i));
                        }
                    }
                    
                }
            }

            return pointsOfTheLine;
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
