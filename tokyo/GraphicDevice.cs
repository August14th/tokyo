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
    class GraphicDevice : IDisposable
    {

        private readonly Bitmap canvas;

        private readonly Graphics canvasGraphics;

        private float[] zBuffer;

        public GraphicDevice(Bitmap bitmap)
        {
            canvas = bitmap;
            canvasGraphics = Graphics.FromImage(canvas);
            white = new byte[canvas.Width * canvas.Height * 4];
            zBuffer = new float[canvas.Width * canvas.Height];
        }

        private int Height => canvas.Height;

        private int Width => canvas.Width;

        private byte[] white;

        public void Clear(Color color)
        {
            if (white[0] != color.B || white[1] != color.G || white[2] != color.R || white[2] != color.A)
            {
                for (int i = 0; i < white.Length; i += 4)
                {
                    white[i] = color.B;
                    white[i + 1] = color.G;
                    white[i + 2] = color.R;
                    white[i + 3] = color.A;
                }
            }
            Array.Clear(zBuffer, 0, zBuffer.Length);
            var bits = canvas.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.WriteOnly, canvas.PixelFormat);
            Marshal.Copy(white, 0, bits.Scan0, white.Length);
            canvas.UnlockBits(bits);
        }

        public void Dispose()
        {
            canvasGraphics.Dispose();
        }

        public void DrawLine(Point p0, Point p1, Color color)
        {
            DrawPoint(p0, color);
            DrawPoint(p1, color);
            Point middle = (p0 + p1) / 2;
            if(middle == p0 || middle == p1) return;
            DrawLine(p0, middle, color);
            DrawLine(middle, p1, color);
        }

        public void DrawPoint(Point point, Color color)
        {
            var index = point.Y * Width + point.X;
            if (zBuffer[index] > point.Z)
            {
                return;
            }
            zBuffer[index] = point.Z;
            SetPixel(point.X, point.Y, color);
        }

        public void DrawString(string str, Font font, Brush brush, float x, float y)
        {
            canvasGraphics.DrawString(str, font, brush, x, y);
        }

        public void DrawTriangle(Vector pa, Vector pb, Vector pc, Color color, Camera camera)
        {
            var a = pa - pb;
            var b = pc - pb;
            var n = a.Cross(b);
            var v = camera.Target - camera.Position;
            if (n.Dot(v) >= 0)
            {
                return;
            }
            DrawLine(pa, pb, color);
            DrawLine(pa, pc, color);
            DrawLine(pc, pb, color);
        }

        public void DrawMeshes(Mesh[] meshes, Color color, Camera camera)
        {
            var view = Matrix.LookAtLH(camera.Position, camera.Target, Vector.UnitY);
            var projection = Matrix.PerspectiveFovLH((float)(Math.PI / 4.0), (float)Width / Height, 0.1f, 1);

            foreach (Mesh mesh in meshes)
            {
                Matrix rotation = Matrix.Rotation(mesh.Rotation);
                Matrix translation = Matrix.Translation(mesh.Position);

                Matrix world = rotation * translation;
                Matrix transform = world * view * projection;

                foreach (Surface face in mesh.Surfaces)
                {
                    Vector v1 = Project(mesh.Vertices[face.A], transform);
                    Vector v2 = Project(mesh.Vertices[face.B], transform);
                    Vector v3 = Project(mesh.Vertices[face.C], transform);
                    DrawTriangle(v1, v2, v3, color, camera);
                }
            }
        }

        private void SetPixel(int x, int y, Color color)
        {
            canvas.SetPixel(x, y, color);
        }

        private Vector Project(Vector src, Matrix transforom)
        {
            Vector target = transforom.Transform(src);

            target.X = target.X * Width + Width / 2f;
            target.Y = -target.Y * Height + Height / 2f;

            return target;
        }
    }
}
