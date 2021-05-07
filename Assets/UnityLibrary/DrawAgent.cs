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

            _agent.position = transform.position.Vector3ToVector2();
            var dir = _agent.GetNextDirectionVector(_gameMap, _destination.FlowField);
            if(dir != Vector2.Zero && (transform.position - _destination.transform.position).magnitude > 1F)
                transform.Translate(new Vector3(dir.X, 0, dir.Y) * (2F * Time.deltaTime));
        }
    }
}