namespace tokyo
{
    public class Mesh
    {
        public Vector Position { get; set; } = Vector.Zero;

        public Vector Rotation { get; set; } = Vector.Zero;

        public Surface[] Surfaces { get; private set; }

        public Vertex[] Vertices { get; private set; }

        public string name { get; private set; }

        public Mesh(string name, int verticesCount, int surfacesCount)
        {
            this.name = name;
            this.Vertices = new Vertex[verticesCount];
            this.Surfaces = new Surface[surfacesCount];
        }
    }

    public struct Vertex
    {
        public Vector Pos;

        public Vector Coord;

        public Vector Normal;       
        
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

}
