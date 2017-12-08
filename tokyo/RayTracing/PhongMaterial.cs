using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tokyo.RayTracing
{
    public class PhongMaterial : IMaterial
    {
        private Color diffuse;
        private Color specular;
        private float shininess;
        private float reflectiveness;

        public PhongMaterial(Color diffuse, Color specular, float shininess, float reflectiveness)
        {
            this.diffuse = diffuse;
            this.specular = specular;
            this.shininess = shininess;
            this.reflectiveness = reflectiveness;
        }

        public Color Sample(Ray ray, Vector position, Vector normal, DirectionalLight light)
        {            
            float NdotL = normal.Dot(light.Direction);

            Vector H = (light.Direction - ray.Direction).Normalize();
            float NdotH = normal.Dot(H);

            Color diffuseTerm = diffuse.Multiply(Math.Max(NdotL, 0));
            Color specularTerm = specular.Multiply((float)(Math.Pow(Math.Max(NdotH, 0), shininess)));

            Color term = diffuseTerm.Add(specularTerm);
            return light.Color.Multiply(term);
        }

        float IMaterial.Reflectiveness()
        {
            return reflectiveness;
        }
    }
}
