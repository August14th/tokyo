using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tokyo.RayTracing
{
    public class Ray
    {
        public readonly Vector Pos;

        public readonly Vector Direction;

        public Ray(Vector pos, Vector direction)
        {
            Pos = pos;

            Direction = direction.Normalize();
        }

        public Vector GetPoint(float distance)
        {
            return Pos + Direction * distance;
        }
    }
}
