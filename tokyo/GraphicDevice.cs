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

        private byte[] bg;

        private int Height => canvas.Height;

        private int Width => canvas.Width;

        public Camera Camera
        {
            get; set;
        }

        public Vector LightPos
        {
            get; set;
        }

        public Color FrontColor
        {
            get; set;
        }

        public RenderMode RenderMode
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

                    switch (RenderMode)
                    {
                        case RenderMode.WireFrame:
                            DrawTriangle0(v1, v2, v3);
                            break;
                        case RenderMode.PhongLight:
                        case RenderMode.FlatLight:
                            DrawTriangle1(v1, v2, v3);
                            break;
                    }
                }
            }
        }

        public void DrawTriangle0(Vertex v1, Vertex v2, Vertex v3)
        {
            DrawLine(v1.Pos, v2.Pos);
            DrawLine(v2.Pos, v3.Pos);
            DrawLine(v3.Pos, v1.Pos);
        }

        public void DrawTriangle1(Vertex v1, Vertex v2, Vertex v3)
        {
            var a = v1.Pos - v2.Pos;
            var b = v3.Pos - v2.Pos;
            var n = a.Cross(b);
            var v = Camera.Target - Camera.Position;
            if (n.Dot(v) > 0) return;

            if (v1.Pos.Y > v2.Pos.Y)
            {
                var temp = v2;
                v2 = v1;
                v1 = temp;
            }

            if (v2.Pos.Y > v3.Pos.Y)
            {
                var temp = v2;
                v2 = v3;
                v3 = temp;
            }

            if (v1.Pos.Y > v2.Pos.Y)
            {
                var temp = v2;
                v2 = v1;
                v1 = temp;
            }

            float dP1P2, dP1P3;

            Vector p1 = v1.Pos;
            Vector p2 = v2.Pos;
            Vector p3 = v3.Pos;

            if (p2.Y - p1.Y > 0)
                dP1P2 = (p2.X - p1.X) / (p2.Y - p1.Y);
            else
                dP1P2 = 0;
            if (p3.Y - p1.Y > 0)
                dP1P3 = (p3.X - p1.X) / (p3.Y - p1.Y);
            else
                dP1P3 = 0;

            float nl1 = ComputeNDotL(v1.Coord, v1.Normal);
            float nl2 = ComputeNDotL(v2.Coord, v2.Normal);
            float nl3 = ComputeNDotL(v3.Coord, v3.Normal);

            Vector facePos = (v1.Coord + v2.Coord + v3.Coord) / 3;
            Vector faceNormal = (v1.Normal + v2.Normal + v3.Normal) / 3;
            float nl = ComputeNDotL(facePos, faceNormal);

            ScanLineData data = new ScanLineData { };
            data.ndotl = nl;

            if (dP1P2 > dP1P3)
            {
                for (int y = (int)p1.Y; y <= p3.Y; y++)
                {
                    data.Y = y;
                    if (y < p2.Y)
                    {
                        data.ndotla = nl1;
                        data.ndotlb = nl3;
                        data.ndotlc = nl1;
                        data.ndotld = nl2;
                        ProcessScanLine(data, p1, p3, p1, p2);
                    }
                    else
                    {
                        data.ndotla = nl1;
                        data.ndotlb = nl3;
                        data.ndotlc = nl2;
                        data.ndotld = nl3;
                        ProcessScanLine(data, p1, p3, p2, p3);
                    }
                }
            }
            else
            {
                for (int y = (int)p1.Y; y <= p3.Y; y++)
                {
                    data.Y = y;
                    if (y < p2.Y)
                    {
                        data.ndotla = nl1;
                        data.ndotlb = nl2;
                        data.ndotlc = nl1;
                        data.ndotld = nl3;
                        ProcessScanLine(data, p1, p2, p1, p3);
                    }
                    else
                    {
                        data.ndotla = nl2;
                        data.ndotlb = nl3;
                        data.ndotlc = nl1;
                        data.ndotld = nl3;
                        ProcessScanLine(data, p2, p3, p1, p3);
                    }
                }
            }
        }

        public void DrawLine(Vector p0, Vector p1)
        {
            var distance = (p1 - p0).Length;
            if (distance < 1) return;

            Vector middle = (p0 + p1) / 2;
            DrawPoint(middle, FrontColor);
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
                Normal = world.Transform(src.Normal)
            };
        }

        private float Clamp(float value, float min = 0, float max = 1)
        {
            return Math.Max(min, Math.Min(value, max));
        }

        private float Interpolate(float min, float max, float gradient)
        {
            return min + (max - min) * Clamp(gradient);
        }

        private void ProcessScanLine(ScanLineData data, Vector pa, Vector pb, Vector pc, Vector pd)
        {
            var gradient1 = pa.Y != pb.Y ? (data.Y - pa.Y) / (pb.Y - pa.Y) : 1;
            var gradient2 = pc.Y != pd.Y ? (data.Y - pc.Y) / (pd.Y - pc.Y) : 1;

            int sx = (int)Interpolate(pa.X, pb.X, gradient1);
            int ex = (int)Interpolate(pc.X, pd.X, gradient2);

            float sz = Interpolate(pa.Z, pb.Z, gradient1);
            float ez = Interpolate(pc.Z, pd.Z, gradient2);

            float snl = Interpolate(data.ndotla, data.ndotlb, gradient1);
            float enl = Interpolate(data.ndotlc, data.ndotld, gradient2);

            for (int x = sx; x < ex; x++)
            {
                float gradient = (x - sx) / (float)(ex - sx);
                var z = Interpolate(sz, ez, gradient);
                float nl = 0;
                switch (RenderMode)
                {
                    case RenderMode.FlatLight:
                        nl = data.ndotl;
                        break;
                    case RenderMode.PhongLight:
                        nl = Interpolate(snl, enl, gradient);
                        break;
                }                 
                DrawPoint(new Vector(x, data.Y, z), Color.FromArgb((int)(FrontColor.R * nl), (int)(FrontColor.G * nl), (int)(FrontColor.A * nl)));
            }
        }

        private float ComputeNDotL(Vector vertex, Vector normal)
        {
            Vector lightDirection = (LightPos - vertex).Normalize();
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

        public float Y;
    }
}
