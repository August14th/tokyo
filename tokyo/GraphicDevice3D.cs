using System;
using System.Drawing;

namespace tokyo
{
    public abstract class GraphicDevice3D: GraphicDevice2D
    {

        public Camera Camera
        {
            get; set;
        }

        public PointLight Light
        {
            get; set;
        }

        public GraphicDevice3D(Bitmap bitmap):base(bitmap)
        {
           
        }

        public void DrawMeshes(Mesh[] meshes)
        {
            var view = Matrix.LookAtLH(Camera.Position, Camera.Forward, Camera.Up);
            var projection = Matrix.PerspectiveFovLH(Camera.Fov, (float)Width / Height, Camera.ZNear, Camera.ZFar);

            foreach (Mesh mesh in meshes)
            {
                Matrix rotation = Matrix.Rotation(mesh.Rotation);
                Matrix translation = Matrix.Translation(mesh.Position);

                Matrix world = rotation * translation;
                Matrix transform = world * view * projection;

                for (int i = 0; i < mesh.Surfaces.Length; i++)
                {
                    var face = mesh.Surfaces[i];
                    Vertex v1 = Project(mesh.Vertices[face.A], transform, world);
                    Vertex v2 = Project(mesh.Vertices[face.B], transform, world);
                    Vertex v3 = Project(mesh.Vertices[face.C], transform, world);

                    DrawTriangle(v1, v2, v3, mesh.Texture);
                }
            }
        }

        public abstract void DrawTriangle(Vertex v1, Vertex v2, Vertex v3, Texture texture);

        public void DrawPoint(Vector point, Color color)
        {
            int px = (int)point.X;
            int py = (int)point.Y;

            if (px >= 0 && px < Width && py >= 0 && py < Height)
            {
                var index = py * Width + px;
                if (zBuffer[index] < point.Z)
                {
                    return;
                }
                zBuffer[index] = point.Z;
                canvas.SetPixel(px, py, color);
            }
        }

        private Vertex Project(Vertex src, Matrix transforom, Matrix world)
        {
            Vector pos = transforom.Transform(src.Coord);

            pos.X = pos.X * Width + Width / 2f;
            pos.Y = -pos.Y * Height + Height / 2f;

            return new Vertex
            {
                Pos = pos,
                Coord = world.Transform(src.Coord),
                Normal = world.Transform(src.Normal),
                u = src.u,
                v = src.v
            };
        }

        protected float ComputeNDotL(Vector vertex, Vector normal)
        {
            Vector lightDirection = (((PointLight)Light).Pos - vertex).Normalize();
            return Math.Max(0, lightDirection.Dot(normal.Normalize()));
        }
    }
}
