using System;
using System.Collections;
using UnityEngine;

namespace UnityLibrary.UI
{
    public class UI : MonoBehaviour
    {
        [SerializeField] private DrawDestination _draw;
        private bool _started;
        private bool _startEvent;

        private DrawDestination Draw => _draw;

        private DrawAgent _agent;

        private DrawAgent Agent
        {
            get
            {
                if (_agent == null) _agent = FindObjectOfType<DrawAgent>();
                return _agent;
            }
        }

        private bool _steering = true;
        private bool _debugFlow = false;

        private void OnGUI()
        {
            if (!_started)
            {
                GUILayout.BeginArea(new Rect(15, 15, 150, 100));
                if (GUILayout.Button("Start"))
                {
                    _startEvent = true;
                }
                GUILayout.EndArea();
                return;
            }


            GUILayout.BeginArea(new Rect(15, 15, 150, 100));
            if (GUILayout.Button("Randomize destination"))
            {
                Draw.RandomizeDestination();
            }

            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(175, 15, 125, 100));

            if (GUILayout.Button(_steering ? "Disable steering" : "Enable steering"))
            {
                if (Agent == null) return;
                _steering = Agent.UseSteering(!_steering);
            }
            
            if (GUILayout.Button(_debugFlow ? "Disable flow debug" : "Enable flow debug"))
            {
                _debugFlow = !_debugFlow;
                Draw.debugFlow = _debugFlow;
            }

            GUILayout.EndArea();
        }

        private void LateUpdate()
        {
            if (_startEvent)
            {
                StartCoroutine(LateDraw());
            }
        }

        private IEnumerator LateDraw()
        {
            _started = true;
            _startEvent = false;
            Draw.gameObject.SetActive(true);
            yield return null;
            Draw.RandomizeDestination();
        }
    }
}