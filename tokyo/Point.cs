using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tokyo
{
    public class Point
    {
        public Point(int x, int y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public int X;

        public int Y;

        public float Z;

        public static Point operator -(Point a, Point b)
        {
            return new Point(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static Point operator +(Point a, Point b)
        {
            return new Point(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Point operator /(Point a, int factor)
        {
            return new Point(a.X / factor, a.Y / factor, a.Z / factor);
        }

        public static Point operator *(Point a, int factor)
        {
            return new Point(a.X * factor, a.Y * factor, a.Z * factor);
        }

        public static bool operator ==(Point a, Point b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(Point a, Point b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is Point)
            {
                Point o = (Point)obj;
                return X == o.X && Y == o.Y;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return X * 13 + Y;
        }
    }
}
