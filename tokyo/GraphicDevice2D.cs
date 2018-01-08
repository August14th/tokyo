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
    public class GraphicDevice2D : IDisposable
    {
        protected readonly Bitmap canvas;

        protected readonly Graphics canvasGraphics;

        protected float[] zBuffer;

        protected byte[] bg;

        protected int Height => canvas.Height;

        protected int Width => canvas.Width;


        public Color BaseColor
        {
            get; set;
        }

        public GraphicDevice2D(Bitmap bitmap)
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

        public void DrawLine(Point p1, Point p2)
        {
            int x1 = p1.X, y1 = p1.Y, x2 = p2.X, y2 = p2.Y;
            int dx = x2 - x1, dy = y2 - y1;
            int ux = dx > 0 ? 1 : -1;
            int uy = dy > 0 ? 1 : -1;

            int x = x1, y = y1, eps = 0;
            dx = Math.Abs(dx); dy = Math.Abs(dy);
            if (dx > dy)
            {
                for (; x != x2 + ux; x += ux)
                {
                    DrawPoint(new Point(x, y), BaseColor);
                    eps += dy;
                    if ((eps << 1) >= dx)
                    {
                        y += uy; eps -= dx;
                    }
                }
            }
            else
            {
                for (; y != y2 + uy; y += uy)
                {
                    DrawPoint(new Point(x, y), BaseColor);
                    eps += dx;
                    if ((eps << 1) >= dy)
                    {
                        x += ux; eps -= dy;
                    }
                }
            }
        }

        public void DrawPoint(Point point, Color color)
        {
            int px = point.X;
            int py = point.Y;

            if (px >= 0 && px < Width && py >= 0 && py < Height)
            {
                var index = py * Width + px;
                canvas.SetPixel(px, py, color);
            }
        }

        public void DrawString(string str, Font font, Brush brush, float x, float y)
        {
            canvasGraphics.DrawString(str, font, brush, x, y);
        }

        // 判断一点 (px, py) 是否在胶囊体（两端为(ax, ay)、(bx, by)，半径 r）之中
        public bool IsInCapsule(float px, float py, float ax, float ay, float bx, float by, float r)
        {
            return CapsuleSDF(px, py, ax, ay, bx, by, r) <= 0;
        }

        // 带符号距离场, 边界值为0，胶囊体内小于0，胶囊体外大于0
        public float CapsuleSDF(float px, float py, float ax, float ay, float bx, float by, float r)
        {
            float pax = px - ax, pay = py - ay, bax = bx - ax, bay = by - ay;
            float h = Math.Max(Math.Min((pax * bax + pay * bay) / (bax * bax + bay * bay), 1.0f), 0.0f);
            float dx = pax - bax * h, dy = pay - bay * h;

            return (float)Math.Sqrt(dx * dx + dy * dy) - r;
        }

        public void lineSDFAABB(float ax, float ay, float bx, float by, float r)
        {
            int x0 = (int)Math.Floor(Math.Min(ax, bx) - r);
            int x1 = (int)Math.Ceiling(Math.Max(ax, bx) + r);
            int y0 = (int)Math.Floor(Math.Min(ay, by) - r);
            int y1 = (int)Math.Ceiling(Math.Max(ay, by) + r);

            for (int y = y0; y <= y1; y++)
            {
                for (int x = x0; x <= x1; x++)
                {
                    int alpha = (int)(Math.Max(Math.Min(0.5f - CapsuleSDF(x, y, ax, ay, bx, by, r), 1.0f), 0.0f) * 255);
                    alphaBlend(x, y, Color.FromArgb(alpha, 0, 0, 0));
                }
            }
        }

        private void alphaBlend(int x, int y, Color color)
        {
            float alpha = color.A / 255f;
            DrawPoint(new Point(x, y), canvas.GetPixel(x, y).Multiply(1 - alpha).Add(color.Multiply(alpha)));
        }

        protected float Clamp(float value, float min = 0, float max = 1)
        {
            return Math.Max(min, Math.Min(value, max));
        }

        protected float Interpolate(float min, float max, float gradient)
        {
            return min + (max - min) * Clamp(gradient);
        }

        public void Dispose()
        {
            canvasGraphics.Dispose();
        }
    }
}
