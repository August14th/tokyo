using System;
using System.Drawing;
using System.IO;

namespace tokyo
{
    public class Mesh
    {
        public Vector Position { get; set; } = Vector.Zero;

        public Vector Rotation { get; set; } = Vector.Zero;

        public Surface[] Surfaces { get; private set; }

        public Vertex[] Vertices { get; private set; }

        public string name { get; private set; }

        public Texture Texture { get; set; }

        public Mesh(string name, int verticesCount, int surfacesCount)
        {
            this.name = name;
            this.Vertices = new Vertex[verticesCount];
            this.Surfaces = new Surface[surfacesCount];
        }
    }

    public class Vertex
    {
        public Vector Pos;

        public Vector Coord;

        public Vector Normal;

        public float u;

        public float v;

    }

    public struct Surface
    {

        public Surface(int a, int b, int c)
        {
            A = a;
            B = b;
            C = c;
        }
        public int A;

        public int B;

        public int C;
    }

    public struct Material
    {
        public string Name;

        public string ID;

        public string DiffuseTextureName;
    }

    public class Texture
    {
        private Bitmap texture;

        public Texture(Bitmap texture)
        {
            this.texture = texture;
        }

        public int Height => texture.Height;

        public int Width => texture.Width;

        public Color Map(int u, int v)
        {
            if (texture == null) return Color.White;
            return texture.GetPixel(u, v);
        }
    }

}
