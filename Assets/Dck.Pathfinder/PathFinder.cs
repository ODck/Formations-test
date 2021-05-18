using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;

namespace Dck.Pathfinder
{
    public class PathFinder
    {
        private readonly HashSet<SteeringAgent> _followers = new HashSet<SteeringAgent>();
        public readonly DijkstraGrid DijkstraGrid;

        public readonly Dictionary<SteeringAgent, HashSet<SteeringAgent>> _collidingEntities =
            new Dictionary<SteeringAgent, HashSet<SteeringAgent>>();

        private readonly GameMap _gameMap;

        public PathFinder(DijkstraGrid dijkstraGrid, GameMap gameMap)
        {
            DijkstraGrid = dijkstraGrid;
            _gameMap = gameMap;
        }

        public void BeginFollowerRegister()
        {
            _followers.Clear();
            _collidingEntities.Clear();
        }

        public void AddFollower(SteeringAgent steeringAgent)
        {
            _followers.Add(steeringAgent);
        }

        public void EndFollowerRegister()
        {
            for (var i = 0; i < _followers.Count; i++)
            {
                var agent1 = _followers.Skip(i).First();
                for (var skipUntilIndex = i + 1; skipUntilIndex < _followers.Count; skipUntilIndex++)
                {
                    var agent2 = _followers.Skip(skipUntilIndex).First();

                    var centerDistanceSqr = (agent2.Position - agent1.Position).LengthSquared();
                    var sumRadiiSqr = Math.Pow(agent2.ColliderRadius + agent1.ColliderRadius, 2);

                    //TODO: Only register collision for the furthest

                    if (!(centerDistanceSqr < sumRadiiSqr)) continue;

                    if (!_collidingEntities.TryGetValue(agent1, out var collisionInfo1))
                    {
                        collisionInfo1 = new HashSet<SteeringAgent>();
                        _collidingEntities.Add(agent1, collisionInfo1);
                    }

                    collisionInfo1.Add(agent2);

                    if (!_collidingEntities.TryGetValue(agent2, out var collisionInfo2))
                    {
                        collisionInfo2 = new HashSet<SteeringAgent>();
                        _collidingEntities.Add(agent2, collisionInfo2);
                    }

                    collisionInfo2.Add(agent1);
                }
            }
        }

        //https://gamedevelopment.tutsplus.com/tutorials/understanding-steering-behaviors-collision-avoidance--gamedev-7777
        public Vector2 GetDirection(SteeringAgent agent)
        {
            var finalVector = Vector2.Zero;
            var flowVector = agent.GetNextDirectionVector(_gameMap, DijkstraGrid, 6F);


            var avoidanceVector = Vector2.Zero;
            if (SteeringOptions.EnableAvoidAgents)
            {
                if (_collidingEntities.TryGetValue(agent, out var colliding))
                {
                    //Choosing the most threatening
                    var mostThreatening = colliding.First();
                    var dist = (mostThreatening.Position - agent.Position).LengthSquared();
                    foreach (var entity in colliding.Skip(1))
                    {
                        var dist2 = (entity.Position - agent.Position).LengthSquared();
                        if (!(dist2 < dist)) continue;
                        dist = dist2;
                        mostThreatening = entity;
                    }
                    //TODO: if more than 1 collision unlucky F

                    var ahead = agent.Position + flowVector * 6 * Time.deltaTime;
                    avoidanceVector = ahead - mostThreatening.Position;
                    avoidanceVector = Vector2.Normalize(avoidanceVector) * SteeringOptions.AvoidAgentsForce;
                }
            }

            var avoidObstaclesVector = Vector2.Zero;
            if (SteeringOptions.EnableAvoidObstacles)
            {
                //var neighbours = DijkstraGrid.StraightNeighboursOf() 
            }

            if (SteeringOptions.EnableSteering)
            {
            }

            return flowVector + avoidanceVector;

            // var collidingEntity = colliding.First();
            // v2 -= collidingEntity.Position;
            // v2 = Vector2.Normalize(v2) * MaxAvoidForce;
            // return v2;
        }
    }

    public static class SteeringOptions
    {
        public static bool EnableSteering = true;
        public static bool EnableAvoidAgents = true;
        public static bool EnableAvoidObstacles = true;
        public const float SteeringForce = 0.0075F;
        public const float AvoidAgentsForce = 0.4F;
        public const float AvoidObstaclesForce = 0.6F;
    }
}