using System;
using System.Collections;
using System.Collections.Generic;
using Dck.Pathfinder;
using TMPro;
using UnityEngine;
using UnityLibrary.Primitives.Extensions;
using Random = System.Random;

namespace UnityLibrary
{
    public class DrawDestination : MonoBehaviour
    {
        private GameMap _gameMap;
        public DrawMesh mesh;
        public PathFinder pathFinder;
        public GameObject pref;
        private readonly Random _random = new Random();
        private Color _destColor;
        public Color Color => _destColor;

        private void Start()
        {
            _gameMap = mesh.gameMap;
            var position = transform.position;
            var pos = _gameMap.GetCellPositionFromWorld(position.x, position.z);

            var flowField = DijkstraGrid.CreateFromGameMap(_gameMap, pos.X, pos.Y);
            Debug.Log(flowField.DijkstraTiles.Length);
            pathFinder = new PathFinder(flowField, _gameMap);
            _destColor = UnityEngine.Random.ColorHSV(0, 1, 1, 1, 0.5F, 1);
            GetComponentInChildren<Renderer>().material.color = _destColor;
        }

        public void RandomizeDestination()
        {
            Debug.Assert(_gameMap != null, "_gameMap != null");
            if (_gameMap == null)
            {
                Invoke(nameof(RandomizeDestination), 0.001F);
                return;
            }

            var i = (uint) _random.Next(0, (int) _gameMap.Width);
            var j = (uint) _random.Next(0, (int) _gameMap.Height);
            var tileType = _gameMap.GetCellAt(i, j);
            if (tileType != MapCellType.Clear)
            {
                RandomizeDestination();
                return;
            }

            var pos = _gameMap.GetWorldPositionFromCell(i, j);
            transform.position = pos.PositionToVector3();
            pathFinder.DijkstraGrid.TryRecalculateGrid(i, j);
        }

        private readonly List<GameObject> _debugList = new List<GameObject>();

        public void StartSlowDraw() => StartCoroutine(SlowDrawFlowField());
        private IEnumerator SlowDrawFlowField()
        {
            yield return null;
            foreach (var node in pathFinder.DijkstraGrid.DijkstraTiles)
            {
                if (node.Weight == int.MaxValue) continue;
                var dir = node.FlowDirection;
                var go = Instantiate(pref);
                go.SetActive(true);
                _debugList.Add(go);
                go.transform.position = Vector3.zero;
                go.transform.LookAt(new Vector3(dir.X, 0, dir.Y));
                var text = go.GetComponentInChildren<TextMeshPro>();
                text.text = node.Weight.ToString();
                text.color = Color.yellow;
                var cellPosition = _gameMap.GetWorldPositionFromCell(node.Position.X, node.Position.Y);
                // ReSharper disable once Unity.InefficientPropertyAccess
                go.transform.position = new Vector3(cellPosition.X, 0, cellPosition.Y);
            }
        }

        public void ClearDrawFlowField()
        {
            _debugList.ForEach(Destroy);
            _debugList.Clear();
        }

        private void OnDestroy()
        {
            ClearDrawFlowField();
        }
    }
}