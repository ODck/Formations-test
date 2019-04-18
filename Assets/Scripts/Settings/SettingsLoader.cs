using UnityEngine;

namespace Settings
{
    public class SettingsLoader : MonoBehaviour
    {
        [SerializeField] private Settings settings;
        public static float MapWidth { get; private set; }

        public static float MapHeight { get; private set; }

        private void Start()
        {
            MapWidth = settings.mapWidth;
            MapHeight = settings.mapHeight;
        }
    }
}