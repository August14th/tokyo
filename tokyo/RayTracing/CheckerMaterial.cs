using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tokyo.RayTracing
{
    public class CheckerMaterial : IMaterial
    {
        private float scale;

        private float reflectiveness;

        public CheckerMaterial(float scale, float reflectiveness)
        {
            this.scale = scale;
            this.reflectiveness = reflectiveness;
        }
        Color IMaterial.Sample(Ray ray, Vector position, Vector normal, DirectionalLight light)
        {
            return Math.Abs((Math.Floor(position.X * scale) + Math.Floor(position.Z * scale)) % 2) < 1 ? Color.Black : Color.White;
        }

        float IMaterial.Reflectiveness()
        {
            return reflectiveness;
        }
    }
}
