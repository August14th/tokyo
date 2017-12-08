using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tokyo.RayTracing
{
    public class Intersection
    {
        public Geometry Gemoemtry { get; set;}

        public float Distance { get; set; }

        public Vector Position { get; set; }

        public Vector Normal { get; set; }

        public static Intersection NoHit = new Intersection();

    }
}
