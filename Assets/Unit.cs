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
    [HideInInspector]
    public Vector3 targetPos;
    [HideInInspector]
    public Vector3 leaderPos;   

    public float velocity = 4;
    public Color color;

    private void Start()
    {
        GetComponent<MeshRenderer>().material.color = color;
    }

    private void Update()
    {
        FollowOrder();
    }

    private void FollowOrder()
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

    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawSphere(leaderPos, 0.5F);
    }
}