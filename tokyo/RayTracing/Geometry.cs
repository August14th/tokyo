using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tokyo.RayTracing
{
    public interface Geometry
    {
        IMaterial Materail();

        Intersection Intersect(Ray ray);
    }
}
