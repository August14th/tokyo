using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tokyo.RayTracing
{
    public class Camera
    {      

        private Vector position;

        private Vector forward;

        private Vector up;

        private Vector right;

        private float fovScale;

        public Camera(Vector position, Vector forward, Vector up, float fov)
        {
            // 左手坐标系
            this.position = position;
            this.forward = forward.Normalize();
            this.right = up.Cross(this.forward).Normalize();
            this.up = this.forward.Cross(right).Normalize();
                        
            fovScale = (float)Math.Tan(fov * 0.5 * Math.PI / 180) * 2;         
        }


        public Ray generateRay(float x, float y)
        {            
            Vector r = right * x * fovScale;
            Vector u = up * y * fovScale;
            return new Ray(position, (forward + r + u).Normalize());
        }
    }
}
