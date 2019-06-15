namespace tokyo.RayTracing
{
    public class Plane : IGeometry
    {
        private readonly Vector _normal;

        private float _d;

        private readonly Vector _position;

        private readonly IMaterial _material;

        public Plane(Vector normal, float d, IMaterial material)
        {
            _normal = normal;
            _d = d;
            _material = material;
            _position = _normal * d;
        }

        public IMaterial Material()
        {
            return _material;
        }

        public Intersection Intersect(Ray ray)
        {
            float a = ray.Direction.Dot(_normal);
            if(a >= 0) return Intersection.NoHit;
            float b = _normal.Dot(ray.Pos - _position);
            var i = new Intersection();

            i.Geometry = this;
            i.Distance = -b / a;
            i.Position = ray.GetPoint(i.Distance);

            i.Normal = _normal;
            return i;

        }
    }
}
