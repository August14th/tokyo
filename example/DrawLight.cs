using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace example
{
    class DrawLight : Canvas
    {
        private Random rand = new Random();

        public static int Width = 512;

        public static int Height = 512;

        public DrawLight() : base(Width, Height)
        {

        }

        protected override void Draw()
        {
            DrawBaseLight();
        }

        private void DrawBaseLight()
        {
            device.Clear(Color.White);
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    int color = Math.Min((int)(MonteCarloSample((float)x / Width, (float)y / Height) * 255), 255);
                    device.DrawPoint(new Point(x, y), Color.FromArgb(color, color, color));
                }
            }
        }

        private float MonteCarloSample(float x, float y)
        {
            int N = 128;
            float sum = 0.0f;
            for (int i = 0; i < N; i++)
            {
                // float angle = (float)(2 * Math.PI * rand.NextDouble());      // 均匀采样
                // float angle = (float)(2 * Math.PI * i / N);                  // 分层采样
                float angle = (float)(2 * Math.PI * (i + (float)rand.NextDouble()) / N); // 抖动采样
                sum += trace(x, y, (float)Math.Cos(angle), (float)Math.Sin(angle));
            }
            return sum / N;
        }

        private float trace(float ox, float oy, float dx, float dy)
        {
            float t = 0.0f;
            for (int i = 0; i < 10 && t < 2.0f; i++)
            {
                float sd = device.CircleSDF(ox + dx * t, oy + dy * t, 0.5f, 0.5f, 0.1f);
                if (sd < 1e-6f)
                {
                    return 2.0f;
                }
                t += sd;
            }
            return 0.0f;
        }
    }
}
