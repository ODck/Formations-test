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
        private DrawDestination _lastDestination;

        private void Start()
        {
            if(PlayerPrefs.HasKey(nameof(SteeringOptions.EnableSteering)))
                SteeringOptions.EnableSteering = PlayerPrefs.GetInt(nameof(SteeringOptions.EnableSteering))==1;
            if(PlayerPrefs.HasKey(nameof(SteeringOptions.EnableAvoidAgents)))
                SteeringOptions.EnableAvoidAgents = PlayerPrefs.GetInt(nameof(SteeringOptions.EnableAvoidAgents))==1;
            if(PlayerPrefs.HasKey(nameof(SteeringOptions.EnableAvoidObstacles)))
                SteeringOptions.EnableAvoidObstacles = PlayerPrefs.GetInt(nameof(SteeringOptions.EnableAvoidObstacles))==1;
            if(PlayerPrefs.HasKey(nameof(SteeringOptions.AgentsSpeed)))
                SteeringOptions.AgentsSpeed = PlayerPrefs.GetFloat(nameof(SteeringOptions.AgentsSpeed));
            if(PlayerPrefs.HasKey(nameof(SteeringOptions.StopRange)))
                SteeringOptions.StopRange = PlayerPrefs.GetFloat(nameof(SteeringOptions.StopRange));
            if(PlayerPrefs.HasKey(nameof(SteeringOptions.SteeringForce)))
                SteeringOptions.SteeringForce = PlayerPrefs.GetFloat(nameof(SteeringOptions.SteeringForce));
            if(PlayerPrefs.HasKey(nameof(SteeringOptions.AvoidAgentsForce)))
                SteeringOptions.AvoidAgentsForce = PlayerPrefs.GetFloat(nameof(SteeringOptions.AvoidAgentsForce));
            if(PlayerPrefs.HasKey(nameof(SteeringOptions.AvoidAgentsOverlap)))
                SteeringOptions.AvoidAgentsOverlap = PlayerPrefs.GetFloat(nameof(SteeringOptions.AvoidAgentsOverlap));
            
        // public static bool EnableSteering = true;
        // public static bool EnableAvoidAgents = true;
        // public static bool EnableAvoidObstacles = true;
        // public static float AgentsSpeed = 6F;
        // public static float SteeringForce = 0.0075F;
        // public static float AvoidAgentsForce = 0.04F;
        // public static float AvoidObstaclesForce = 0.06F;
        }

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
                Spawner.destinations.ForEach(x => x.RandomizeDestination());
                if (_debugFlow)
                {
                    _lastDestination?.ClearDrawFlowField();
                    _lastDestination?.StartSlowDraw();
                }
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

            var offset = (_debugFlow ? 30 : 0);
            offset += _lastDestination != null ? 25 : 0;
            GUI.Box(new Rect(175, 115+ offset, 125, 225+offset), "", GUI.skin.box);
            GUILayout.BeginArea(new Rect(175, 15, 125, 400 + offset));

            if (GUILayout.Button(SteeringOptions.EnableSteering ? "Disable steering" : "Enable steering"))
            {
                SteeringOptions.EnableSteering = !SteeringOptions.EnableSteering;
                PlayerPrefs.SetInt(nameof(SteeringOptions.EnableSteering), SteeringOptions.EnableSteering ? 1:0);
            }

            if (GUILayout.Button(SteeringOptions.EnableAvoidAgents ? "Disable avoid/other" : "Enable avoid/other"))
            {
                SteeringOptions.EnableAvoidAgents = !SteeringOptions.EnableAvoidAgents;
                PlayerPrefs.SetInt(nameof(SteeringOptions.EnableAvoidAgents), SteeringOptions.EnableAvoidAgents ? 1:0);
            }

            if (GUILayout.Button(SteeringOptions.EnableAvoidObstacles ? "Disable avoid/wall" : "Enable avoid/wall"))
            {
                SteeringOptions.EnableAvoidObstacles = !SteeringOptions.EnableAvoidObstacles;
                PlayerPrefs.SetInt(nameof(SteeringOptions.EnableAvoidObstacles), SteeringOptions.EnableAvoidObstacles ? 1:0);
            }

            if (GUILayout.Button(_debugFlow ? "Disable flow debug" : "Enable flow debug"))
            {
                _debugFlow = !_debugFlow;
                //Draw.debugFlow = _debugFlow;
            }

            if (_debugFlow)
            {
                GUILayout.Space(5);
                if (_lastDestination != null)
                {
                    var button = ScriptableObject.CreateInstance<GUISkin>().button;
                    button.normal.background = MakeTex(1, 1, _lastDestination.Color);
                    //button.contentOffset = new Vector2(5, 0);
                    //button.border = new RectOffset(0, 0, 20, 20);
                    GUILayout.Button("  Active Destination", button);
                    GUILayout.Space(10);
                }

                GUILayout.BeginHorizontal();
                foreach (var destination in spawner.destinations)
                {
                    var button = ScriptableObject.CreateInstance<GUISkin>().button;
                    button.normal.background = MakeTex(1, 1, destination.Color);
                    if (GUILayout.Button("", button))
                    {
                        _lastDestination?.ClearDrawFlowField();
                        _lastDestination = destination;
                        _lastDestination.StartSlowDraw();
                    }
                }

                GUILayout.EndHorizontal();
            }
            else
            {
                _lastDestination?.ClearDrawFlowField();
                _lastDestination = null;
            }

            GUILayout.Label($"Agents Speed {SteeringOptions.AgentsSpeed:f4}");
            SteeringOptions.AgentsSpeed = GUILayout.HorizontalSlider(SteeringOptions.AgentsSpeed, 0, 20);
            PlayerPrefs.SetFloat(nameof(SteeringOptions.AgentsSpeed), SteeringOptions.AgentsSpeed);
            
            GUILayout.Label($"Agents Range {SteeringOptions.StopRange:f4}");
            SteeringOptions.StopRange = GUILayout.HorizontalSlider(SteeringOptions.StopRange, 0, 5);
            PlayerPrefs.SetFloat(nameof(SteeringOptions.StopRange), SteeringOptions.StopRange);
            
            GUILayout.Label($"Seek F {SteeringOptions.SteeringForce:f4}");
            SteeringOptions.SteeringForce = GUILayout.HorizontalSlider(SteeringOptions.SteeringForce, 0, 0.1F);
            PlayerPrefs.SetFloat(nameof(SteeringOptions.SteeringForce), SteeringOptions.SteeringForce);
            
            GUILayout.Label($"Avoid Ahead F{SteeringOptions.AvoidAgentsForce:f4}");
            SteeringOptions.AvoidAgentsForce = GUILayout.HorizontalSlider(SteeringOptions.AvoidAgentsForce, 0, 0.9F);
            PlayerPrefs.SetFloat(nameof(SteeringOptions.AvoidAgentsForce), SteeringOptions.AvoidAgentsForce);
            
            GUILayout.Label($"Avoid Overlap F {SteeringOptions.AvoidAgentsOverlap:f4}");
            SteeringOptions.AvoidAgentsOverlap =
                GUILayout.HorizontalSlider(SteeringOptions.AvoidAgentsOverlap, 0, 2F);
            PlayerPrefs.SetFloat(nameof(SteeringOptions.AvoidAgentsOverlap), SteeringOptions.AvoidAgentsOverlap);

            GUILayout.EndArea();
        }

        private Texture2D MakeTex(int width, int height, Color col)
        {
            var pix = new Color[width * height];

            for (var i = 0; i < pix.Length; i++)
                pix[i] = col;

            var result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();

            return result;
        }
    }
}