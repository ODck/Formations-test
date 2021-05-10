using System;
using Dck.Pathfinder;
using Dck.Pathfinder.Primitives;
using UnityEngine;
using UnityLibrary.Primitives.Extensions;
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

        private void Update()
        {
            if (_destination == null)
            {
                _destination = FindObjectOfType<DrawDestination>();
                if (_destination == null) return;
                _gameMap = drawMesh.gameMap;
                _agent = new SteeringAgent
                {
                    SteeringActive = true
                };
            }

            const float velocity = 6F;
            _agent.position = transform.position.Vector3ToVector2();
            var dir = _agent.GetNextDirectionVector(_gameMap, _destination.FlowField, velocity);
            if (dir != Vector2.Zero && (transform.position - _destination.transform.position).magnitude > 1F)
                transform.Translate(new Vector3(dir.X, 0, dir.Y) * (velocity * Time.deltaTime));
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
    }
}