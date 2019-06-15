using System;
using System.Drawing;

namespace tokyo.RayTracing
{
    public class CheckerMaterial : IMaterial
    {
        private readonly float _scale;

        private readonly float _reflectiveness;

        public CheckerMaterial(float scale, float reflectiveness)
        {
            _scale = scale;
            _reflectiveness = reflectiveness;
        }
        Color IMaterial.Sample(Ray ray, Vector position, Vector normal, DirectionalLight light)
        {
            return Math.Abs((Math.Floor(position.X * _scale) + Math.Floor(position.Z * _scale)) % 2) < 1 ? Color.Black : Color.White;
        }

        float IMaterial.Reflectiveness()
        {
            return _reflectiveness;
        }
    }
}
