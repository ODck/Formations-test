using System.Numerics;

namespace Dck.Pathfinder
{
    public class SteeringAgent
    {
        private Vector2 _direction;
        public Vector2 position;
        private const float Smooth = 0.0075F;
        public bool SteeringActive { get; set; } = true;

        public Vector2 GetNextDirectionVector(GameMap gameMap, DijkstraGrid grid, float velocity)
        {
            var dir = _direction;
            var cellPos = gameMap.GetCellPositionFromWorld(position.X, position.Y);
            var flow = grid.DijkstraTiles[cellPos.X, cellPos.Y].FlowDirection;
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