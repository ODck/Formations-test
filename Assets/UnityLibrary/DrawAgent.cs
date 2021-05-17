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
        private DrawDestination _destination;
        private SteeringAgent _agent;

        private Vector2 _debugDir;
        private readonly Random _random = new Random();
        private float _minWidth, _minHeight, _maxWidth, _maxHeight;
        

        private void Update()
        {
            if (_destination == null)
            {
                var dest = FindObjectsOfType<DrawDestination>();
                _destination = dest[_random.Next(0, dest.Length)];
                if (_destination == null) return;
                if(_gameMap == null && _agent == null)
                {
                    _gameMap = drawMesh.gameMap;
                    var min = _gameMap.GetWorldPositionFromCell(0, 0);
                    _minWidth = min.X;
                    _minHeight = min.Y;
                    
                    var max = _gameMap.GetWorldPositionFromCell(_gameMap.Width -1, _gameMap.Height -1);
                    _maxWidth = max.X;
                    _maxHeight = max.Y;
                    
                    Debug.Log($"{_minWidth} {_minHeight} {_maxWidth} {_maxHeight}");
                    
                    _agent = new SteeringAgent
                    {
                        SteeringActive = true
                    };
                }
            }

            const float velocity = 6F;
            _agent.Position = transform.position.Vector3ToVector2();
            var dir = _agent.GetNextDirectionVector(_gameMap, _destination.FlowField, velocity);
            if (dir != Vector2.Zero && (transform.position - _destination.transform.position).magnitude > 1F)
            {
                transform.Translate(new Vector3(dir.X, 0, dir.Y) * (velocity * Time.deltaTime));
                var pos = transform.position;
                var clampedPos = new Vector3(Mathf.Clamp(pos.x, _minWidth, _maxWidth), 0,
                    Mathf.Clamp(pos.z, _minHeight, _maxHeight));
                transform.position = clampedPos;
            }
            
            _debugDir = dir;
        }

        public void OnDrawGizmos()
        {
            if (_debugDir == Vector2.Zero) return;
            var position = transform.position;
            Gizmos.DrawLine(position, position + _debugDir.PositionToVector3().normalized);
        }

        public bool UseSteering(bool value)
        {
            if (_agent == null) return !value;
            Debug.Log("Steering " + value);
            _agent.SteeringActive = value;
            return value;

        }

        public void RandomizeOrigin()
        {
            if (_gameMap == null)
            {
                Invoke(nameof(RandomizeOrigin), 0.001F);
                return;
            }

            _destination = null;
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