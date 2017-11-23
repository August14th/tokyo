using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace tokyo
{
    public abstract class GraphicDevice : IDisposable
    {
        private readonly Bitmap canvas;

        private readonly Graphics canvasGraphics;

        private float[] zBuffer;

        private byte[] bg;

        private int Height => canvas.Height;

        private int Width => canvas.Width;

        public Camera Camera
        {
            get; set;
        }

        public Light Light
        {
            get; set;
        }

        public Color BaseColor
        {
            get; set;
        }

        public GraphicDevice(Bitmap bitmap)
        {
            canvas = bitmap;
            canvasGraphics = Graphics.FromImage(canvas);
            bg = new byte[canvas.Width * canvas.Height * 4];
            zBuffer = new float[canvas.Width * canvas.Height];
        }       

        public void Clear(Color color)
        {
            if (bg[0] != color.B || bg[1] != color.G || bg[2] != color.R || bg[2] != color.A)
            {
                for (int i = 0; i < bg.Length; i += 4)
                {
                    bg[i] = color.B;
                    bg[i + 1] = color.G;
                    bg[i + 2] = color.R;
                    bg[i + 3] = color.A;
                }
            }
            var bits = canvas.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.WriteOnly, canvas.PixelFormat);
            Marshal.Copy(bg, 0, bits.Scan0, bg.Length);
            canvas.UnlockBits(bits);
            for (var index = 0; index < zBuffer.Length; index++)
            {
                zBuffer[index] = float.MaxValue;
            }
        }

        public void DrawMeshes(Mesh[] meshes)
        {
            var view = Matrix.LookAtLH(Camera.Position, Camera.Target, Vector.UnitY);
            var projection = Matrix.PerspectiveFovLH(Camera.Fov, (float)Width / Height, Camera.ZNear, Camera.ZFar);

            foreach (Mesh mesh in meshes)
            {
                Matrix rotation = Matrix.Rotation(mesh.Rotation);
                Matrix translation = Matrix.Translation(mesh.Position);

                Matrix world = rotation * translation;
                Matrix transform = world * view * projection;

                for (int i = 0; i < mesh.Surfaces.Length; i++)
                {
                    var face = mesh.Surfaces[i];
                    Vertex v1 = Project(mesh.Vertices[face.A], transform, world);
                    Vertex v2 = Project(mesh.Vertices[face.B], transform, world);
                    Vertex v3 = Project(mesh.Vertices[face.C], transform, world);

                    DrawTriangle(v1, v2, v3, mesh.Texture);
                }
            }
        }

        public abstract void DrawTriangle(Vertex v1, Vertex v2, Vertex v3, Texture texture);

        public void DrawLine(Vector p0, Vector p1)
        {
            var distance = (p1 - p0).Length;
            if (distance < 1) return;

            Vector middle = (p0 + p1) / 2;
            DrawPoint(middle, BaseColor);
            DrawLine(p0, middle);
            DrawLine(middle, p1);
        }

        public void DrawPoint(Vector point, Color color)
        {
            int px = (int)point.X;
            int py = (int)point.Y;

            if (px >= 0 && px < Width && py >= 0 && py < Height)
            {
                var index = py * Width + px;
                if (zBuffer[index] < point.Z)
                {
                    return;
                }
                zBuffer[index] = point.Z;
                canvas.SetPixel(px, py, color);
            }
        }

        public void DrawString(string str, Font font, Brush brush, float x, float y)
        {
            canvasGraphics.DrawString(str, font, brush, x, y);
        }

        private Vertex Project(Vertex src, Matrix transforom, Matrix world)
        {
            Vector pos = transforom.Transform(src.Coord);

            pos.X = pos.X * Width + Width / 2f;
            pos.Y = -pos.Y * Height + Height / 2f;

            return new Vertex
            {
                Pos = pos,
                Coord = world.Transform(src.Coord),
                Normal = world.Transform(src.Normal),
                u = src.u,
                v = src.v
            };
        }

        protected float Clamp(float value, float min = 0, float max = 1)
        {
            return Math.Max(min, Math.Min(value, max));
        }

        protected float Interpolate(float min, float max, float gradient)
        {
            return min + (max - min) * Clamp(gradient);
        }

        protected float ComputeNDotL(Vector vertex, Vector normal)
        {
            Vector lightDirection = (Light.Pos - vertex).Normalize();
            return Math.Max(0, lightDirection.Dot(normal.Normalize()));
        }

        public void Dispose()
        {
            canvasGraphics.Dispose();
        }
    }

    class ScanLineData
    {
        public float ndotla;

        public float ndotlb;

        public float ndotlc;

        public float ndotld;

        public float ndotl;

        public float ua;

        public float ub;

        public float uc;

        public float ud;

        public float va;

        public float vb;

        public float vc;

        public float vd;

        public float Y;
    }
}
