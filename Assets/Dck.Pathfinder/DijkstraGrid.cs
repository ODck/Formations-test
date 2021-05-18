using System;
using System.Collections.Generic;
using System.Linq;
using Dck.Pathfinder.Primitives;
using JetBrains.Annotations;
using Server.vendor.Pathfinder;
using Vector2 = System.Numerics.Vector2;

namespace Dck.Pathfinder
{
    [PublicAPI]
    public class DijkstraGrid
    {
        public readonly DijkstraTile[,] DijkstraTiles;
        public DijkstraTile Destination { get; private set; }
        protected readonly GameMap _gameMap;
        public Vector2Uint GridSize { get; }

        public uint Columns => GridSize.X;
        public uint Rows => GridSize.Y;

        protected DijkstraGrid(DijkstraTile[,] tiles, DijkstraTile target, GameMap gameMap)
        {
            GridSize = new Vector2Uint(tiles.GetLength(0), tiles.GetLength(1));
            CheckGridSize(GridSize);
            DijkstraTiles = tiles;
            Destination = target;
            _gameMap = gameMap;
            GenerateFlowField();
        }

        public static DijkstraGrid Create(DijkstraTile[,] tiles, DijkstraTile target, GameMap gameMap)
        {
            return new DijkstraGrid(tiles, target, gameMap);
        }

        public static DijkstraGrid CreateFromGameMap(GameMap gameMap, uint destinationX, uint destinationY)
        {
            DijkstraTile destination = null;
            var tiles = new DijkstraTile[gameMap.Width, gameMap.Height];
            for (var i = 0; i < gameMap.Width; i++)
            {
                for (var j = 0; j < gameMap.Height; j++)
                {
                    //var tilePos = Vector2.UnitX * (i * GameMap.CellSize) + Vector2.UnitY * (j * GameMap.CellSize);
                    var tile = new DijkstraTile(new Vector2(i, j));
                    if (gameMap.GetCellAt((uint) i, (uint) j) != MapCellType.Clear)
                    {
                        tile.Weight = int.MaxValue;
                    }

                    tiles[i, j] = tile;
                    if (i == destinationX && j == destinationY)
                    {
                        destination = tile;
                    }
                }
            }

            if (destination == null) throw new NullReferenceException("destination is null");
            var grid = new DijkstraGrid(tiles, destination, gameMap);
            grid.SetWeights(destination);
            return grid;
        }

        protected virtual void SetWeights(DijkstraTile destination)
        {
            destination.Weight = 0;
            var toCheckNeighbours = new Queue<DijkstraTile>();
            var visited = new HashSet<DijkstraTile>();
            toCheckNeighbours.Enqueue(destination);

            while (toCheckNeighbours.Count > 0)
            {
                var tile = toCheckNeighbours.Dequeue();
                var neighbours = StraightNeighboursOf(tile, DijkstraTiles, _gameMap);

                foreach (var dijkstraTile in neighbours.Where(dijkstraTile => !visited.Contains(dijkstraTile)))
                {
                    dijkstraTile.Weight = dijkstraTile == destination ? 0 : tile.Weight + 1;
                    toCheckNeighbours.Enqueue(dijkstraTile);
                    visited.Add(dijkstraTile);
                }
            }
        }

        private static void CheckGridSize(Vector2Uint gridSize)
        {
            if (gridSize.X < 1)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(gridSize), $"Argument {nameof(gridSize.X)} is {gridSize.X} but should be >= 1");
            }

