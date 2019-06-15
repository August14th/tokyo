using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tokyo.RayTracing
{
    public class Sphere: IGeometry
    {
        private readonly Vector _center;

        private readonly float _sqrRadius;

        private readonly IMaterial _material;

        public Sphere(Vector center, float radius, IMaterial material)
        {
            _center = center;
            _sqrRadius = radius * radius;
            _material = material;
        }
        
        public Intersection Intersect(Ray ray)
        {
            Vector v = ray.Pos - _center;

            float diff = v.SqrLength - _sqrRadius;
            float dDotV = ray.Direction.Dot(v);

            if (dDotV <= 0)
            {
                float discr = dDotV * dDotV - diff;
                if (discr >= 0)
                {
                    Intersection i = new Intersection();
                    i.Geometry = this;
                    i.Distance = -dDotV - (float)Math.Sqrt(discr);
                    i.Position = ray.GetPoint(i.Distance);

                    i.Normal = (i.Position - _center).Normalize();

                    return i;
                }
            }

            return Intersection.NoHit;
        }

        public IMaterial Material()
        {
            return _material;
        }
    }
}
