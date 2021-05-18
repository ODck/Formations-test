using System.Numerics;
using Dck.Pathfinder.Primitives;
using Server.vendor.Pathfinder;

namespace Dck.Pathfinder
{
    public class SteeringAgent
    {
        public readonly float ColliderRadius = 0.5F;
        private Vector2 _position;
        private readonly GameMap _gameMap;

        public Vector2 Position
        {
            get => _position;
            set
            {
                CellPos = _gameMap.GetCellPositionFromWorld(Position.X, Position.Y);
                _position = value;
            }
        }

        public SteeringAgent(GameMap gameMap)
        {
            _gameMap = gameMap;
        }

        public Vector2Uint CellPos { get; private set; }

        public Vector2 GetNextDirectionVector(DijkstraGrid grid)
        {
            return grid.DijkstraTiles[CellPos.X, CellPos.Y].FlowDirection;
        }
    }
}