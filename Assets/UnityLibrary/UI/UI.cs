using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace UnityLibrary.UI
{
    public class UI : MonoBehaviour
    {
        [SerializeField] private Spawner spawner;
        private bool _started;

        private Spawner Spawner => spawner ? spawner : throw new NullReferenceException();


        private bool _steering = true;
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

            GUILayout.BeginArea(new Rect(175, 15, 125, 100));

            if (GUILayout.Button(_steering ? "Disable steering" : "Enable steering"))
            {
                _steering = !_steering;
                Spawner.agents.ForEach(x=>x.UseSteering(_steering));
            }
            
            if (GUILayout.Button(_debugFlow ? "Disable flow debug" : "Enable flow debug"))
            {
                _debugFlow = !_debugFlow;
                //Draw.debugFlow = _debugFlow;
            }

            GUILayout.EndArea();
        }
    }
}