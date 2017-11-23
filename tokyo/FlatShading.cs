using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tokyo
{
    class FlatShading : GraphicDevice
    {
        public FlatShading(Bitmap bitmap) : base(bitmap)
        {

        }

        public override void DrawTriangle(Vertex v1, Vertex v2, Vertex v3, Texture texture)
        {
            var a = v1.Pos - v2.Pos;
            var b = v3.Pos - v2.Pos;
            var n = a.Cross(b);
            var v = Camera.Target - Camera.Position;
            if (n.Dot(v) > 0) return;

            if (v1.Pos.Y > v2.Pos.Y)
            {
                var temp = v2;
                v2 = v1;
                v1 = temp;
            }

            if (v2.Pos.Y > v3.Pos.Y)
            {
                var temp = v2;
                v2 = v3;
                v3 = temp;
            }

            if (v1.Pos.Y > v2.Pos.Y)
            {
                var temp = v2;
                v2 = v1;
                v1 = temp;
            }

            float dP1P2, dP1P3;

            Vector p1 = v1.Pos;
            Vector p2 = v2.Pos;
            Vector p3 = v3.Pos;

            if (p2.Y - p1.Y > 0)
                dP1P2 = (p2.X - p1.X) / (p2.Y - p1.Y);
            else
                dP1P2 = 0;
            if (p3.Y - p1.Y > 0)
                dP1P3 = (p3.X - p1.X) / (p3.Y - p1.Y);
            else
                dP1P3 = 0;

            Vector facePos = (v1.Coord + v2.Coord + v3.Coord) / 3;
            Vector faceNormal = (v1.Normal + v2.Normal + v3.Normal) / 3;
            float nl = ComputeNDotL(facePos, faceNormal);

            ScanLineData data = new ScanLineData { };
            data.ndotl = nl;

            if (dP1P2 > dP1P3)
            {
                for (int y = (int)p1.Y; y <= p3.Y; y++)
                {
                    data.Y = y;
                    if (y < p2.Y)
                    {                        
                        ProcessScanLine(data, p1, p3, p1, p2);
                    }
                    else
                    {                        
                        ProcessScanLine(data, p1, p3, p2, p3);
                    }
                }
            }
            else
            {
                for (int y = (int)p1.Y; y <= p3.Y; y++)
                {
                    data.Y = y;
                    if (y < p2.Y)
                    {                        
                        ProcessScanLine(data, p1, p2, p1, p3);
                    }
                    else
                    {                        
                        ProcessScanLine(data, p2, p3, p1, p3);
                    }
                }
            }
        }

        private void ProcessScanLine(ScanLineData data, Vector pa, Vector pb, Vector pc, Vector pd)
        {
            var gradient1 = pa.Y != pb.Y ? (data.Y - pa.Y) / (pb.Y - pa.Y) : 1;
            var gradient2 = pc.Y != pd.Y ? (data.Y - pc.Y) / (pd.Y - pc.Y) : 1;

            int sx = (int)Interpolate(pa.X, pb.X, gradient1);
            int ex = (int)Interpolate(pc.X, pd.X, gradient2);

            float sz = Interpolate(pa.Z, pb.Z, gradient1);
            float ez = Interpolate(pc.Z, pd.Z, gradient2);

            for (int x = sx; x < ex; x++)
            {
                float gradient = (x - sx) / (float)(ex - sx);
                var z = Interpolate(sz, ez, gradient);

                Color color = BaseColor;
                float nl = data.ndotl;
                color = Color.FromArgb((int)(color.R * nl), (int)(color.G * nl), (int)(color.B * nl));
                DrawPoint(new Vector(x, data.Y, z), color);
            }
        }
    }
}
