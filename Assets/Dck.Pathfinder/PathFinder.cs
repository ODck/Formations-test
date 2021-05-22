using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Box2DSharp.Dynamics;
using Dck.Pathfinder.Primitives;
using UnityEngine;
using UnityLibrary;
using Vector2 = System.Numerics.Vector2;

namespace Dck.Pathfinder
{
    public class PathFinder
    {
        //private readonly Dictionary<SteeringAgent, IBox> _followers = new Dictionary<SteeringAgent, IBox>();
        public readonly DijkstraGrid DijkstraGrid;

        private readonly GameMap _gameMap;
        //private World _humperWorld;

        public PathFinder(DijkstraGrid dijkstraGrid, GameMap gameMap)
        {
            DijkstraGrid = dijkstraGrid;
            _gameMap = gameMap;
            //_humperWorld = new World(_gameMap.Width, _gameMap.Height);
        }

        //https://gamedevelopment.tutsplus.com/tutorials/understanding-steering-behaviors-collision-avoidance--gamedev-7777
        public Vector2 GetDirection(SteeringAgent agent, Body body, Vector2 currentDirection, float velocity, float tickFactor)
        {
            var flowVector = agent.GetNextDirectionVector(DijkstraGrid);
            var finalVector = currentDirection;
            
            
            if (SteeringOptions.EnableSteering)
            {
                var steeringVector = Vector2Extensions.Truncate(flowVector, SteeringOptions.SteeringForce * velocity);
                finalVector += steeringVector;
                // var steeringVector = finalVector - currentDirection;
                // finalVector = currentDirection + steeringVector * SteeringOptions.SteeringForce * velocity;
            }
            
            if (SteeringOptions.EnableAvoidAgents)
            {
                var ray = new RayCastAvoidAgents(body.UserData);
                var position = body.GetTransform().Position;
                //var aheadPoint = body.GetTransform().Position + Vector2Extensions.Truncate(finalVector, agent.ColliderRadius);
                
                
                PhysicsWorld.World.RayCast(ray, position, position+ Vector2.Normalize(currentDirection));
                if (ray.Hit)
                {
                    Debug.Log("HITTTT!!");
                    var avoidVector = position - ray.BodyCenter;
                    Debug.Log(avoidVector + " HITS");
                    finalVector += Vector2Extensions.Truncate(avoidVector, SteeringOptions.AvoidAgentsForce);
                }
                else
                {
                    foreach (var fixture in body.FixtureList)
                    {
                        if (!PhysicsWorld.ContactListener.Contacts.ContainsKey(fixture)) continue;
                        Body body2 = null;
                        var dist = float.MaxValue;
                        foreach (var fixture2 in PhysicsWorld.ContactListener.Contacts[fixture])
                        {
                            var distance = (position - fixture2.Body.GetPosition()).LengthSquared();
                            if (!(distance < dist)) continue;
                            dist = distance;
                            body2 = fixture2.Body;
                        }

                        if (body2 == null) continue;
                        var avoidVector = position - body2.GetPosition();
                        Debug.Log(avoidVector + " HITS22222");
                        finalVector += Vector2Extensions.Truncate(avoidVector, SteeringOptions.AvoidAgentsOverlap);
                    }
                }
            }

            agent.LastKnownDirection = finalVector;
            return finalVector;
        }
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class SteeringOptions
    {
        public static bool EnableSteering = true;
        public static bool EnableAvoidAgents = true;
        public static bool EnableAvoidObstacles = true;
        public static float AgentsSpeed = 6F;
        public static float StopRange = 1F;
        public static float SteeringForce = 0.077F;
        public static float AvoidAgentsForce = 0.46F;
        public static float AvoidAgentsOverlap = 0.17F;
    }
    
    public class RayCastAvoidAgents : IRayCastCallback
    {
        public bool Hit;

        public Vector2 Normal;

        public Vector2 Point;
        public Vector2 BodyCenter;
        private readonly object _casterData;

        public RayCastAvoidAgents(object casterData)
        {
            Hit = false;
            _casterData = casterData;
        }

        public float RayCastCallback(Fixture fixture, in Vector2 point, in Vector2 normal, float fraction)
        {
            var body = fixture.Body;
            var userData = body.UserData;
            if (fixture.Filter.CategoryBits != PhysicsCategory.CATEGORY_MINION) return - 1;
            if (userData != _casterData)
            {
                // By returning -1, we instruct the calling code to ignore this fixture and
                // continue the ray-cast to the next fixture.
                return -1.0f;
            }

            Hit = true;
            Point = point;
            BodyCenter = fixture.Body.GetPosition();
            Normal = normal;

            // By returning the current fraction, we instruct the calling code to clip the ray and
            // continue the ray-cast to the next fixture. WARNING: do not assume that fixtures
            // are reported in order. However, by clipping, we can always get the closest fixture.
            return fraction;
        }
        
    }
}