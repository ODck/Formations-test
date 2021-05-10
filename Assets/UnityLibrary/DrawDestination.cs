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
        public bool debugFlow;
        private GameMap _gameMap;
        private DijkstraGrid _flowField;
        public DrawMesh mesh;
        public GameObject pref;
        public DijkstraGrid FlowField => _flowField;
        private readonly Random _random = new Random();


        private void Start()
        {
            _gameMap = mesh.gameMap;
            var position = transform.position;
            var pos = _gameMap.GetCellPositionFromWorld(position.x, position.z);

            _flowField = DijkstraGrid.CreateFromGameMap(_gameMap, pos.X, pos.Y);
        }

        private void Update()
        {
            var position = transform.position.Vector3ToVector2();
            var newPos = _gameMap.GetCellPositionFromWorld(position.X, position.Y);
            _flowField.TryRecalculateGrid(newPos.X, newPos.Y);
        }

        private List<GameObject> _debugList = new List<GameObject>();
        private IEnumerator SlowDrawFlowField()
        {
            _debugList.ForEach(Destroy);
            _debugList.Clear();
            if (!debugFlow) yield break;
            yield return null;
            foreach (var node in _flowField.DijkstraTiles)
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

        public void RandomizeDestination()
        {
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
            StartCoroutine(SlowDrawFlowField());
        }
    }
}