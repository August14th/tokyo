using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using tokyo;

namespace example
{
    class DrawLines : Canvas
    {
        public static int Width = 512;

        public static int Height = 512;

        public DrawLines():base(Width, Height)
        {
            
        }

        protected override void Draw()
        {            
            DrawLineBresenham();
            // DrawLineSampling();
            // DrawLineSuperSampling();
            // DrawLineSDF();
            // DrawLineSDFAABB();
        }

        private void DrawLineBresenham()
        {
            device.BaseColor = Color.Black;
            device.Clear(Color.White);
            float cx = Width * 0.5f - 0.5f, cy = Height * 0.5f - 0.5f;
            for (int j = 0; j < 5; j++)
            {
                float r1 = Math.Min(Width, Height) * (j + 0.5f) * 0.085f;
                float r2 = Math.Min(Width, Height) * (j + 1.5f) * 0.085f;

                float t = j * (float)Math.PI / 64f;
                for (int i = 1; i <= 64; i++, t += 2.0f * (float)Math.PI / 64f)
                {
                    float ct = (float)Math.Cos(t), st = (float)Math.Sin(t);
                    device.DrawLine(new Point((int)(cx + r1 * ct), (int)(cy - r1 * st)), new Point((int)(cx + r2 * ct), (int)(cy - r2 * st)));
                }
            }
        }

        private void DrawLineSampling()
        {
            device.Clear(Color.White);
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (Sample(x, y))
                    {
                        device.DrawPoint(new Point(x, y), Color.Black);
                    }
                }
            }
        }

        private void DrawLineSuperSampling()
        {
            device.Clear(Color.White);
            int sw = 5, sh = 5;
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    float sum = 0;
                    for (int j = -sh / 2; j <= sh / 2; j++)
                    {
                        for (int i = -sw / 2; i <= sw / 2; i++)
                        {
                            sum += Sample(x + (float)i / sw, y + (float)j / sh) ? 1 : 0;
                            device.DrawPoint(new Point(x, y), Color.White.Multiply(1 - sum / (sw * sh)));
                        }
                    }
                }
            }
        }

        private void DrawLineSDF()
        {
            device.Clear(Color.White);
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {

                    device.DrawPoint(new Point(x, y), Color.White.Multiply(1 - SampleSDF(x, y)));
                }
            }
        }

        private void DrawLineSDFAABB()
        {
            device.Clear(Color.White);
            float cx = Width * 0.5f - 0.5f, cy = Height * 0.5f - 0.5f;
            for (int j = 0; j < 5; j++)
            {
                float r1 = Math.Min(Width, Height) * (j + 0.5f) * 0.085f;
                float r2 = Math.Min(Width, Height) * (j + 1.5f) * 0.085f;

                float t = j * (float)Math.PI / 64f, r = (j + 1) * 0.5f;
                for (int i = 1; i <= 64; i++, t += 2.0f * (float)Math.PI / 64f)
                {
                    float ct = (float)Math.Cos(t), st = (float)Math.Sin(t);
                    device.lineSDFAABB(cx + r1 * ct, cy - r1 * st, cx + r2 * ct, cy - r2 * st, r);
                }
            }
        }

        private bool Sample(float x, float y)
        {
            float cx = Width * 0.5f, cy = Height * 0.5f;
            for (int j = 0; j < 5; j++)
            {
                float r1 = Math.Max(Width, Height) * (j + 0.5f) * 0.085f;
                float r2 = Math.Max(Width, Height) * (j + 1.5f) * 0.085f;
                float t = j * (float)Math.PI / 64.0f, r = (j + 1) * 0.5f;
                for (int i = 1; i <= 64; i++, t += 2.0f * (float)Math.PI / 64.0f)
                {
                    float ct = (float)Math.Cos(t), st = (float)Math.Sin(t);
                    if (device.IsInCapsule(x, y, cx + r1 * ct, cy - r1 * st, cx + r2 * ct, cy - r2 * st, r))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private float SampleSDF(float x, float y)
        {
            float s = 0.0f, cx = Width * 0.5f, cy = Height * 0.5f;
            for (int j = 0; j < 5; j++)
            {
                float r1 = Math.Max(Width, Height) * (j + 0.5f) * 0.085f;
                float r2 = Math.Max(Width, Height) * (j + 1.5f) * 0.085f;
                float t = j * (float)Math.PI / 64.0f, r = (j + 1) * 0.5f;
                for (int i = 1; i <= 64; i++, t += 2.0f * (float)Math.PI / 64.0f)
                {
                    float ct = (float)Math.Cos(t), st = (float)Math.Sin(t);
                    s = Math.Max((float)Math.Min(0.5 - device.CapsuleSDF(x, y, cx + r1 * ct, cy - r1 * st, cx + r2 * ct, cy - r2 * st, r), 1), s);
                }
            }
            return s;
        }        
    }
}
