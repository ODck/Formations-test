using System.Numerics;
using Dck.Pathfinder.Primitives;
using Vector3 = UnityEngine.Vector3;

namespace UnityLibrary.Primitives.Extensions
{
    public static class PositionExtensions
    {
        public static Vector3 PositionToVector3(this Vector2Uint pos)
        {
            return new Vector3(pos.X, 0, pos.Y);
        }
        
        public static Vector3 PositionToVector3(this Vector2 pos)
        {
            return new Vector3(pos.Y, 0, pos.Y);
        }

        public static Vector2 Vector3ToVector2(this Vector3 position)
        {
            return new Vector2(position.x, position.z);
        }
    }
}