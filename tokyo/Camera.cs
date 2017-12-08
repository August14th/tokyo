using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tokyo
{
    public class Camera
    {      

        public Vector Position;

        public readonly Vector Forward;

        public readonly Vector Up;

        public readonly Vector Right;

        public readonly float Fov;

        public readonly float ZNear;

        public readonly float ZFar;

        private float fovScale;

        public Camera(Vector position, Vector forward, Vector up, float fov, float zNear, float zFar)
        {
            // 左手坐标系
            Position = position;
            Forward = forward.Normalize();
            Right = up.Cross(Forward).Normalize();
            Up = Forward.Cross(Right).Normalize();
                        
            Fov = fov;
            fovScale = (float)Math.Tan(Fov * 0.5 * Math.PI / 180) * 2;

            ZNear = zNear;
            ZFar = zFar;
        }
    }
}
