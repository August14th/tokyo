using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tokyo.RayTracing
{
    public interface IGeometry
    {
        IMaterial Material();

        Intersection Intersect(Ray ray);
    }
}
