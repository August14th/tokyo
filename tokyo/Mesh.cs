namespace tokyo
{
    public class Mesh
    {
        public Vector Position { get; set; } = Vector.Zero;

        public Vector Rotation { get; set; } = Vector.Zero;

        public Surface[] Surfaces { get; private set; }

        public Vector[] Vertices { get; private set; }

        public string name { get; private set; }

        public static Mesh Cube()
        {
            Mesh cube = new Mesh("cube", 8, 12);

            cube.Vertices[0] = new Vector(1, 1, 1);
            cube.Vertices[1] = new Vector(1, 1, -1);
            cube.Vertices[2] = new Vector(1, -1, 1);
            cube.Vertices[3] = new Vector(1, -1, -1);
            cube.Vertices[4] = new Vector(-1, 1, 1);
            cube.Vertices[5] = new Vector(-1, 1, -1);
            cube.Vertices[6] = new Vector(-1, -1, 1);
            cube.Vertices[7] = new Vector(-1, -1, -1);

            cube.Surfaces[0] = new Surface(0, 1, 2);
            cube.Surfaces[1] = new Surface(3, 1, 2);
            cube.Surfaces[2] = new Surface(4, 5, 6);
            cube.Surfaces[3] = new Surface(7, 5, 6);
            cube.Surfaces[4] = new Surface(0, 1, 4);
            cube.Surfaces[5] = new Surface(5, 1, 4);
            cube.Surfaces[6] = new Surface(2, 3, 6);
            cube.Surfaces[7] = new Surface(7, 3, 6);
            cube.Surfaces[8] = new Surface(1, 3, 5);
            cube.Surfaces[9] = new Surface(7, 3, 5);
            cube.Surfaces[10] = new Surface(0, 4, 2);
            cube.Surfaces[11] = new Surface(6, 4, 2);

            // cube.Rotation = new Vector(1, 1, 1);
            return cube;
        }

        public Mesh(string name, int verticesCount, int surfacesCount)
        {
            this.name = name;
            this.Vertices = new Vector[verticesCount];
            this.Surfaces = new Surface[surfacesCount];
        }
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
