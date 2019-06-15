using System;

namespace tokyo.RayTracing
{
    public class Camera
    {      

        private readonly Vector _position;

        private readonly Vector _forward; // z轴

        private readonly Vector _up; // y轴

        private readonly Vector _right; // x轴

        private readonly float _fovScale;

        public Camera(Vector position, Vector forward, Vector up, float fov)
        {
            // 左手坐标系
            _position = position;
            _forward = forward.Normalize();
            _right = up.Cross(_forward).Normalize();
            _up = _forward.Cross(_right).Normalize();
                        
            _fovScale = (float)Math.Tan(fov * 0.5 * Math.PI / 180) * 2;         
        }


        public Ray GenerateRay(float x, float y)
        {            
            Vector r = _right * x * _fovScale;
            Vector u = _up * y * _fovScale;
            return new Ray(_position, (_forward + r + u).Normalize());
        }
    }
}
