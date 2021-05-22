using System.Numerics;

namespace Dck.Pathfinder.Primitives
{
    public static class Vector2Extensions
    {
        public static Vector2 Truncate(Vector2 original, float max)
        {
            if (!(original.Length() > max)) return original;
            var v2 = Vector2.Normalize(original);
            v2 *= max;
            return v2;
        }
    }
}