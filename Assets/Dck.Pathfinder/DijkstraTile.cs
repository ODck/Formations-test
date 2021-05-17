using System.Numerics;
using Dck.Pathfinder.Primitives;

namespace Server.vendor.Pathfinder
{
    public class DijkstraTile
    {
        public Vector2Uint Position { get; set; }
        public int Weight { get; set; }

        public Vector2 FlowDirection { get; set; }

        public DijkstraTile(Vector2 pos)
        {
            Position = pos.ToVector2Uint();
            Weight = -1;
            FlowDirection = Vector2.Zero;
        }
        
        
    }
}