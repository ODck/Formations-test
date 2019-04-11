using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

public class Unit : MonoBehaviour
{
    public Vector3 Position
    {
        get => transform.position;
        set => transform.position = value;
    }

    public bool guard;
    public bool attacking;
    public Vector3 targetPos;
    public Vector3 leaderPos;

    public float velocity = 4;

    private Controller _controller;
    public Color color;

    private void Start()
    {
        _controller = FindObjectOfType<Controller>();
        GetComponent<MeshRenderer>().material.color = color;
    }

    private void Update()
    {
        GuardMovement();
//        if (guard)
//            GuardMovement();
//        else
//            AttackMovement();
    }

    private void GuardMovement()
    {
        const double guardRange = 0.1;
        leaderPos = targetPos;
        //leaderPos = leader.transform.position;
        var totalDistance = Vector3.Distance(Position, leaderPos);
        if (totalDistance > guardRange)
        {
            Move();
            //transform.Trans//(delta,tickDistance);
        }
    }

    private void Move()
    {
        var tickDistance = velocity * Time.deltaTime;
        var delta = Position - leaderPos;
        delta = delta.normalized;
        Position -= delta * tickDistance;
    }

    private void AttackMovement()
    {
        leaderPos = targetPos;
        var followers = transform.parent;
        var radius = 3;
        var direction = leaderPos - Position;
        var normalized = direction.normalized;
        direction = direction - (normalized * radius);
        leaderPos = Position + direction;
        if (Vector3.Distance(Position, leaderPos) > radius + 0.1F)
        {
            Move();
            attacking = false;
        }
        else
        {
            Debug.Log(Vector3.Distance(direction, normalized * radius));
            const float d = 2;
            var list = 
                (from follow in followers.GetComponentsInChildren<Unit>() 
                    where follow != this where follow.attacking == true 
                    where Vector3.Distance(follow.Position, Position) < d 
                    select follow).ToList();
            if (list.Count == 0)
                goto End;
            
            
            End:
                Debug.Log(color.b + " " + leaderPos);
                Move();
                attacking = true;
        }


        return;
        var count = followers.childCount;
        var i = 0;
        foreach (var follow in followers.GetComponentsInChildren<Unit>())
        {
            if (follow == this) continue;
            var v2 = new Vector3((float) (leaderPos.x + radius * Math.Cos(2 * Math.PI * i / count)), 0,
                (float) (leaderPos.z + radius * Math.Sin(2 * Math.PI * i / count)));
            i++;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawSphere(leaderPos, 0.5F);
    }
}