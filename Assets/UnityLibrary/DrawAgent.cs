using System;
using Dck.Pathfinder;
using UnityEngine;
using UnityLibrary.Primitives.Extensions;
using Random = System.Random;
using Vector2 = System.Numerics.Vector2;

namespace UnityLibrary
{
    public class DrawAgent : MonoBehaviour
    {
        [SerializeField] private DrawMesh drawMesh;
        private GameMap _gameMap;
        public DrawDestination destination;
        public SteeringAgent agent;

        private Vector2 _debugDir;
        private readonly Random _random = new Random();
        private float _minWidth, _minHeight, _maxWidth, _maxHeight;
        private Transform _line;
        private SpriteRenderer _outSideColor;
        private Spawner _spawner;

        private void Start()
        {
            _line =transform.GetChild(2);
            _outSideColor = GetComponentInChildren<SpriteRenderer>();
            _spawner = FindObjectOfType<Spawner>();

        }

        private void Update()
        {
            if (destination == null)
            {
                var dest = _spawner.destinations;
                destination = dest[_random.Next(0, dest.Count)];
                _outSideColor.color = destination.Color;
                if (destination == null) return;
                if (_gameMap == null && agent == null)
                {
                    _gameMap = drawMesh.gameMap;
                    var min = _gameMap.GetWorldPositionFromCell(0, 0);
                    _minWidth = min.X;
                    _minHeight = min.Y;

                    var max = _gameMap.GetWorldPositionFromCell(_gameMap.Width - 1, _gameMap.Height - 1);
                    _maxWidth = max.X;
                    _maxHeight = max.Y;

                    Debug.Log($"{_minWidth} {_minHeight} {_maxWidth} {_maxHeight}");

                    agent = new SteeringAgent(_gameMap);
                }
            }
        }

        public void Move()
        {
            agent.Position = transform.position.Vector3ToVector2();

            if (!((transform.position - destination.transform.position).magnitude > 1F)) return;
            
            var dir = destination.pathFinder.GetDirection(agent, _debugDir, SteeringOptions.AgentsSpeed, Time.deltaTime);
            if(dir == Vector2.Zero) return;
            dir = Vector2.Normalize(dir);
            transform.Translate(new Vector3(dir.X, 0, dir.Y) * (SteeringOptions.AgentsSpeed * Time.deltaTime));
            var pos = transform.position;
            var clampedPos = new Vector3(Mathf.Clamp(pos.x, _minWidth, _maxWidth), 0,
                Mathf.Clamp(pos.z, _minHeight, _maxHeight));
            transform.position = clampedPos;
            _debugDir = dir;
            _line.LookAt(pos + dir.PositionToVector3());
        }

        public void OnDrawGizmos()
        {
            if (_debugDir == Vector2.Zero) return;
            var position = transform.position;
            Gizmos.DrawLine(position, position + _debugDir.PositionToVector3().normalized);
        }

        public void RandomizeOrigin()
        {
            if (_gameMap == null)
            {
                Invoke(nameof(RandomizeOrigin), 0.001F);
                return;
            }

            destination = null;
            var i = (uint) _random.Next(0, (int) _gameMap.Width);
            var j = (uint) _random.Next(0, (int) _gameMap.Height);
            var tileType = _gameMap.GetCellAt(i, j);
            if (tileType != MapCellType.Clear)
            {
                RandomizeOrigin();
                return;
            }

            var pos = _gameMap.GetWorldPositionFromCell(i, j);
            transform.position = pos.PositionToVector3();
        }
    }
}