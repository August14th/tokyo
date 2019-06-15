using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tokyo.RayTracing;

namespace tokyo.RayTracing
{
    public class Scene
    {
        public readonly DirectionalLight Light;

        private readonly IGeometry[] _items;

        public Scene(DirectionalLight light, IGeometry[] items)
        {
            Light = light;
            _items = items;
        }

        public Intersection Intersect(Ray ray)
        {
            float minDistance = float.MaxValue;
            Intersection minResult = Intersection.NoHit;
            foreach (IGeometry item in _items)
            {
                Intersection i = item.Intersect(ray);
                if (i != Intersection.NoHit && i.Distance < minDistance)
                {
                    minDistance = i.Distance;
                    minResult = i;
                }
            }
            return minResult;
        }
    }
}
