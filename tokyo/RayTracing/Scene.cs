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

        private Geometry[] items;

        public Scene(DirectionalLight light, Geometry[] items)
        {
            this.Light = light;
            this.items = items;
        }

        public Intersection Intersect(Ray ray)
        {
            float minDistance = float.MaxValue;
            Intersection minInterset = Intersection.NoHit;
            foreach (Geometry item in items)
            {
                Intersection i = item.Intersect(ray);
                if (i != Intersection.NoHit && i.Distance < minDistance)
                {
                    minDistance = i.Distance;
                    minInterset = i;
                }
            }

            return minInterset;
        }
    }
}
