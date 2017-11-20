using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tokyo
{
    public class Camera
    {
        public Vector Position { get; set; }

        public Vector Target { get; set; }

        public float Fov { get; set; }

        public float ZNear { get; set; }

        public float ZFar { get; set; }
    }
}
