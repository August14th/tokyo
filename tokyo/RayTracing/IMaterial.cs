using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using tokyo.RayTracing;

namespace tokyo.RayTracing
{
    public interface IMaterial
    {
        float Reflectiveness();

        Color Sample(Ray ray, Vector position, Vector normal, DirectionalLight light);
    }
}
