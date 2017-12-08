using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tokyo
{
    public static class ColorX
    {
        public static Color Multiply(this Color color, float r)
        {
            return FromVector(color.Vector().Modulate(new Vector(r, r, r)));
        }

        public static Color Multiply(this Color color, Vector v)
        {
            return FromVector(color.Vector().Modulate(v));
        }

        public static Color Multiply(this Color color, Color other)
        {
            return FromVector(color.Vector().Modulate(other.Vector()));
        }

        public static Color Add(this Color color, Color other)
        {
            return FromVector(color.Vector() + other.Vector());
        }

        public static Color Sub(this Color color, Color other)
        {
            return FromVector(color.Vector() - other.Vector());
        }

        private static Color FromVector(Vector v)
        {
            return Color.FromArgb((int)(255 * Math.Max(Math.Min(v.X, 1), 0)), (int)(255 * Math.Max(Math.Min(v.Y, 1), 0)), (int)(255 * Math.Max(Math.Min(v.Z, 1), 0)));
        }

        public static Vector Vector(this Color color)
        {
            return new Vector(color.R / 255f, color.G / 255f, color.B / 255f);
        }
    }
}
