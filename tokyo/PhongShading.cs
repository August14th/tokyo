﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tokyo
{
    class PhongShading : GraphicDevice3D
    {
        public PhongShading(Bitmap bitmap) : base(bitmap)
        {
        }

        public override void DrawTriangle(Vertex v1, Vertex v2, Vertex v3, Texture texture)
        {
            var a = v1.Pos - v2.Pos;
            var b = v3.Pos - v2.Pos;
            var n = a.Cross(b);
            var v = Camera.Forward;
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
            {
                if (p2.X > p1.X) dP1P2 = float.MaxValue; else dP1P2 = float.MinValue;
            }

            if (p3.Y - p1.Y > 0)
                dP1P3 = (p3.X - p1.X) / (p3.Y - p1.Y);
            else
            {
                if (p3.X > p1.X) dP1P3 = float.MaxValue; else dP1P3 = float.MinValue;
            }

            float nl1 = ComputeNDotL(v1.Coord, v1.Normal);
            float nl2 = ComputeNDotL(v2.Coord, v2.Normal);
            float nl3 = ComputeNDotL(v3.Coord, v3.Normal);

            ScanLineData data = new ScanLineData { };

            if (dP1P2 > dP1P3)
            {
                for (int y = (int)p1.Y; y <= p3.Y; y++)
                {
                    data.Y = y;
                    if (y < p2.Y)
                    {
                        data.ndotla = nl1;
                        data.ndotlb = nl3;
                        data.ndotlc = nl1;
                        data.ndotld = nl2;

                        ProcessScanLine(data, p1, p3, p1, p2, texture);
                    }
                    else
                    {
                        data.ndotla = nl1;
                        data.ndotlb = nl3;
                        data.ndotlc = nl2;
                        data.ndotld = nl3;

                        ProcessScanLine(data, p1, p3, p2, p3, texture);
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
                        data.ndotla = nl1;
                        data.ndotlb = nl2;
                        data.ndotlc = nl1;
                        data.ndotld = nl3;
                        
                        ProcessScanLine(data, p1, p2, p1, p3, texture);
                    }
                    else
                    {
                        data.ndotla = nl2;
                        data.ndotlb = nl3;
                        data.ndotlc = nl1;
                        data.ndotld = nl3;
                        
                        ProcessScanLine(data, p2, p3, p1, p3, texture);
                    }
                }
            }
        }

        private void ProcessScanLine(ScanLineData data, Vector pa, Vector pb, Vector pc, Vector pd, Texture texture)
        {
            var gradient1 = pa.Y != pb.Y ? (data.Y - pa.Y) / (pb.Y - pa.Y) : 1;
            var gradient2 = pc.Y != pd.Y ? (data.Y - pc.Y) / (pd.Y - pc.Y) : 1;

            int sx = (int)Interpolate(pa.X, pb.X, gradient1);
            int ex = (int)Interpolate(pc.X, pd.X, gradient2);

            float sz = Interpolate(pa.Z, pb.Z, gradient1);
            float ez = Interpolate(pc.Z, pd.Z, gradient2);

            float snl = Interpolate(data.ndotla, data.ndotlb, gradient1);
            float enl = Interpolate(data.ndotlc, data.ndotld, gradient2);

            for (int x = sx; x < ex; x++)
            {
                float gradient = (x - sx) / (float)(ex - sx);
                var z = Interpolate(sz, ez, gradient);
                
                float nl = Interpolate(snl, enl, gradient);
                Color color = Color.FromArgb((int)(BaseColor.R * nl), (int)(BaseColor.G * nl), (int)(BaseColor.B * nl));
                DrawPoint(new Vector(x, data.Y, z), color);
            }
        }

        class ScanLineData
        {
            public float ndotla;

            public float ndotlb;

            public float ndotlc;

            public float ndotld;

            public float Y;
        }
    }
}
