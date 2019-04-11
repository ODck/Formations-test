using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Leader : MonoBehaviour
{
    [SerializeField]
    private float mapWidth = 5, mapHeight = 15, attackRange = 1.5F;
    private readonly float unitsSize = 1;
    public GameObject followers;
    public GameObject enemy;
    private bool guard = true;
    private List<Unit> followersUnits;

    private void FollowersPositions()
    {
        var i = (float)Math.Floor(-followers.transform.childCount / 2F);
        i += followers.transform.childCount % 2 == 0 ? 0.5F : 1;

        var allPositions = new Dictionary<Vector3, bool>();
        foreach (var follow in followersUnits)
        {
            allPositions.Add(transform.position - (transform.forward * 3) + (transform.right * (i * 2)), true);
            i++;
        }

        AsignPositions(followersUnits, allPositions);
    }

    private List<Unit> OrderUnits(Vector3 target)
    {
        return (from unit in followersUnits
                orderby Vector3.Distance(unit.Position, target)
                select unit).ToList();
    }

    private void Attack2Positions()
    {
        //if (attackRange <= Mathf.Epsilon) attackRange = 0.1F;
        double circumference = 2 * Math.PI * attackRange;
        double pointsNum = Math.Floor(circumference / unitsSize);
        if (pointsNum < followersUnits.Count) pointsNum = followersUnits.Count;
        var enemyPosition = enemy.transform.position;

        var allPositions = new Dictionary<Vector3, bool>();
        for (var i = 0; i < pointsNum; i++)
        {
            allPositions.Add(new Vector3((float)(enemyPosition.x + attackRange * Math.Cos(2 * Math.PI * i / pointsNum)), 0,
                (float)(enemyPosition.z + attackRange * Math.Sin(2 * Math.PI * i / pointsNum))), true);
        }
        var allUnits = OrderUnits(enemyPosition);
        AsignPositions(followersUnits, allPositions);
    }

    private void AsignPositions(List<Unit> allUnits, Dictionary<Vector3, bool> allPositions)
    {
        foreach (var follow in allUnits)
        {
            var closerPos =
                (from pos in allPositions
                 where pos.Value == true
                 orderby Vector3.Distance(follow.Position, pos.Key)
                 select pos.Key).First();

            allPositions[closerPos] = false;
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