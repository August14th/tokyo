using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tokyo
{
    class Matrix
    {
        public float[] Values { get; }

        public static Matrix Identity => new Matrix(new float[16]
        {
            1, 0, 0, 0,
            0, 1, 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1
        });

        public Matrix(float[] values = null)
        {
            Values = new float[16];
            if (values == null)
            {
                return;
            }
            for (int i = 0; i < Values.Length; i++)
            {
                Values[i] = values[i];
            }
        }

        public static bool operator ==(Matrix a, Matrix b)
        {
            return !a.Values.Where((t, i) => Math.Abs(t - b.Values[i]) > float.MinValue).Any();
        }

        public static Matrix operator *(Matrix a, Matrix b)
        {
            var values = new float[16];
            for (int idx = 0; idx < 16; idx++)
            {
                int row = idx / 4;
                int column = idx % 4;
                values[idx] = 
                     a.Values[row * 4]     * b.Values[column]         +
                     a.Values[row * 4 + 1] * b.Values[column + 4 * 1] +
                     a.Values[row * 4 + 2] * b.Values[column + 4 * 2] +
                     a.Values[row * 4 + 3] * b.Values[column + 4 * 3];
            }
            return new Matrix(values);
        }

        public static bool operator !=(Matrix a, Matrix b)
        {
            return !(a == b);
        }

        // https://msdn.microsoft.com/en-us/library/bb205342(v=vs.85).aspx
        public static Matrix LookAtLH(Vector eye, Vector forward, Vector up)
        {
            var zaxis = forward.Normalize();
            var xaxis = up.Cross(zaxis).Normalize();
            var yaxis = zaxis.Cross(xaxis).Normalize();

            return new Matrix(new float[16]
            {
                   xaxis.X,         yaxis.X,         zaxis.X,      0,
                   xaxis.Y,         yaxis.Y,         zaxis.Y,      0,
                   xaxis.Z,         yaxis.Z,         zaxis.Z,      0,
                -xaxis.Dot(eye), -yaxis.Dot(eye), -zaxis.Dot(eye), 1
            });
        }

        // https://msdn.microsoft.com/en-us/library/bb205350(VS.85).aspx
        public static Matrix PerspectiveFovLH(float fov, float aspect, float znear, float zfar)
        {
            float yScale = 1 / (float)Math.Tan(fov / 2);
            float xScale = yScale / aspect;
            return new Matrix(new float[16] {
                xScale,    0,               0,               0,
                0,      yScale,             0,               0,
                0,         0,       zfar/ (zfar - znear),    1,
                0,         0,   -znear * zfar/(zfar -znear), 0
            });
        }

        public static Matrix Rotation(Vector r)
        {
            var x = RotationX(r.X);
            var y = RotationY(r.Y);
            var z = RotationZ(r.Z);
            return z * x * y;
        }

        public static Matrix RotationX(float angle)
        {
            var s = (float)Math.Sin(angle);
            var c = (float)Math.Cos(angle);

            return new Matrix(new float[16] {
                1, 0,  0, 0,
                0, c,  s, 0,
                0, -s, c, 0,
                0, 0,  0, 1
            });
        }

        public static Matrix RotationY(float angle)
        {
            var s = (float)Math.Sin(angle);
            var c = (float)Math.Cos(angle);

            return new Matrix(new float[16] {
                c, 0,  -s, 0,
                0, 1,   0, 0,
                s, 0,   c, 0,
                0, 0,   0, 1
            });
        }

        public static Matrix RotationZ(float angle)
        {
            var s = (float)Math.Sin(angle);
            var c = (float)Math.Cos(angle);

            return new Matrix(new float[16] {
                c,  s,  0, 0,
                -s, c,  0, 0,
                0,  0,  1, 0,
                0,  0,  0, 1
            });
        }

        public static Matrix Translation(Vector t)
        {
            var values = new[] {
                1,    0,   0,  0,
                0,    1,   0,  0,
                0,    0,   1,  0,
                t.X, t.Y, t.Z, 1
            };
            return new Matrix(values);
        }

        public Vector Transform(Vector v)
        {
            var x = v.X * Values[0 * 4 + 0] + v.Y * Values[1 * 4 + 0] + v.Z * Values[2 * 4 + 0] + Values[3 * 4 + 0];
            var y = v.X * Values[0 * 4 + 1] + v.Y * Values[1 * 4 + 1] + v.Z * Values[2 * 4 + 1] + Values[3 * 4 + 1];
            var z = v.X * Values[0 * 4 + 2] + v.Y * Values[1 * 4 + 2] + v.Z * Values[2 * 4 + 2] + Values[3 * 4 + 2];
            var w = v.X * Values[0 * 4 + 3] + v.Y * Values[1 * 4 + 3] + v.Z * Values[2 * 4 + 3] + Values[3 * 4 + 3];
            return new Vector(x / w, y / w, z / w);
        }

        public bool Equals(Matrix other)
        {
            return Equals(Values, other.Values);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            return obj is Matrix && Equals((Matrix)obj);
        }

        public override int GetHashCode()
        {
            return Values?.GetHashCode() ?? 0;
        }

    }
}
