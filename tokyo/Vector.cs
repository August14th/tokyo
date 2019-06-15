using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tokyo
{
    public class Vector
    {
        public static readonly Vector Zero = new Vector(0, 0, 0);

        public static readonly Vector One = new Vector(1, 1, 1);

        public static readonly Vector UnitX = new Vector(1, 0, 0);

        public static readonly Vector UnitY = new Vector(0, 1, 0);

        public static readonly Vector UnitZ = new Vector(0, 0, 1);

        public float[] Values { get; }

        public Vector(float x, float y, float z)
        {
            Values = new float[3];
            Values[0] = x;
            Values[1] = y;
            Values[2] = z;
        }

        public float X
        {
            get => Values[0];
            set => Values[0] = value;
        }

        public float Y
        {
            get => Values[1];
            set => Values[1] = value;
        }

        public float Z
        {
            get => Values[2];
            set => Values[2] = value;
        }

        public float Length => (float) Math.Sqrt(X * X + Y * Y + Z * Z);

        public float SqrLength => X * X + Y * Y + Z * Z;

        public Vector Normalize()
        {
            float length = Length;
            return new Vector(X / length, Y / length, Z / length);
        }

        public static Vector operator -(Vector a, Vector b)
        {
            return new Vector(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static Vector operator +(Vector a, Vector b)
        {
            return new Vector(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Vector operator /(Vector a, float factor)
        {
            return new Vector(a.X / factor, a.Y / factor, a.Z / factor);
        }

        public static Vector operator *(Vector a, float factor)
        {
            return new Vector(a.X * factor, a.Y * factor, a.Z * factor);
        }

        public Vector Cross(Vector v)
        {
            return new Vector(Y * v.Z - Z * v.Y, Z * v.X - X * v.Z, X * v.Y - Y * v.X);
        }

        public float Dot(Vector v)
        {
            return X * v.X + Y * v.Y + Z * v.Z;
        }

        public Vector Modulate(Vector v)
        {
            return new Vector(X * v.X, Y * v.Y, Z * v.Z);
        }

        public override string ToString()
        {
            return "(" + X + ", " + Y + ", " + Z + ")";
        }
    }
}