            if (gridSize.Y < 1)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(gridSize), $"Argument {nameof(gridSize.Y)} is {gridSize.Y} but should be >= 1");
            }
        }

        public void GenerateFlowField()
        {
            for (var i = 0; i < Columns; i++)
            {
                for (var j = 0; j < Rows; j++)
                {
                    var tile = DijkstraTiles[i, j];
                    if (tile.Weight == int.MaxValue) continue;
                    var neighbours = GetAllNeighbours(tile);
                    var pos = new Vector2(i, j);
                    var minPos = Vector2.Zero;
                    var minNotNull = false;
                    var minDist = 0;
                    foreach (var dijkstraTile in neighbours)
                    {
                        var dist = dijkstraTile.Weight - tile.Weight;
                        if (dist >= minDist) continue;
                        minPos = dijkstraTile.Position.ToVector2();
                        minNotNull = true;
                        minDist = dist;
                    }

                    if (minNotNull)
                    {
                        tile.FlowDirection = minPos - pos;
                    }
                }
            }
        }

        public static IEnumerable<DijkstraTile> StraightNeighboursOf(DijkstraTile tile, DijkstraTile[,] tiles,
            GameMap gameMap)
        {
            var neighbours = new List<DijkstraTile>();
            if (tile.Position.X > 0)
            {
                var dijkstraTile = tiles[(int) (tile.Position.X - 1), (int) tile.Position.Y];
                if (gameMap.GetCellAt(tile.Position.X - 1, tile.Position.Y) != MapCellType.Wall)
                    neighbours.Add(dijkstraTile);
            }

            if (tile.Position.Y > 0)
            {
                var dijkstraTile = tiles[(int) tile.Position.X, (int) (tile.Position.Y - 1)];
                if (gameMap.GetCellAt(tile.Position.X, tile.Position.Y - 1) != MapCellType.Wall)
                    neighbours.Add(dijkstraTile);
            }

            if (tile.Position.X < tiles.GetLength(0) - 1)
            {
                var dijkstraTile = tiles[(int) (tile.Position.X + 1), (int) tile.Position.Y];
                if (gameMap.GetCellAt(tile.Position.X + 1, tile.Position.Y) != MapCellType.Wall)
                    neighbours.Add(dijkstraTile);
            }

            if (tile.Position.Y < tiles.GetLength(1) - 1)
            {
                var dijkstraTile = tiles[(int) tile.Position.X, (int) (tile.Position.Y + 1)];
                if (gameMap.GetCellAt(tile.Position.X, tile.Position.Y + 1) != MapCellType.Wall)
                    neighbours.Add(dijkstraTile);
            }

            return neighbours;
        }

        private IEnumerable<DijkstraTile> GetAllNeighbours(DijkstraTile tile)
        {
            var neighbours = new List<DijkstraTile>();
            var x = (int) tile.Position.X;
            var y = (int) tile.Position.Y;
            var north = IsValid(x, y + 1);
            var south = IsValid(x, y - 1);
            var east = IsValid(x + 1, y);
            var west = IsValid(x - 1, y);
            var northeast = IsValid(x + 1, y + 1);
            var northwest = IsValid(x - 1, y + 1);
            var southeast = IsValid(x + 1, y - 1);
            var southwest = IsValid(x - 1, y - 1);

            //Check clockwise
            //Nested ifs bc, we dont want to traverse between corners
            if (west)
            {
                neighbours.Add(DijkstraTiles[x - 1, y]);
                
                if (northwest)
                {
                    neighbours.Add(DijkstraTiles[x - 1, y + 1]);
                }
            }

            if (north)
            {
                neighbours.Add(DijkstraTiles[x, y + 1]);
                
                if (northeast)
                {
                    neighbours.Add(DijkstraTiles[x + 1, y + 1]);
                }
            }
            

            if (east)
            {
                neighbours.Add(DijkstraTiles[x + 1, y]);
                
                if (southeast)
                {
                    neighbours.Add(DijkstraTiles[x + 1, y - 1]);
                }
            }
            

            if (south)
            {
                neighbours.Add(DijkstraTiles[x, y - 1]);
                
                if (southwest)
                {
                    neighbours.Add(DijkstraTiles[x - 1, y - 1]);
                }
            }
            

            return neighbours;
        }

        private static bool IsValid(int x, int y, Vector2Uint gridSize, DijkstraTile[,] grid)
        {
            return x >= 0 && y >= 0 && x < gridSize.X && y < gridSize.Y && grid[x, y].Weight != int.MaxValue;
        }

        private bool IsValid(int x, int y)
        {
            return IsValid(x, y, GridSize, DijkstraTiles);
        }

        public void TryRecalculateGrid(uint x, uint y)
        {
            if (Destination.Position.X == x && Destination.Position.Y == y)
            {
                return;
            }
            RecalculateGrid(x, y);
        }

        private void RecalculateGrid(uint x, uint y)
        {
            Destination = DijkstraTiles[x, y];
            SetWeights(Destination);
            GenerateFlowField();
        }
    }
}