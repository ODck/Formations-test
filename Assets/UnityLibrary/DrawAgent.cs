using System;
using Box2DSharp.Collision.Shapes;
using Box2DSharp.Dynamics;
using Dck.Pathfinder;
using Dck.Pathfinder.Primitives;
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
        public Body _2dBody;

        private void Start()
        {
            _line = transform.GetChild(2);
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

            Debug.Log(_2dBody.GetPosition());
        }

        private void LateUpdate()
        {
            if (_gameMap != null && agent != null && _2dBody == null)
            {
                CreatePhysics();
            }
        }

        private void CreatePhysics()
        {
            var pos = transform.position.Vector3ToVector2();
            var cellPos = _gameMap.GetCellPositionFromWorld(pos.X, pos.Y);
            var body = new BodyDef
            {
                Position = cellPos.ToVector2(),
                BodyType = BodyType.DynamicBody
            };
            _2dBody = PhysicsWorld.World.CreateBody(body);
            var shape = new CircleShape {Radius = 0.5F};
            var fixDef = new FixtureDef
            {
                Density = 1F,
                Filter = new Filter
                    {CategoryBits = PhysicsCategory.CATEGORY_MINION, MaskBits = PhysicsMask.COLLIDE_OBSTACLES},
                IsSensor = false,
                Shape = shape
            };
            _2dBody.CreateFixture(fixDef);
            fixDef.IsSensor = true;
            fixDef.Filter = new Filter
                {CategoryBits = PhysicsCategory.CATEGORY_MINION, MaskBits = PhysicsMask.COLLIDE_MINIONS};
            _2dBody.CreateFixture(fixDef);
        }

        public void MoveBody()
        {
            agent.Position = transform.position.Vector3ToVector2();

            if (!((transform.position - destination.transform.position).magnitude > SteeringOptions.StopRange))
            {
                _2dBody.SetLinearVelocity(Vector2.Zero);
                return;
            }

            var dir = destination.pathFinder.GetDirection(agent, _2dBody, _debugDir, SteeringOptions.AgentsSpeed,
                Time.deltaTime);
            if (dir != Vector2.Zero)
                dir = Vector2.Normalize(dir) * (SteeringOptions.AgentsSpeed);
            //transform.Translate(new Vector3(dir.X, 0, dir.Y));
            // var pos = transform.position;
            // var clampedPos = new Vector3(Mathf.Clamp(pos.x, _minWidth, _maxWidth), 0,
            //     Mathf.Clamp(pos.z, _minHeight, _maxHeight));
            _2dBody.SetLinearVelocity(dir);
            //transform.position = clampedPos;
            _debugDir = dir;
            //_line.LookAt(pos + dir.PositionToVector3());
        }

        public void ApplyTranslate()
        {
            var pos = _2dBody.GetPosition();
            pos = new Vector2(Mathf.Clamp(pos.X, 0, _gameMap.Width - 1),
                Mathf.Clamp(pos.Y, 0, _gameMap.Height - 1));

            var wPosV3 = _gameMap.GetWorldPositionFromSimulated(pos.X, pos.Y).PositionToVector3();
            transform.position = wPosV3;
            if (_2dBody.LinearVelocity == Vector2.Zero)
                _line.LookAt(destination.transform.position);
            else
                _line.LookAt(wPosV3 + agent.LastKnownDirection.PositionToVector3());
        }

        public void OnDrawGizmos()
        {
            if (_debugDir == Vector2.Zero) return;
            var position = transform.position;
            Gizmos.DrawLine(position, position + _debugDir.PositionToVector3().normalized);
        }

        public void RandomizeOrigin()
        {
            if (_gameMap == null || _2dBody == null)
            {
                Invoke(nameof(RandomizeOrigin), Time.deltaTime);
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

            var pos = new Vector2(i, j);
            _2dBody.SetTransform(pos, _2dBody.GetAngle());
            transform.position = _gameMap.GetWorldPositionFromSimulated(i, j).PositionToVector3();
        }

        private void OnDestroy()
        {
            PhysicsWorld.World.DestroyBody(_2dBody);
        }
    }
}