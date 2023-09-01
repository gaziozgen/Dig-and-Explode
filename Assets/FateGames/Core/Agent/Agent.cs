using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using FateGames.Core;
using System;

[DisallowMultipleComponent]
public class Agent : FateMonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] float speed = 3.5f, angularSpeed = 360, radius = 0.5f;
    [SerializeField] ObstacleAvoidanceType obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
    NavMeshAgent navMeshAgent;
    event Action OnReachedToDestination;
    public bool GoingToDestination { get; private set; } = false;
    Vector3 destination = Vector3.zero;
    public float Speed => navMeshAgent.speed;

    private void Awake()
    {
        InitializeNavMeshAgent();
    }

    private void Start()
    {
        InvokeRepeating(nameof(CheckReached), 0, 0.15f);
    }

    void InitializeNavMeshAgent()
    {
        navMeshAgent = gameObject.AddComponent<NavMeshAgent>();
        navMeshAgent.speed = speed;
        navMeshAgent.angularSpeed = angularSpeed;
        navMeshAgent.obstacleAvoidanceType = obstacleAvoidanceType;
        navMeshAgent.radius = 0.5f;
        navMeshAgent.hideFlags = HideFlags.HideInInspector;
    }

    public void Disable()
    {
        navMeshAgent.enabled = false;
    }
    public void Enable()
    {
        navMeshAgent.enabled = true;
    }

    public void Warp(Vector3 position)
    {
        navMeshAgent.Warp(position);
    }

    public void SetAnimatorMovingSpeed(float movingSpeed)
    {
        if (animator)
            animator.SetFloat("Speed", movingSpeed);
    }

    public void GoToExactPoint(Vector3 position, Action onReached = null)
    {
        GoTo(position, 0.04f, onReached);
    }

    public void GoToClosestPoint(Vector3 position, float maxDistance = Mathf.Infinity, Action onReached = null)
    {
        GoTo(position, maxDistance, onReached);
    }

    void GoTo(Vector3 position, float maxDistance, Action onReached = null)
    {
        if (!NavMesh.SamplePosition(position, out NavMeshHit hit, maxDistance, NavMesh.AllAreas))
        {
            Debug.LogError("Cannot go to position! " + position, this);
            return;
        }
        Enable();
        Stop();
        navMeshAgent.isStopped = false;
        OnReachedToDestination = () => { };
        if (onReached != null)
        {
            OnReachedToDestination += onReached;
        }
        navMeshAgent.SetDestination(hit.position);
        GoingToDestination = true;
        destination = position;
        SetAnimatorMovingSpeed(navMeshAgent.speed);
    }

    public void SetSpeed(float speed)
    {
        navMeshAgent.speed = speed;
    }

   

    public void Stop()
    {
        navMeshAgent.isStopped = true;
        GoingToDestination = false;
        OnReachedToDestination = () => { };
        destination = Vector3.zero;
        SetAnimatorMovingSpeed(0);
    }

    void CheckReached()
    {
        if (!GoingToDestination) return;
        if (navMeshAgent.enabled && !navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance && !navMeshAgent.hasPath && navMeshAgent.velocity.sqrMagnitude == 0f)
        {
            OnReachedToDestination();
            Stop();
        }
    }

}
