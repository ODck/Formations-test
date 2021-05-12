using System.Collections.Generic;
using UnityEngine;

namespace UnityLibrary
{
    public class Spawner : MonoBehaviour
    {
        public List<DrawAgent> agents = new List<DrawAgent>();
        public List<DrawDestination> destinations = new List<DrawDestination>();

        [SerializeField] private DrawAgent agentPrefab;
        [SerializeField] private DrawDestination destinationPrefab;

        public void SpawnAgent()
        {
            var go = Instantiate(agentPrefab);
            agents.Add(go);
            go.RandomizeOrigin();
            go.gameObject.SetActive(true);
        }

        public void ShuffleAgents()
        {
            agents.ForEach(x=>x.RandomizeOrigin());
        }

        public void SpawnDestination()
        {
            var go = Instantiate(destinationPrefab);
            destinations.Add(go);
            go.gameObject.SetActive(true);
            go.RandomizeDestination();
        }

        public void DestroyExtraDestinations()
        {
            for (var i = destinations.Count-1; i >= 1; i--)
            {
                var go = destinations[i];
                Destroy(go.gameObject);
                destinations.Remove(go);
            }
        }

        public void DestroyAllAgents()
        {
            agents.ForEach(x=>Destroy(x.gameObject));
            agents.Clear();
        }
    }
}