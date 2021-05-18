using System.Numerics;
using Dck.Pathfinder.Primitives;
using Server.vendor.Pathfinder;

namespace Dck.Pathfinder
{
    public class SteeringAgent
    {
        public readonly float ColliderRadius = 0.5F;
        private Vector2 _direction;
        public Vector2 Position;
        public Vector2Uint CellPos { get; private set; }
        private const float Smooth = 0.0075F;
        public bool SteeringActive { get; set; } = true;
        public bool AvoidOthers { get; set; } = true;

        public Vector2 GetNextDirectionVector(GameMap gameMap, DijkstraGrid grid, float velocity)
        {
            var dir = _direction;
            CellPos = gameMap.GetCellPositionFromWorld(Position.X, Position.Y);
            var flow = grid.DijkstraTiles[CellPos.X, CellPos.Y].FlowDirection;
            if (flow != Vector2.Zero)
                dir = flow;
            if (SteeringActive)
            {
                dir -= _direction;
                _direction += dir * Smooth * velocity;
            }
            else
            {
                _direction = dir;
            }

            return _direction;
        }
    }
}