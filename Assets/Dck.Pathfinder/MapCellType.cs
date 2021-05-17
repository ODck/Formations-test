using UnityEngine;

namespace Dck.Pathfinder
{
    public enum MapCellType
    {
        Clear,
        Wall,
        Water,
        Bush,
        Invalid
    }

    public static class MathUtils
    {
        public static float Clamp(float value, float min, float max)
        {
            return Mathf.Clamp(value, min, max);
        }
    }
}