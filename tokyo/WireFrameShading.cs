using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tokyo
{
    class WireFrameShading : GraphicDevice3D
    {
        public WireFrameShading(Bitmap bitmap) : base(bitmap)
        {

        }

        public override void DrawTriangle(Vertex v1, Vertex v2, Vertex v3, Texture texture)
        {
            DrawLine(new Point((int)v1.Pos.X, (int)v1.Pos.Y), new Point((int)v2.Pos.X, (int)v2.Pos.Y));
            DrawLine(new Point((int)v2.Pos.X, (int)v2.Pos.Y), new Point((int)v3.Pos.X, (int)v3.Pos.Y));
            DrawLine(new Point((int)v3.Pos.X, (int)v3.Pos.Y), new Point((int)v1.Pos.X, (int)v1.Pos.Y));
        }
    }
}
