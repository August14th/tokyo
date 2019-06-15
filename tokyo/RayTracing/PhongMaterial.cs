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
        private readonly Color _diffuse;
        private readonly Color _specular;
        private readonly float _shininess;
        private readonly float _reflectiveness;

        public PhongMaterial(Color diffuse, Color specular, float shininess, float reflectiveness)
        {
            _diffuse = diffuse;
            _specular = specular;
            _shininess = shininess;
            _reflectiveness = reflectiveness;
        }

        public Color Sample(Ray ray, Vector position, Vector normal, DirectionalLight light)
        {            
            float nDotL = normal.Dot(light.Direction); // diffuse部分，和视角方向无关

            Vector H = (light.Direction - ray.Direction).Normalize(); 
            float nDotH = normal.Dot(H); // specular部分，blinn模型

            Color diffuseTerm = _diffuse.Multiply(Math.Max(nDotL, 0));
            Color specularTerm = _specular.Multiply((float)Math.Pow(Math.Max(nDotH, 0), _shininess));

            Color term = diffuseTerm.Add(specularTerm);
            return light.Color.Multiply(term);
        }

        float IMaterial.Reflectiveness()
        {
            return _reflectiveness;
        }
    }
}
