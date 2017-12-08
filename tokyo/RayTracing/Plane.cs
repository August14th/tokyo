using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tokyo.RayTracing
{
    public class Plane : Geometry
    {
        private Vector normal;

        private float d;

        private Vector position;

        private IMaterial material;

        public Plane(Vector normal, float d, IMaterial material)
        {
            this.normal = normal;
            this.d = d;
            this.material = material;
            this.position = this.normal * d;
        }

        public IMaterial Materail()
        {
            return material;
        }

        public Intersection Intersect(Ray ray)
        {
            float a = ray.Direction.Dot(this.normal);

            if(a >= 0)
            {
                return Intersection.NoHit;
            }

            float b = this.normal.Dot(ray.Pos - this.position);
            var i = new Intersection();

            i.Gemoemtry = this;
            i.Distance = -b / a;
            i.Position = ray.getPoint(i.Distance);

            i.Normal = this.normal;
            return i;

        }
    }
}
