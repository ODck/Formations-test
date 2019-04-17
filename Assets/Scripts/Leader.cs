using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Leader : MonoBehaviour
{
    private const float UnitsSize = 1;
    public GameObject enemy;
    public GameObject followers;
    private List<Unit> followersUnits;
    private bool guard = true;
    public float mapWidth = 5, mapHeight = 10, attackRange = 1.5F;

    /// <summary>
    /// Assign positions for every unit behind the player
    /// </summary>
    private void FollowersPositions()
    {
        var childCount = followers.transform.childCount;

        var i = (float) Math.Floor(-childCount / 2F);
        i += childCount % 2 == 0 ? 0.5F : 1;
        var positions = new List<Vector3>();

        foreach (var follow in followersUnits)
        {
            var transform1 = transform;
            var pos = transform1.position - transform1.forward * 1.5F + transform1.right * i;
            //Clamp the limits of the map.
            pos = new Vector3(
                Mathf.Clamp(pos.x, -mapWidth, mapWidth),
                pos.y,
                Mathf.Clamp(pos.z, -mapHeight, mapHeight)
            );
            positions.Add(pos);
            i++;
            
        }

        AssignPositions(followersUnits, positions);
    }

    private void Followers2Positions()
    {
        var childCount = followers.transform.childCount;
        var positions = new List<Vector3>();
        
        var lines = Mathf.Floor(childCount/5F);
        var last = childCount % 5;
        var actualLine = 1;
        var actualUnit = 0;
        for (var i = 0; i < lines; i++)
        {
            for (var j = 0; j < 5; j++)
            {
                var distance = -2 + j;
                var transform1 = transform;
                var pos = transform1.position - transform1.forward * actualLine * 1.5F + transform1.right * (float) distance;
                pos = new Vector3(
                    Mathf.Clamp(pos.x, -mapWidth, mapWidth),
                    pos.y,
                    Mathf.Clamp(pos.z, -mapHeight, mapHeight)
                );
                positions.Add(pos);
            }
            actualLine++;
        }
        
        
        var dist = (float) Math.Floor(-last / 2F);
        dist += last % 2 == 0 ? 0.5F : 1;
        for (var j = 0; j < last; j++)
        {
            var transform1 = transform;
            var pos = transform1.position - transform1.forward * actualLine * 1.5F + transform1.right * (dist+j);
            pos = new Vector3(
                Mathf.Clamp(pos.x, -mapWidth, mapWidth),
                pos.y,
                Mathf.Clamp(pos.z, -mapHeight, mapHeight)
            );
            positions.Add(pos);
            actualUnit++;
        }
        AssignPositions(followersUnits, positions);
        
    }

    private void Attack3Positions()
    {
        var circumference = 2 * Math.PI * attackRange;
        var pointsNum = Math.Floor(circumference / UnitsSize);
        if (pointsNum < followersUnits.Count) pointsNum = followersUnits.Count;
        var enemyPosition = enemy.transform.position;

        var distanceX = Mathf.Sign(enemyPosition.x) * (mapWidth - Math.Abs(enemyPosition.x));
        var distanceY = Mathf.Sign(enemyPosition.z) * (mapHeight - Math.Abs(enemyPosition.z));

        var positions = new List<Vector3>();

        //Option 1: Hero away from the walls
        if (Math.Abs(enemyPosition.x) < mapWidth - attackRange &&
            Math.Abs(enemyPosition.z) < mapHeight - attackRange)
            for (var i = 0; i < pointsNum; i++)
                positions.Add(new Vector3(
                    (float) (enemyPosition.x +
                             attackRange * Math.Cos(2 * Math.PI * i / pointsNum)),
                    0,
                    (float) (enemyPosition.z +
                             attackRange * (Math.Sin(2 * Math.PI * i / pointsNum)))));
        //Option 2: Close to a corner
        else if (Math.Abs(enemyPosition.x) > (mapWidth - attackRange) &&
                 Math.Abs(enemyPosition.z) > (mapHeight - attackRange))
        {
            //Angles alpha and beta
            var asinA = Math.Asin(Math.Abs(distanceX / attackRange));
            var asinB = Math.Asin(Math.Abs(distanceY / attackRange));
            var wallAngle = (Math.PI / 2) + asinA + asinB;
            for (var i = 0; i < pointsNum; i++)
            {
                //We get the points of the arc.
                var vector3 = new Vector3(
                    (float) (enemyPosition.x +
                             attackRange * (Math.Cos(wallAngle * i / pointsNum)) *
                             (Mathf.Sign(enemyPosition.x) * -1)),
                    0,
                    (float) (enemyPosition.z +
                             attackRange * (Math.Sin(wallAngle * i / pointsNum)) *
                             (Mathf.Sign(enemyPosition.z) * -1)));
                //Rotate the points. To match with the intersections.
                var angleNew = Mathf.Atan(distanceY / attackRange) * Mathf.Sign(enemyPosition.z);
                angleNew = -angleNew * Mathf.Sign(enemyPosition.x) * Mathf.Sign(enemyPosition.z);
                vector3 = RotateVectorWithAngleAroundPoint(vector3, angleNew, enemyPosition);
                positions.Add(vector3);
            }
        }
        //Option 3: Close to top or bot and away from sides.
        else if (Math.Abs(enemyPosition.x) <= (mapWidth - attackRange) &&
                 Math.Abs(enemyPosition.z) > (mapHeight - attackRange))
        {
            var asinA = Math.Asin(Math.Abs(distanceY / attackRange));
            var wallAngle = Math.PI + asinA * 2;

            for (var i = 0; i < pointsNum; i++)
            {
                //We get the points of the arc.
                var vector3 = new Vector3(
                    (float) (enemyPosition.x +
                             attackRange * (Math.Cos(wallAngle * i / pointsNum))),
                    0,
                    (float) (enemyPosition.z +
                             attackRange * (Math.Sin(wallAngle * i / pointsNum)) *
                             (Mathf.Sign(enemyPosition.z) * -1)));
                //Rotate the points. To match with the intersections.
                var angleNew = Mathf.Atan(distanceY / attackRange);
                vector3 = RotateVectorWithAngleAroundPoint(vector3, angleNew, enemyPosition);
                positions.Add(vector3);
            }
        }
        //Option 4: Close to sides and away top or bot.
        else if (Math.Abs(enemyPosition.x) > (mapWidth - attackRange) &&
                 Math.Abs(enemyPosition.z) <= (mapHeight - attackRange))
        {
            var asinA = Math.Asin(Math.Abs(distanceX / attackRange));
            var wallAngle = Math.PI + asinA * 2;

            for (var i = 0; i < pointsNum; i++)
            {
                //We get the points of the arc.
                var vector3 = new Vector3(
                    (float) (enemyPosition.x +
                             attackRange * (Math.Cos(wallAngle * i / pointsNum)) *
                             (Mathf.Sign(enemyPosition.x) * -1)),
                    0,
                    (float) (enemyPosition.z +
                             attackRange * (Math.Sin(wallAngle * i / pointsNum))));
                //Rotate the points. To match with the intersections.
                var angleNew = Mathf.Atan(distanceX / attackRange);// * Mathf.Sign(enemyPosition.x);
                angleNew = (float) (angleNew + Math.PI / 2 * Mathf.Sign(enemyPosition.x));
                vector3 = RotateVectorWithAngleAroundPoint(vector3, angleNew, enemyPosition);      
                positions.Add(vector3);
            }
        }
        else
        {
            Debug.Log(Math.Abs(enemyPosition.x) + " x vs " + (mapWidth - attackRange) + " " +
                      Math.Abs(enemyPosition.z) + " z " + (mapHeight - attackRange));
            throw new NotSupportedException();
        }
        AssignPositions(followersUnits, positions);
    }

    /// <summary>
    /// Rotate a point around another with the given angle
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

    /// <summary>
    /// Each unit select the closer position in order.
    /// </summary>
    /// <param name="allUnits"></param>
    /// <param name="allPositions"></param>
    private static void AssignPositions(IEnumerable<Unit> allUnits, ICollection<Vector3> allPositions)
    {
        foreach (var follow in allUnits)
        {
            var closerPos =
                (from pos in allPositions
                    orderby Vector3.Distance(follow.Position, pos)
                    select pos).First();

            allPositions.Remove(closerPos);
            follow.targetPos = closerPos;
        }
    }

    private void Update()
    {
        //this line is inefficient, but is a cheap implementation to find each frame the units following
        followersUnits = followers.GetComponentsInChildren<Unit>().ToList();
        if (guard)
            Followers2Positions();
        else
            Attack3Positions();
        //Left click attack, right defend
        if (Input.GetKey(KeyCode.Mouse0))
            guard = true;
        else if (Input.GetKey(KeyCode.Mouse1))
            guard = false;
    }
}