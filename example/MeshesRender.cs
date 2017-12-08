using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using tokyo;
using tokyo.RayTracing;

namespace example
{
    class MeshesRender
    {
        private const int Width = 768;

        private const int Height = 768;

        private Form form;

        private GraphicBuffer buffer;

        Mesh[] Suzanne = new BabylonMeshLoader("babylon").Load("Suzanne");

        public MeshesRender()
        {
            form = new Form
            {
                Size = new Size(Width, Height),
                StartPosition = FormStartPosition.CenterScreen,
                Text = "tokyo"
            };

            // buffer = new GraphicBuffer(ShadingMode.WireFrame, Width, Height);
            // buffer = new GraphicBuffer(ShadingMode.Flat, Width, Height);
            // buffer = new GraphicBuffer(ShadingMode.Phong, Width, Height);
            buffer = new GraphicBuffer(ShadingMode.Texture, Width, Height);
        }

        public void Run()
        {
            form.Show();
            var stopwatch = new Stopwatch();
            var deltatime = TimeSpan.FromMilliseconds(1000.0 / 60);

            while (!form.IsDisposed)
            {
                stopwatch.Start();
                // 渲染到backbuffer
                Render(deltatime);
                // backbuffer、frontbuffer轮换
                buffer.SwapBuffers();
                // 显示frontbuffer
                Present();
                Application.DoEvents();
                stopwatch.Stop();
                deltatime = stopwatch.Elapsed;

                stopwatch.Reset();
            }
        }

        private void Present()
        {
            using (var g = form.CreateGraphics())
            {
                g.DrawImage(buffer.Current, Point.Empty);
            }
        }

        private void Render(TimeSpan dt)
        {
            var g = buffer.BackgroundGraphicDevice;
            g.Camera = new tokyo.Camera(new Vector(0, 0, 10), new Vector(0, 0, -10), Vector.UnitY, (float)Math.PI / 4, 0.1f, 1f); ;
            g.BaseColor = Color.White;
            g.Light = new PointLight { Pos = new Vector(0, 10, 10), Color = Color.White };
            Suzanne[0].Rotation += new Vector(0, 0.1f, 0);

            g.Clear(Color.Black);
            g.DrawMeshes(Suzanne);
            g.DrawString($"FPS: {1000.0 / dt.Milliseconds}", new Font(new FontFamily("Microsoft Yahei"), 14), Brushes.White, 0, 0);

        }
    }
}
