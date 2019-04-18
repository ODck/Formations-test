using System.Collections.Generic;
using System.Linq;
using Behaviours;
using UnityEngine;

public class Leader : MonoBehaviour
{
    public const float UnitsSize = 1;
    private readonly IUnitsBehaviour attackBehaviour = new SurroundEnemy();

    private readonly IUnitsBehaviour guardBehaviour = new Guard();
    public float attackRange = 1.5F;
    public GameObject enemy;
    public GameObject followers;
    private bool guard = true;

    public List<Unit> FollowersUnits { get; private set; }


    /// <summary>
    ///     Each unit select the closer position in order.
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
        FollowersUnits = followers.GetComponentsInChildren<Unit>().ToList();
        var positions = guard ? guardBehaviour.CalculatePositions(this) : attackBehaviour.CalculatePositions(this);
        AssignPositions(FollowersUnits, positions);
        //Left click attack, right defend
        if (Input.GetKey(KeyCode.Mouse0))
            guard = true;
        else if (Input.GetKey(KeyCode.Mouse1))
            guard = false;
    }
}