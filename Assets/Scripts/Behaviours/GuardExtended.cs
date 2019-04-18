using System;
using System.Collections.Generic;
using Settings;
using UnityEngine;

namespace Behaviours
{
    public class GuardExtended : IUnitsBehaviour
    {
        /// <inheritdoc />
        public List<Vector3> CalculatePositions(Leader allLeader)
        {
            var childCount = allLeader.followers.transform.childCount;
            var positions = new List<Vector3>();

            var lines = Mathf.Floor(childCount / 5F);
            var last = childCount % 5;
            var actualLine = 1;
            float dist = -2;
            for (var i = 0; i < lines; i++)
            {
                for (var j = 0; j < 5; j++)
                {
                    var pos = GeneratePoint(j);
                    positions.Add(pos);
                }

                actualLine++;
            }


            dist = (float) Math.Floor(-last / 2F);
            dist += last % 2 == 0 ? 0.5F : 1;
            for (var j = 0; j < last; j++)
            {
                var pos = GeneratePoint(j);
                positions.Add(pos);
            }

            return positions;

            Vector3 GeneratePoint(int j)
            {
                var transform1 = allLeader.transform;
                var pos = transform1.position - transform1.forward * actualLine * 1.5F + transform1.right * (dist + j);
                pos = new Vector3(
                    Mathf.Clamp(pos.x, -SettingsLoader.MapWidth, SettingsLoader.MapWidth),
                    pos.y,
                    Mathf.Clamp(pos.z, -SettingsLoader.MapHeight, SettingsLoader.MapHeight)
                );
                return pos;
            }
        }
    }
}