using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tokyo
{
    public class GraphicBuffer
    {
        public Bitmap Current { get; private set; }

        public GraphicDevice CurrentGraphicDevice { get; private set; }

        public Bitmap Background { get; private set; }

        public GraphicDevice BackgroundGraphicDevice { get; private set; }

        public GraphicBuffer(int width, int height)
        {
            Current = new Bitmap(width, height);

            CurrentGraphicDevice = new GraphicDevice(Current);

            Background = new Bitmap(width, height);

            BackgroundGraphicDevice = new GraphicDevice(Background);
        }

        public void SwapBuffers()
        {
            var t = Current;
            Current = Background;
            Background = t;

            var g = CurrentGraphicDevice;
            CurrentGraphicDevice = BackgroundGraphicDevice;
            BackgroundGraphicDevice = g;
        }

    }
}
