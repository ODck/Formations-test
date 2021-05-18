using System;
using System.Collections;
using Dck.Pathfinder;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace UnityLibrary.UI
{
    public class UI : MonoBehaviour
    {
        [SerializeField] private Spawner spawner;
        private bool _started;

        private Spawner Spawner => spawner ? spawner : throw new NullReferenceException();
        
        private bool _debugFlow = false;

        private void OnGUI()
        {
            if (!_started)
            {
                GUILayout.BeginArea(new Rect(15, 15, 150, 100));
                if (GUILayout.Button("Start"))
                {
                    Spawner.SpawnDestination();
                    _started = true;
                }
                GUILayout.EndArea();
                return;
            }


            GUILayout.BeginArea(new Rect(15, 15, 150, 200));
            if (GUILayout.Button("Randomize destination"))
            {
                Spawner.destinations.ForEach(x=>x.RandomizeDestination());
            }

            if (GUILayout.Button("Shuffle Agents"))
            {
                Spawner.ShuffleAgents();
            }
            
            GUILayout.Label("Spawn ---");
            
            if (GUILayout.Button("Spawn Agent"))
            {
                Spawner.SpawnAgent();
            }

            if (GUILayout.Button("Spawn Destination"))
            {
                Spawner.SpawnDestination();
            }

            GUILayout.Label("Clean ---");
            
            if (GUILayout.Button("Clean Destinations"))
            {
                Spawner.DestroyExtraDestinations();
            }

            if (GUILayout.Button("Clean Agents"))
            {
                Spawner.DestroyAllAgents();
            }

            GUILayout.EndArea();

            GUI.Box(new Rect(175,115,125,185),"", GUI.skin.box);
            GUILayout.BeginArea(new Rect(175, 15, 125, 300));

            if (GUILayout.Button(SteeringOptions.EnableSteering ? "Disable steering" : "Enable steering"))
            {
                SteeringOptions.EnableSteering = !SteeringOptions.EnableSteering;
            }
            
            if (GUILayout.Button(SteeringOptions.EnableAvoidAgents ? "Disable avoid/other" : "Enable avoid/other"))
            {
                SteeringOptions.EnableAvoidAgents = !SteeringOptions.EnableAvoidAgents;
            }
            
            if (GUILayout.Button(SteeringOptions.EnableAvoidObstacles ? "Disable avoid/wall" : "Enable avoid/wall"))
            {
                SteeringOptions.EnableAvoidObstacles = !SteeringOptions.EnableAvoidObstacles;
            }
            
            if (GUILayout.Button(_debugFlow ? "Disable flow debug" : "Enable flow debug"))
            {
                _debugFlow = !_debugFlow;
                //Draw.debugFlow = _debugFlow;
            }
            
            GUILayout.Label($"Agents Speed {SteeringOptions.AgentsSpeed:f4}");
            SteeringOptions.AgentsSpeed = GUILayout.HorizontalSlider(SteeringOptions.AgentsSpeed, 0, 20);
            GUILayout.Label($"Steering Force {SteeringOptions.SteeringForce:f4}");
            SteeringOptions.SteeringForce = GUILayout.HorizontalSlider(SteeringOptions.SteeringForce, 0, 0.2F);
            GUILayout.Label($"Agents Force {SteeringOptions.AvoidAgentsForce:f4}");
            SteeringOptions.AvoidAgentsForce = GUILayout.HorizontalSlider(SteeringOptions.AvoidAgentsForce, 0, 1F);
            GUILayout.Label($"Walls Force {SteeringOptions.AvoidObstaclesForce:f4}");
            SteeringOptions.AvoidObstaclesForce = GUILayout.HorizontalSlider(SteeringOptions.AvoidObstaclesForce, 0, 1F);

            GUILayout.EndArea();
        }
    }
}