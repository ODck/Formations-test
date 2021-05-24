using System;
using Dck.Pathfinder;
using Newtonsoft.Json;
using Settings;
using UnityEngine;

namespace UnityLibrary
{
    public class DrawMesh : MonoBehaviour
    {
        public GameMap gameMap;
        public bool color;

        public void Init(uint blocks)
        {
            gameMap = new GameMap((uint) SettingsLoader.MapWidth, (uint) SettingsLoader.MapHeight);
            gameMap.FillWithRandomBlockType(blocks, MapCellType.Wall);
            DrawCells();
            PhysicsWorld.Init(gameMap);
            //DrawConnections();
        }

        private void DrawCells()
        {
            for (var i = 0u; i < gameMap.Width; i++)
            {
                for (var j = 0u; j < gameMap.Height; j++)
                {
                    var cellPosition = gameMap.GetWorldPositionFromCell(i, j);
                    DrawCell(cellPosition.X, cellPosition.Y, i, j);
                    color = !color;
                }
                color = !color;
            }
        }

        private void DrawCell(float posX, float posY, uint nodePosX, uint nodePosY)
        {
            var plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            plane.transform.SetParent(transform);
            plane.transform.localPosition = new Vector3(posX, 0, posY);
            plane.transform.localScale = Vector3.one * 0.1F;
            plane.GetComponent<Renderer>().material.color = color ? Color.gray : Color.white;
            if (gameMap.GetCellAt(nodePosX, nodePosY) == MapCellType.Wall)
            {
                plane.GetComponent<Renderer>().material.color = Color.black;
            }
        }

        private void Update()
        {
            //Debug.Log(PhysicsWorld.World.BodyCount);
        }
    }
}