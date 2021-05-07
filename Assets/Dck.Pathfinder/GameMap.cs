using System;
using Dck.Pathfinder.Primitives;
using UnityEngine;
using Random = System.Random;
using Vector2 = System.Numerics.Vector2;

// ReSharper disable PossibleLossOfFraction

namespace Dck.Pathfinder
{
    public class GameMap
    {
        public readonly uint Width;
        public readonly uint Height;

        public const float CellSize = 1.0f;

        private readonly MapCellType[] grid;
        private readonly Random random = new Random();

        private const int MinMapDimension = 16;

        public GameMap(uint mapWidth, uint mapHeight, MapCellType[] cells)
        {
            if (mapWidth % 2 != 0 || mapHeight % 2 != 0)
                throw new Exception("Width and Height have to be even");
            Width = mapWidth;
            Height = mapHeight;
            grid = cells;
        }

        public GameMap(uint mapWidth, uint mapHeight)
        {
            if (mapWidth < MinMapDimension || mapHeight < MinMapDimension)
            {
                throw new Exception($"Map grid dimensions must be at least (W:{MinMapDimension},H:{MinMapDimension})");
            }

            Width = mapWidth;
            Height = mapHeight;

            grid = new MapCellType[Width * Height];
        }

        public MapCellType[] GetCellsArray()
        {
            return grid;
        }

        public bool GetCellAtWorldCoords(float x, float y, out uint i, out uint j)
        {
            const uint cellWidth = (uint) CellSize;
            const uint cellHeight = (uint) CellSize;

            x += Width / 2;
            y += Height / 2;

            i = ((uint) Math.Floor(x)) / cellWidth;
            j = ((uint) Math.Floor(y)) / cellHeight;

            return !(x < 0) && !(x >= Width / cellWidth) && !(y < 0) && !(y >= Height / cellHeight);
        }

        public void FillWithRandomBlockType(uint blocksAmount, MapCellType blockType)
        {
            for (var b = 0; b < blocksAmount; b++)
            {
                var i = random.Next(0, (int) Width);
                var j = random.Next(0, (int) Height);
                SetCellAt((uint) i, (uint) j, blockType);
                Debug.Log("cell @ " + i + "  " + j);
            }
        }

        public bool SetCellAt(uint i, uint j, MapCellType value)
        {
            var index = i + j * Width;

            if (index < 0 || index >= grid.Length)
            {
                Debug.LogWarning("err");
                return false;
            }

            grid[index] = value;

            return true;
        }

        public MapCellType GetCellAt(uint i, uint j)
        {
            var index = i + j * Width;
            return index >= grid.Length ? MapCellType.Invalid : grid[index];
        }

        public Vector2Uint GetCellPositionFromWorld(float x, float y)
        {
            x += Width / 2;
            y += Height / 2;
            return new Vector2Uint((uint) x, (uint) y);
        }

        public Vector2 GetWorldPositionFromCell(uint x, uint y)
        {
            var x1 = x - (float)Math.Floor((float) (Width / 2)) + CellSize/2;
            var y1 = y - (float)Math.Floor((float) (Height / 2)) + CellSize/2;
            return new Vector2(x1, y1);
        }
        
        
    }

    public enum MapCellType
    {
        Clear,
        Wall,
        Water,
        Bush,
        Invalid
    }
}