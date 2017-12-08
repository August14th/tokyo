using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tokyo.RayTracing
{
    public class GraphicDevice : IDisposable
    {
        private readonly Bitmap canvas;

        private readonly Graphics canvasGraphics;

        private int Height => canvas.Height;

        private int Width => canvas.Width;

        public GraphicDevice(Bitmap bitmap)
        {
            canvas = bitmap;
            canvasGraphics = Graphics.FromImage(canvas);
        }

        public void RayTracing(Camera camera, Scene scene)
        {
            for (int py = 0; py < Height; py++)
            {
                float sy = 0.5f - (float)py / Height;
                for (int px = 0; px < Width; px++)
                {
                    float sx = (float)px / Width - 0.5f;
                    Ray ray = camera.generateRay(sx, sy);
                    Intersection i = scene.Intersect(ray);
                    if (i.Gemoemtry != null)
                    {
                        Color color = i.Gemoemtry.Materail().Sample(ray, i.Position, i.Normal, scene.Light);
                        canvas.SetPixel(px, py, color);
                    }
                }
            }
        }

        private Color RayTraceRecursive(Scene scene, Ray ray, int maxReflect)
        {
            Intersection i = scene.Intersect(ray);

            if (i != Intersection.NoHit)
            {
                float reflectiveness = i.Gemoemtry.Materail().Reflectiveness();
                Color color = i.Gemoemtry.Materail().Sample(ray, i.Position, i.Normal, scene.Light);
                if (reflectiveness > 0 && maxReflect > 0)
                {
                    Vector reflectDirection = ray.Direction + (i.Normal * -2 * i.Normal.Dot(ray.Direction));
                    Ray reflectRay = new Ray(i.Position, reflectDirection);
                    Color reflectColor = RayTraceRecursive(scene, reflectRay, maxReflect - 1);

                    color = color.Multiply(1 - reflectiveness).Add(reflectColor.Multiply(reflectiveness));
                }
                return color;
            }
            else
            {
                return Color.Black;
            }
        }

        public void RayTracingReflection(Camera camera, Scene scene, int maxReflect)
        {
            for (int py = 0; py < Height; py++)
            {
                float sy = 0.5f - (float)py / Height;
                for (int px = 0; px < Width; px++)
                {
                    float sx = (float)px / Width - 0.5f;
                    Ray ray = camera.generateRay(sx, sy);

                    Color color = RayTraceRecursive(scene, ray, maxReflect);
                    canvas.SetPixel(px, py, color);
                }
            }
        }

        public void Dispose()
        {
            canvasGraphics.Dispose();
        }
    }
}
