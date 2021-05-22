using System;
using System.Collections.Generic;
using System.Numerics;
using Dck.Pathfinder.Primitives;

namespace Dck.Pathfinder
{
    public class GameMap
    {
        public readonly uint Width;
        public readonly uint Height;

        public const float CellSize = 1.0f;

        public readonly MapCellType[] _grid;
        private readonly Random _random = new Random();

        private const int MinMapDimension = 16;
        private const float PositionEpsilon = 0.01F;

        public GameMap(uint mapWidth, uint mapHeight, MapCellType[] cells)
        {
            if (mapWidth % 2 != 0 || mapHeight % 2 != 0)
                throw new Exception("Width and Height have to be even");
            Width = mapWidth;
            Height = mapHeight;
            _grid = cells;
        }

        public GameMap(uint mapWidth, uint mapHeight)
        {
            if (mapWidth < MinMapDimension || mapHeight < MinMapDimension)
            {
                throw new Exception($"Map grid dimensions must be at least (W:{MinMapDimension},H:{MinMapDimension})");
            }

            Width = mapWidth;
            Height = mapHeight;

            _grid = new MapCellType[Width * Height];
        }

        public IEnumerable<MapCellType> GetCellsArray()
        {
            return _grid;
        }

        public bool GetCellAtWorldCoords(float x, float y, out uint i, out uint j)
        {
            const uint cellWidth = (uint) CellSize;
            const uint cellHeight = (uint) CellSize;

            x += Width / 2F;
            y += Height / 2F;

            i = ((uint) Math.Floor(x)) / cellWidth;
            j = ((uint) Math.Floor(y)) / cellHeight;

            return !(x < 0) && !(x >= Width / cellWidth) && !(y < 0) && !(y >= Height / cellHeight);
        }

        public void FillWithRandomBlockType(uint blocksAmount, MapCellType blockType)
        {
            for (var b = 0; b < blocksAmount; b++)
            {
                var i = _random.Next(0, (int) Width);
                var j = _random.Next(0, (int) Height);
                SetCellAt((uint) i, (uint) j, blockType);
            }
        }

        public bool SetCellAt(uint i, uint j, MapCellType value)
        {
            var index = i + j * Width;

            if (index >= _grid.Length)
            {
                return false;
            }

            _grid[index] = value;

            return true;
        }

        public MapCellType GetCellAt(uint i, uint j)
        {
            var index = i + j * Width;
            return index >= _grid.Length ? MapCellType.Invalid : _grid[index];
        }

        public Vector2Uint GetCellPositionFromWorld(float x, float y)
        {
            x += Width / 2F;
            y += Height / 2F;
            return new Vector2Uint((uint) x, (uint) y);
        }

        public Vector2 GetWorldPositionFromCell(uint x, uint y)
        {
            var x1 = x - (float) Math.Floor((float) (Width / 2F)) + CellSize / 2;
            var y1 = y - (float) Math.Floor((float) (Height / 2F)) + CellSize / 2;
            return new Vector2(x1, y1);
        }
        
        public Vector2 GetWorldPositionFromSimulated(float x, float y)
        {
            var x1 = x - Width / 2F + CellSize / 2;
            var y1 = y - Height / 2F + CellSize / 2;
            return new Vector2(x1, y1);
        }

        public bool InsideMap(Vector2 position, out float virtualX, out float virtualY)
        {
            const uint cellWidth = (uint) CellSize;
            const uint cellHeight = (uint) CellSize;

            virtualX = position.X + Width / 2F;
            virtualY = position.Y + Height / 2F;
            return !(virtualX < 0) && !(virtualX >= Width / cellWidth) && !(virtualY < 0) && !(virtualY >= Height / cellHeight);
        }
        
    }
}