using UnityEngine;

namespace Settings
{
    [CreateAssetMenu(fileName = "Settings", menuName = "Settings")]
    public class Settings : ScriptableObject
    {
        public float mapWidth, mapHeight;
    }
}