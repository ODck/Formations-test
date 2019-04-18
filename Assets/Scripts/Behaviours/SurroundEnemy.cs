using System;
using System.Collections.Generic;
using Settings;
using UnityEngine;

namespace Behaviours
{
    public class SurroundEnemy : IUnitsBehaviour
    {
        /// <inheritdoc />
        public List<Vector3> CalculatePositions(Leader allLeader)
        {
            var circumference = 2 * Math.PI * allLeader.attackRange;
            var pointsNum = Math.Floor(circumference / Leader.UnitsSize);
            if (pointsNum < allLeader.FollowersUnits.Count) pointsNum = allLeader.FollowersUnits.Count;
            var enemyPosition = allLeader.enemy.transform.position;

            var distanceX = Mathf.Sign(enemyPosition.x) * (SettingsLoader.MapWidth - Math.Abs(enemyPosition.x));
            var distanceY = Mathf.Sign(enemyPosition.z) * (SettingsLoader.MapHeight - Math.Abs(enemyPosition.z));

            var positions = new List<Vector3>();

            //Option 1: Hero away from the walls
            if (Math.Abs(enemyPosition.x) < SettingsLoader.MapWidth - allLeader.attackRange &&
                Math.Abs(enemyPosition.z) < SettingsLoader.MapHeight - allLeader.attackRange)
            {
                for (var i = 0; i < pointsNum; i++)
                    positions.Add(new Vector3(
                        (float) (enemyPosition.x + allLeader.attackRange * Math.Cos(2 * Math.PI * i / pointsNum)),
                        0,
                        (float) (enemyPosition.z + allLeader.attackRange * Math.Sin(2 * Math.PI * i / pointsNum))));
            }
            //Option 2: Close to a corner
            else if (Math.Abs(enemyPosition.x) > SettingsLoader.MapWidth - allLeader.attackRange &&
                     Math.Abs(enemyPosition.z) > SettingsLoader.MapHeight - allLeader.attackRange)
            {
                //Angles alpha and beta
                var asinA = Math.Asin(Math.Abs(distanceX / allLeader.attackRange));
                var asinB = Math.Asin(Math.Abs(distanceY / allLeader.attackRange));
                var wallAngle = Math.PI / 2 + asinA + asinB;
                for (var i = 0; i < pointsNum; i++)
                {
                    //We get the points of the arc.
                    var vector3 = new Vector3(
                        (float) (enemyPosition.x + allLeader.attackRange * Math.Cos(wallAngle * i / pointsNum) *
                                 (Mathf.Sign(enemyPosition.x) * -1)),
                        0,
                        (float) (enemyPosition.z + allLeader.attackRange * Math.Sin(wallAngle * i / pointsNum) *
                                 (Mathf.Sign(enemyPosition.z) * -1)));
                    //Rotate the points. To match with the intersections.
                    var angleNew = Mathf.Atan(distanceY / allLeader.attackRange) * Mathf.Sign(enemyPosition.z);
                    angleNew = -angleNew * Mathf.Sign(enemyPosition.x) * Mathf.Sign(enemyPosition.z);
                    vector3 = RotateVectorWithAngleAroundPoint(vector3, angleNew, enemyPosition);
                    positions.Add(vector3);
                }
            }
            //Option 3: Close to top or bot and away from sides.
            else if (Math.Abs(enemyPosition.x) <= SettingsLoader.MapWidth - allLeader.attackRange &&
                     Math.Abs(enemyPosition.z) > SettingsLoader.MapHeight - allLeader.attackRange)
            {
                var asinA = Math.Asin(Math.Abs(distanceY / allLeader.attackRange));
                var wallAngle = Math.PI + asinA * 2;

                for (var i = 0; i < pointsNum; i++)
                {
                    //We get the points of the arc.
                    var vector3 = new Vector3(
                        (float) (enemyPosition.x + allLeader.attackRange * Math.Cos(wallAngle * i / pointsNum)),
                        0,
                        (float) (enemyPosition.z + allLeader.attackRange * Math.Sin(wallAngle * i / pointsNum) *
                                 (Mathf.Sign(enemyPosition.z) * -1)));
                    //Rotate the points. To match with the intersections.
                    var angleNew = Mathf.Atan(distanceY / allLeader.attackRange);
                    vector3 = RotateVectorWithAngleAroundPoint(vector3, angleNew, enemyPosition);
                    positions.Add(vector3);
                }
            }
            //Option 4: Close to sides and away top or bot.
            else if (Math.Abs(enemyPosition.x) > SettingsLoader.MapWidth - allLeader.attackRange &&
                     Math.Abs(enemyPosition.z) <= SettingsLoader.MapHeight - allLeader.attackRange)
            {
                var asinA = Math.Asin(Math.Abs(distanceX / allLeader.attackRange));
                var wallAngle = Math.PI + asinA * 2;

                for (var i = 0; i < pointsNum; i++)
                {
                    //We get the points of the arc.
                    var vector3 = new Vector3(
                        (float) (enemyPosition.x + allLeader.attackRange * Math.Cos(wallAngle * i / pointsNum) *
                                 (Mathf.Sign(enemyPosition.x) * -1)),
                        0,
                        (float) (enemyPosition.z + allLeader.attackRange * Math.Sin(wallAngle * i / pointsNum)));
                    //Rotate the points. To match with the intersections.
                    var angleNew = Mathf.Atan(distanceX / allLeader.attackRange);
                    angleNew = (float) (angleNew + Math.PI / 2 * Mathf.Sign(enemyPosition.x));
                    vector3 = RotateVectorWithAngleAroundPoint(vector3, angleNew, enemyPosition);
                    positions.Add(vector3);
                }
            }
            else
            {
                Debug.Log(Math.Abs(enemyPosition.x) + " x vs " + (SettingsLoader.MapWidth - allLeader.attackRange) +
                          " " +
                          Math.Abs(enemyPosition.z) + " z " + (SettingsLoader.MapHeight - allLeader.attackRange));
                throw new NotSupportedException();
            }

            return positions;
        }

        /// <summary>
        ///     Rotate a point around another with the given angle
        /// </summary>
        /// <param name="vector3">Point to move</param>
        /// <param name="angleNew">Angle</param>
        /// <param name="enemyPosition"></param>
        /// <returns></returns>
        private static Vector3 RotateVectorWithAngleAroundPoint(Vector3 vector3, float angleNew, Vector3 enemyPosition)
        {
            var rotatedX = Math.Cos(angleNew) * (vector3.x - enemyPosition.x) -
                           Math.Sin(angleNew) * (vector3.z - enemyPosition.z) + enemyPosition.x;
            var rotatedY = Math.Sin(angleNew) * (vector3.x - enemyPosition.x) +
                           Math.Cos(angleNew) * (vector3.z - enemyPosition.z) + enemyPosition.z;
            vector3 = new Vector3((float) rotatedX, 0, (float) rotatedY);
            return vector3;
        }
    }
}