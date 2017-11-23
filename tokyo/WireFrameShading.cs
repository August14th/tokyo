using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tokyo
{
    class WireFrameShading : GraphicDevice
    {
        public WireFrameShading(Bitmap bitmap) : base(bitmap)
        {

        }

        public override void DrawTriangle(Vertex v1, Vertex v2, Vertex v3, Texture texture)
        {
            DrawLine(v1.Pos, v2.Pos);
            DrawLine(v2.Pos, v3.Pos);
            DrawLine(v3.Pos, v1.Pos);
        }
    }
}
