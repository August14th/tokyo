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
    public class GraphicDevice : IDisposable
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

        public float Clamp(float value, float min = 0, float max = 1)
        {
            return Math.Max(min, Math.Min(value, max));
        }

        public float Interpolate(float min, float max, float gradient)
        {
            return min + (max - min) * Clamp(gradient);
        }

        private void ProcessScanLine(int y, Vector pa, Vector pb, Vector pc, Vector pd, Color color)
        {
            var gradient1 = pa.Y != pb.Y ? (y - pa.Y) / (pb.Y - pa.Y) : 1;
            var gradient2 = pc.Y != pd.Y ? (y - pc.Y) / (pd.Y - pc.Y) : 1;

            int sx = (int)Interpolate(pa.X, pb.X, gradient1);
            int ex = (int)Interpolate(pc.X, pd.X, gradient2);

            float sz = Interpolate(pa.Z, pb.Z, gradient1);
            float ez = Interpolate(pc.Z, pd.Z, gradient2);

            for (int x = sx; x < ex; x++)
            {
                float gradient = (x - sx) / (float)(ex - sx);
                var z = Interpolate(sz, ez, gradient);
                DrawPoint(new Vector(x, y, z), color);
            }
        }

        public void DrawTriangle(Vector p1, Vector p2, Vector p3, Color color)
        {
            if (p1.Y > p2.Y)
            {
                var temp = p2;
                p2 = p1;
                p1 = temp;
            }

            if (p2.Y > p3.Y)
            {
                var temp = p2;
                p2 = p3;
                p3 = temp;
            }

            if (p1.Y > p2.Y)
            {
                var temp = p2;
                p2 = p1;
                p1 = temp;
            }

            float dP1P2, dP1P3;
            if (p2.Y - p1.Y > 0)
                dP1P2 = (p2.X - p1.X) / (p2.Y - p1.Y);
            else
                dP1P2 = 0;
            if (p3.Y - p1.Y > 0)
                dP1P3 = (p3.X - p1.X) / (p3.Y - p1.Y);
            else
                dP1P3 = 0;

            if (dP1P2 > dP1P3)
            {

                for (int y = (int)p1.Y; y <= p3.Y; y++)
                {
                    if (y < p2.Y)
                    {
                        ProcessScanLine(y, p1, p3, p1, p2, color);
                    }
                    else
                    {
                        ProcessScanLine(y, p1, p3, p2, p3, color);
                    }
                }
            }
            else
            {
                for (int y = (int)p1.Y; y <= p3.Y; y++)
                {
                    if (y < p2.Y)
                    {
                        ProcessScanLine(y, p1, p2, p1, p3, color);
                    }
                    else
                    {
                        ProcessScanLine(y, p2, p3, p1, p3, color);
                    }
                }
            }
        }

        public void DrawLine(Vector p0, Vector p1, Color color)
        {
            DrawPoint(p0, color);
            DrawPoint(p1, color);
            Vector middle = (p0 + p1) / 2;
            if (middle == p0 || middle == p1) return;
            DrawLine(p0, middle, color);
            DrawLine(middle, p1, color);
        }

        public void DrawPoint(Vector point, Color color)
        {
            int px = (int)point.X;
            int py = (int)point.Y;
        
            if (px >= 0 && px < Width && py >= 0 && py < Height)
            {
                var index = py * Width + px;
                if (zBuffer[index] > point.Z)
                {
                    return;
                }
                zBuffer[index] = point.Z;
                SetPixel(px, py, color);
            }
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
            if (n.Dot(v) < 0)
            {
                return;
            }
            DrawTriangle(pa, pb, pc, color);
        }

        public void DrawMeshes(Mesh[] meshes, Color color, Camera camera)
        {
            var view = Matrix.LookAtLH(camera.Position, camera.Target, Vector.UnitY);
            var projection = Matrix.PerspectiveFovLH(camera.Fov, (float)Width / Height, camera.ZNear, camera.ZFar);

            foreach (Mesh mesh in meshes)
            {
                Matrix rotation = Matrix.Rotation(mesh.Rotation);
                Matrix translation = Matrix.Translation(mesh.Position);

                Matrix world = rotation * translation;
                Matrix transform = world * view * projection;

                for (int i = 0; i < mesh.Surfaces.Length; i++)
                {
                    var face = mesh.Surfaces[i];
                    Vector v1 = Project(mesh.Vertices[face.A], transform);
                    Vector v2 = Project(mesh.Vertices[face.B], transform);
                    Vector v3 = Project(mesh.Vertices[face.C], transform);

                    Color[] colors = new Color[] { Color.Gray, Color.White, Color.Silver};
                    DrawTriangle(v1, v2, v3, colors[i % colors.Length], camera);
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
