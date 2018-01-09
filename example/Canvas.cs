using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using tokyo;

namespace example
{
    public abstract class Canvas
    {
        private Form form;

        protected GraphicDevice2D device;

        private Bitmap canvas;

        protected abstract void Draw();

        public Canvas(int width, int height)
        {
            form = new Form
            {
                Size = new Size(width, height),
                StartPosition = FormStartPosition.CenterScreen,
                Text = "tokyo"
            };
            canvas = new Bitmap(width, height);
            device = new GraphicDevice2D(canvas);
        }

        public void Run()
        {
            form.Show();
            Draw();
            while (!form.IsDisposed)
            {
                using (var g = form.CreateGraphics())
                {
                    g.DrawImage(canvas, Point.Empty);
                }
                Application.DoEvents();
            }
        }
    }
}
