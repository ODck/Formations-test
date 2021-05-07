using System.Collections;
using Dck.Pathfinder;
using Dck.Pathfinder.Primitives;
using TMPro;
using UnityEngine;

namespace UnityLibrary
{
    public class DrawDestination : MonoBehaviour
    {
        [SerializeField] private bool debugFlow;
        private GameMap _gameMap;
        private DijkstraGrid _flowField;
        public DrawMesh mesh;
        public GameObject pref;
        public DijkstraGrid FlowField => _flowField;
        

        private void Start()
        {
            _gameMap = mesh.gameMap;
            var position = transform.position;
            var pos = _gameMap.GetCellPositionFromWorld(position.x, position.z);

            _flowField = DijkstraGrid.CreateFromGameMap(_gameMap, pos.X, pos.Y);
            if(debugFlow)
                StartCoroutine(SlowDrawFlowField());
        }

        private IEnumerator SlowDrawFlowField()
        {
            foreach (var node in _flowField.DijkstraTiles)
            {
                if (node.Weight == int.MaxValue) continue;
                var dir = node.FlowDirection;
                var go = Instantiate(pref);
                go.transform.position = Vector3.zero;
                go.transform.LookAt(new Vector3(dir.X, 0, dir.Y));
                var text = go.GetComponentInChildren<TextMeshPro>();
                text.text = node.Weight.ToString();
                text.color = Color.yellow;
                var cellPosition = _gameMap.GetWorldPositionFromCell(node.Position.X, node.Position.Y);
                // ReSharper disable once Unity.InefficientPropertyAccess
                go.transform.position = new Vector3(cellPosition.X, 0, cellPosition.Y);
                //yield return new WaitForSeconds(.05F);
                yield return null;
            }
        }
    }
}