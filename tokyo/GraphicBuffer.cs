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

        public GraphicBuffer(ShadingMode renderMode, int width, int height)
        {
            Current = new Bitmap(width, height);
            Background = new Bitmap(width, height);
            switch (renderMode)
            {
                case ShadingMode.WireFrame:
                    CurrentGraphicDevice = new WireFrameShading(Current);
                    BackgroundGraphicDevice = new WireFrameShading(Background);
                    break;
                case ShadingMode.Flat:
                    CurrentGraphicDevice = new FlatShading(Current);
                    BackgroundGraphicDevice = new FlatShading(Background);
                    break;
                case ShadingMode.Phong:
                    CurrentGraphicDevice = new PhongShading(Current);
                    BackgroundGraphicDevice = new PhongShading(Background);
                    break;
                case ShadingMode.Texture:
                    CurrentGraphicDevice = new TextureShading(Current);
                    BackgroundGraphicDevice = new TextureShading(Background);
                    break;
            }
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
