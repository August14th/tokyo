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
        private readonly Bitmap _canvas;

        private readonly Graphics _canvasGraphics;

        private int Height => _canvas.Height;

        private int Width => _canvas.Width;

        public GraphicDevice(Bitmap bitmap)
        {
            _canvas = bitmap;
            _canvasGraphics = Graphics.FromImage(_canvas);
        }

        public void RayTracing(Camera camera, Scene scene)
        {
            for (int py = 0; py < Height; py++)
            {
                float sy = 0.5f - (float)py / Height;
                for (int px = 0; px < Width; px++)
                {
                    float sx = (float)px / Width - 0.5f;
                    Ray ray = camera.GenerateRay(sx, sy);
                    Intersection i = scene.Intersect(ray);
                    if (i.Geometry != null)
                    {
                        Color color = i.Geometry.Material().Sample(ray, i.Position, i.Normal, scene.Light);
                        _canvas.SetPixel(px, py, color);
                    }
                }
            }
        }

        private Color RayTraceRecursive(Scene scene, Ray ray, int maxReflect)
        {
            Intersection i = scene.Intersect(ray);
            if (i != Intersection.NoHit)
            {
                float reflectiveness = i.Geometry.Material().Reflectiveness();
                Color color = i.Geometry.Material().Sample(ray, i.Position, i.Normal, scene.Light);
                if (reflectiveness > 0 && maxReflect > 0)
                {
                    Vector reflectDirection = ray.Direction + i.Normal * -2 * i.Normal.Dot(ray.Direction);
                    Ray reflectRay = new Ray(i.Position, reflectDirection);
                    Color reflectColor = RayTraceRecursive(scene, reflectRay, maxReflect - 1);

                    color = color.Multiply(1 - reflectiveness).Add(reflectColor.Multiply(reflectiveness));
                }
                return color;
            }
            return Color.Black;
        }

        public void RayTracingReflection(Camera camera, Scene scene, int maxReflect)
        {
            for (int py = 0; py < Height; py++)
            {
                float sy = 0.5f - (float)py / Height;
                for (int px = 0; px < Width; px++)
                {
                    float sx = (float)px / Width - 0.5f;
                    Ray ray = camera.GenerateRay(sx, sy);

                    Color color = RayTraceRecursive(scene, ray, maxReflect);
                    _canvas.SetPixel(px, py, color);
                }
            }
        }

        public void Dispose()
        {
            _canvasGraphics.Dispose();
        }
    }
}
