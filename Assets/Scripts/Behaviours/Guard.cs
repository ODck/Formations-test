using System.Collections.Generic;
using Settings;
using UnityEngine;

namespace Behaviours
{
    /// <inheritdoc />
    internal class Guard : IUnitsBehaviour
    {
        /// <inheritdoc />
        public List<Vector3> CalculatePositions(Leader allLeader)
        {
            var childCount = allLeader.followers.transform.childCount;
            var positions = new List<Vector3>();
            float starting;
            float dis;
            if (childCount <= 5)
            {
                IncompleteLine(childCount, 1);
            }
            else if (childCount <= 10)
            {
                IncompleteLine(5, 1);
                IncompleteLine(childCount - 5, 2);
            }
            else
            {
                starting = -2;
                dis = 5F / Mathf.Ceil(childCount / 2F);
                for (var i = 0; i < Mathf.Ceil(childCount / 2F); i++) GeneratePoint(dis, 1, starting, i);

                dis = 5F / Mathf.Floor(childCount / 2F);
                for (var i = 0; i < Mathf.Floor(childCount / 2F); i++) GeneratePoint(dis, 2, starting, i);
            }

            return positions;


            void IncompleteLine(int unitCount, int laneNumber)
            {
                starting = Mathf.Floor(-unitCount / 2F);
                starting += unitCount % 2 == 0 ? 0.5F : 1F;
                dis = 1F;
                for (var i = 0; i < childCount; i++) GeneratePoint(dis, laneNumber, starting, i);
            }

            void GeneratePoint(float d, int line, float startPoint, int unit)
            {
                var distance = startPoint + unit * d;
                var transform1 = allLeader.transform;
                var pos = transform1.position - transform1.forward * line * 1.5F +
                          transform1.right * distance;
                pos = new Vector3(
                    Mathf.Clamp(pos.x, -SettingsLoader.MapWidth, SettingsLoader.MapWidth),
                    pos.y,
                    Mathf.Clamp(pos.z, -SettingsLoader.MapHeight, SettingsLoader.MapHeight)
                );
                positions.Add(pos);
            }
        }
    }
}