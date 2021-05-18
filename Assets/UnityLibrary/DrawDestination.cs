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
        public bool debugFlow = true;
        private GameMap _gameMap;
        public DrawMesh mesh;
        public PathFinder pathFinder;
        public GameObject pref;
        private readonly Random _random = new Random();

        private void Start()
        {
            _gameMap = mesh.gameMap;
            var position = transform.position;
            var pos = _gameMap.GetCellPositionFromWorld(position.x, position.z);

            var flowField = DijkstraGrid.CreateFromGameMap(_gameMap, pos.X, pos.Y);
            Debug.Log(flowField.DijkstraTiles.Length);
            pathFinder = new PathFinder(flowField, _gameMap);
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
            StartCoroutine(SlowDrawFlowField());
        }
        
        private readonly List<GameObject> _debugList = new List<GameObject>();
        private IEnumerator SlowDrawFlowField()
        {
            _debugList.ForEach(Destroy);
            _debugList.Clear();
            if (!debugFlow) yield break;
            yield return null;
            Debug.Log(pathFinder.DijkstraGrid.DijkstraTiles.Length);
            foreach (var node in pathFinder.DijkstraGrid.DijkstraTiles)
            {
                if (node.Weight == int.MaxValue) continue;
                var dir = node.FlowDirection;
                var go = Instantiate(pref);
                _debugList.Add(go);
                go.transform.position = Vector3.zero;
                go.transform.LookAt(new Vector3(dir.X, 0, dir.Y));
                var text = go.GetComponentInChildren<TextMeshPro>();
                text.text = node.Weight.ToString();
                text.color = Color.yellow;
                var cellPosition = _gameMap.GetWorldPositionFromCell(node.Position.X, node.Position.Y);
                // ReSharper disable once Unity.InefficientPropertyAccess
                go.transform.position = new Vector3(cellPosition.X, 0, cellPosition.Y);
                //yield return new WaitForSeconds(.05F);
                //yield return null;
            }
        }
    }
}