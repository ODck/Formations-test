using UnityEngine;

public class Unit : MonoBehaviour
{
    public Color color;

    [HideInInspector] public Vector3 leaderPos;

    [HideInInspector] public Vector3 targetPos;

    public float velocity = 4;

    public Vector3 Position
    {
        get => transform.position;
        private set => transform.position = value;
    }

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
        if (totalDistance > guardRange) Move();
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