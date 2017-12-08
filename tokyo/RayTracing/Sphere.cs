using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tokyo.RayTracing
{
    public class Sphere: Geometry
    {
        private Vector Center;

        private float Radius;

        private float SqrRadius;

        private IMaterial Material;

        public Sphere(Vector center, float radius, IMaterial material)
        {
            Center = center;
            Radius = radius;

            SqrRadius = Radius * Radius;
            Material = material;
        }
        
        public Intersection Intersect(Ray ray)
        {
            Vector v = ray.Pos - Center;

            float diff = v.SqrLength - SqrRadius;
            float DdotV = ray.Direction.Dot(v);

            if (DdotV <= 0)
            {
                float discr = DdotV * DdotV - diff;
                if (discr >= 0)
                {
                    Intersection i = new Intersection();
                    i.Gemoemtry = this;
                    i.Distance = -DdotV - (float)Math.Sqrt(discr);
                    i.Position = ray.getPoint(i.Distance);

                    i.Normal = (i.Position - Center).Normalize();

                    return i;
                }
            }

            return Intersection.NoHit;
        }

        public IMaterial Materail()
        {
            return Material;
        }
    }
}
