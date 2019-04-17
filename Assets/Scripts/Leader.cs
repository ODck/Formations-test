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

    /// <summary>
    /// Assign positions around the enemy
    /// </summary>
    private void Attack2Positions()
    {
        var circumference = 2 * Math.PI * attackRange;
        var pointsNum = Math.Floor(circumference / UnitsSize);
        if (pointsNum < followersUnits.Count) pointsNum = followersUnits.Count;
        var enemyPosition = enemy.transform.position;
        var angle = FindAngle();
        var positions = new List<Vector3>();
        for (var i = 0; i < pointsNum; i++)
            positions.Add(new Vector3(
                (float) (enemyPosition.x +
                         attackRange * (Math.Cos(angle * i / pointsNum)) * (Mathf.Sign(enemyPosition.x) * -1)),
                0,
                (float) (enemyPosition.z +
                         attackRange * (Math.Sin(angle * i / pointsNum)) * (Mathf.Sign(enemyPosition.z) * -1))));
        AssignPositions(followersUnits, positions);
    }

    private double FindAngle()
    {
        var position = enemy.transform.position;
        var distanceX = Mathf.Sign(position.x) * (mapWidth - Math.Abs(position.x));
        var distanceY = Mathf.Sign(position.z) * (mapHeight - Math.Abs(position.z));
        //if (Math.Abs(distanceX) >= Math.Abs(mapWidth - attackRange) &&
        //    Math.Abs(distanceY) >= Math.Abs(mapWidth - attackRange))
        //    return 2 * Math.PI;
        //    return 2 * Math.PI;
        //Option 1: Hero away from the walls
        if (Math.Abs(distanceX) > Mathf.Epsilon && Math.Abs(distanceY) > Mathf.Epsilon)
            return 2 * Math.PI;
        //Option 2: Close to a side but away from top or bot.
        if (Math.Abs(distanceX) < Mathf.Epsilon && Math.Abs(distanceY) > Mathf.Epsilon)
            return Math.PI * Mathf.Sign(distanceX);
        if (Math.Abs(distanceX) > Mathf.Epsilon && Math.Abs(distanceY) < Mathf.Epsilon)
            return Math.PI * Mathf.Sign(distanceY);
        Debug.Log(Math.Abs(distanceX) + "  " + Math.Abs(distanceY));
        if (Math.Abs(distanceX) < Mathf.Epsilon && Math.Abs(distanceY) < Mathf.Epsilon)
            return Math.PI / 2;

        return 0;
//        var asinA = Math.Asin(distanceX / attackRange);
//        if (double.IsNaN(asinA)) asinA = 0;
//        var asinB = Math.Asin(distanceY / attackRange);
//        if (double.IsNaN(asinB)) asinB = 0;
//        var wallAngle = (Math.PI / 2) + asinA + asinB;
//        return 2 * Math.PI - wallAngle;
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
        if (Math.Abs(enemyPosition.x) < (mapWidth - attackRange) && Math.Abs(enemyPosition.z) < (mapHeight - attackRange))
            for (var i = 0; i < pointsNum; i++)
                positions.Add(new Vector3(
                    (float) (enemyPosition.x +
                             attackRange * (Math.Cos(2 * Math.PI * i / pointsNum))),
                    0,
                    (float) (enemyPosition.z +
                             attackRange * (Math.Sin(2 * Math.PI * i / pointsNum)))));

        //Option 2: In a side but away from top or bot.
        else if (Math.Abs(Math.Abs(enemyPosition.x) - (mapWidth - attackRange)) < Mathf.Epsilon && Math.Abs(enemyPosition.z) < (mapHeight - attackRange))
            for (var i = 0; i < pointsNum; i++)
            {
                var vector3 = new Vector3(
                    (float) (enemyPosition.x +
                             attackRange * (Math.Cos(Math.PI * i / pointsNum))),
                    0,
                    (float) (enemyPosition.z +
                             attackRange * (Math.Sin(Math.PI * i / pointsNum))));
                //Rotate the point 90 degrees
                var angleNew = Math.PI / 2 * Mathf.Sign(enemyPosition.x);
                var rotatedX = Math.Cos(angleNew) * (vector3.x - enemyPosition.x) -
                               Math.Sin(angleNew) * (vector3.z - enemyPosition.z) + enemyPosition.x;
                var rotatedY = Math.Sin(angleNew) * (vector3.x - enemyPosition.x) +
                               Math.Cos(angleNew) * (vector3.z - enemyPosition.z) + enemyPosition.z;

                vector3 = new Vector3((float) rotatedX, 0, (float) rotatedY);
                positions.Add(vector3);
            }
        //Option 3: In top or bot but away from the sides
        else if (Math.Abs(enemyPosition.x) < (mapWidth - attackRange) && Math.Abs(Math.Abs(enemyPosition.z) - (mapHeight - attackRange)) < Mathf.Epsilon)
            for (var i = 0; i < pointsNum; i++)
                positions.Add(new Vector3(
                    (float) (enemyPosition.x +
                             attackRange * (Math.Cos(Math.PI * i / pointsNum))),
                    0,
                    (float) (enemyPosition.z +
                             attackRange * (Math.Sin(Math.PI * i / pointsNum)) * (Mathf.Sign(enemyPosition.z) * -1))));
        //Option 4: In a corner
        else if (Math.Abs(Math.Abs(enemyPosition.x) - (mapWidth - attackRange)) < Mathf.Epsilon && Math.Abs(Math.Abs(enemyPosition.z) - (mapHeight - attackRange)) < Mathf.Epsilon)
            for (var i = 0; i < pointsNum; i++)
                positions.Add(new Vector3(
                    (float) (enemyPosition.x +
                             attackRange * (Math.Cos(Math.PI / 2 * i / pointsNum)) *
                             (Mathf.Sign(enemyPosition.x) * -1)),
                    0,
                    (float) (enemyPosition.z +
                             attackRange * (Math.Sin(Math.PI / 2 * i / pointsNum)) *
                             (Mathf.Sign(enemyPosition.z) * -1))));
        //Option 5: Close to a corner
        else if (Math.Abs(enemyPosition.x) > (mapWidth - attackRange) && Math.Abs(enemyPosition.z) > (mapHeight - attackRange))
        {
            //Angles alpha and beta
            var asinA = Math.Asin(Math.Abs(distanceX / attackRange));
            var asinB = Math.Asin(Math.Abs(distanceY / attackRange));
            var wallAngle =(Math.PI / 2) + asinA + asinB;
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
                
                var rotatedX = Math.Cos(angleNew) * (vector3.x - enemyPosition.x) -
                               Math.Sin(angleNew) * (vector3.z - enemyPosition.z) + enemyPosition.x;
                var rotatedY = Math.Sin(angleNew) * (vector3.x - enemyPosition.x) +
                               Math.Cos(angleNew) * (vector3.z - enemyPosition.z) + enemyPosition.z;
                vector3 = new Vector3((float) rotatedX, 0, (float) rotatedY);
                positions.Add(vector3);
            }
        }
        else
        {
            Debug.Log(Math.Abs(enemyPosition.x) + " x vs " + (mapWidth-attackRange) + " " + Math.Abs(enemyPosition.z) + " z " +(mapHeight - attackRange));
        }

        AssignPositions(followersUnits, positions);
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
            FollowersPositions();
        else
            Attack3Positions();
        //Left click attack, right defend
        if (Input.GetKey(KeyCode.Mouse0))
            guard = true;
        else if (Input.GetKey(KeyCode.Mouse1))
            guard = false;
    }
}