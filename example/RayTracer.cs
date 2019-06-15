using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using tokyo;
using tokyo.RayTracing;

namespace example
{
    class RayTracer
    {

        private const int Width = 768;

        private const int Height = 768;

        private Form form;

        private GraphicDevice device;

        public RayTracer()
        {
            form = new Form
            {
                Size = new Size(Width, Height),
                StartPosition = FormStartPosition.CenterScreen,
                Text = "tokyo"
            };
        }

        public void Run()
        {
            form.Show();
            Bitmap canvas = new Bitmap(Width, Height);
            device = new GraphicDevice(canvas);

            Scene scene = new Scene(new DirectionalLight { Direction = new Vector(1, 1, 1).Normalize(), Color = Color.White },
                new IGeometry[]{
                 new Sphere(new Vector(-10, 10, -10), 8, new PhongMaterial(Color.Red, Color.White, 8f, 0.25f)),
                 new Sphere(new Vector(10, 10, -10), 8, new PhongMaterial(Color.Blue, Color.White, 8f, 0.25f)),
                 new Plane(new Vector(0, 1, 0), 0, new CheckerMaterial(0.1f, 0.25f))
            });
            tokyo.RayTracing.Camera camera = new tokyo.RayTracing.Camera(new Vector(0, 5, 15), new Vector(0, 0, -1), new Vector(0, 1, 0), 90);

            device.RayTracingReflection(camera, scene, 2);

            using (var g = form.CreateGraphics())
            {
                g.DrawImage(canvas, Point.Empty);
            }
            while (!form.IsDisposed)
            {
                Application.DoEvents();
            }            
        }
    }
}
