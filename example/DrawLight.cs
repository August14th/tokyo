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

        public delegate Result Scene(float x, float y);

        public DrawLight() : base(Width, Height)
        {

        }

        protected override void Draw()
        {
            // 圆月
            Scene scene = FullMoon;
            // 月食
            // Scene scene = HalfMoon;
            Draw(scene);
        }

        private void Draw(Scene scene)
        {
            device.Clear(Color.White);
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    int color = Math.Min((int)(MonteCarloSample((float)x / Width, (float)y / Height, scene) * 255), 255);
                    device.DrawPoint(new Point(x, y), Color.FromArgb(color, color, color));
                }
            }
        }

        private float MonteCarloSample(float x, float y, Scene scene)
        {
            int N = 64;
            float sum = 0.0f;
            for (int i = 0; i < N; i++)
            {
                // float angle = (float)(2 * Math.PI * rand.NextDouble());      // 均匀采样
                // float angle = (float)(2 * Math.PI * i / N);                  // 分层采样
                float angle = (float)(2 * Math.PI * (i + (float)rand.NextDouble()) / N); // 抖动采样
                sum += trace(x, y, (float)Math.Cos(angle), (float)Math.Sin(angle), scene);
            }
            return sum / N;
        }

        private float trace(float ox, float oy, float dx, float dy, Scene scene)
        {
            float t = 0.0f;
            for (int i = 0; i < 64 && t < 2.0f; i++)
            {
                Result r = scene(ox + dx * t, oy + dy * t);
                if (r.sd < 1e-6f)
                {
                    return r.emissive;
                }
                t += r.sd;
            }
            return 0.0f;
        }

        Result FullMoon(float x, float y)
        {
            return new Result() { sd = device.CircleSDF(x, y, 0.4f, 0.5f, 0.20f), emissive = 1.0f };
        }

        // 月食
        Result HalfMoon(float x, float y)
        {
            Result r1 = new Result() { sd = device.CircleSDF(x, y, 0.4f, 0.5f, 0.20f), emissive = 1.0f };
            Result r2 = new Result() { sd = device.CircleSDF(x, y, 0.6f, 0.5f, 0.20f), emissive = 0.8f };
            return subtractOp(r1, r2);
        }

        Result unionOp(Result a, Result b)
        {
            return a.sd < b.sd ? a : b;
        }

        Result intersectOp(Result a, Result b)
        {
            Result r = a.sd > b.sd ? b : a;
            r.sd = a.sd > b.sd ? a.sd : b.sd;
            return r;
        }

        Result subtractOp(Result a, Result b)
        {
            Result r = a;
            r.sd = (a.sd > -b.sd) ? a.sd : -b.sd;
            return r;
        }

    }

    struct Result
    {
        public float sd, emissive;
    }
}
