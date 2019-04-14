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

    private void FollowersPositions()
    {
        var childCount = followers.transform.childCount;

        var i = (float) Math.Floor(-childCount / 2F);
        i += childCount % 2 == 0 ? 0.5F : 1;
        var positions = new List<Vector3>();
        foreach (var follow in followersUnits)
        {
            var transform1 = transform;
            var pos = transform1.position - transform1.forward * 3 + transform1.right * (i * 2);
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

    private void Attack2Positions()
    {
        var circumference = 2 * Math.PI * attackRange;
        var pointsNum = Math.Floor(circumference / UnitsSize);
        if (pointsNum < followersUnits.Count) pointsNum = followersUnits.Count;
        var enemyPosition = enemy.transform.position;

        var positions = new List<Vector3>();
        for (var i = 0; i < pointsNum; i++)
            positions.Add(new Vector3((float) (enemyPosition.x + attackRange * Math.Cos(-Math.PI * i / pointsNum)), 0,
                (float) (enemyPosition.z + attackRange * Math.Sin(-Math.PI * i / pointsNum))));
        AssignPositions(followersUnits, positions);
    }

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
        followersUnits = followers.GetComponentsInChildren<Unit>().ToList();
        if (guard)
            FollowersPositions();
        else
            Attack2Positions();

        if (Input.GetKey(KeyCode.Mouse0))
            guard = true;
        else if (Input.GetKey(KeyCode.Mouse1))
            guard = false;
    }
}