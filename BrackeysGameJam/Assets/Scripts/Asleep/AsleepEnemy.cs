using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class AsleepEnemy : MonoBehaviour
{
    public float roamSpeed;
    public float chaseSpeed;
    public float viewDistance;
    public float killDistance;
    public int enemyID;

    public List<int> allowedTargetNodes;

    private Maze maze;
    private bool isTrapped;

    private GameObject player;
    private NavMeshAgent navMeshAgent;
    private Collider playerCollider;
    private Collider enemyCollider;
    private MeshRenderer meshRenderer;
    private AudioSource audioSource;

    public static event Action<int, AudioSource> onPlayerSeen;

    private void Stop() => SetStopped(true);

    private void Trap(GameObject enemy)
    {
        if (enemy.GetComponent<AsleepEnemy>().enemyID != enemyID) return;

        enemyCollider.enabled = false;
        navMeshAgent.isStopped = true;
        isTrapped = true;
    }

    private void SetStopped(bool frozen)
    {
        if (!isTrapped || frozen)
        {
            navMeshAgent.isStopped = frozen;
        }
    }

    private void SetMaze(Maze newMaze)
    {
        maze = newMaze;
    }

    private void OnEnable()
    {
        AsleepLucidControl.onLucidToggled += SetStopped;
        AsleepPlayerControl.onPlayerKilled += Stop;
        AsleepTrap.onEnemyTrapped += Trap;
        MazeGenerator.onMazeGenerated += SetMaze;
    }

    private void OnDisable()
    {
        AsleepLucidControl.onLucidToggled -= SetStopped;
        AsleepPlayerControl.onPlayerKilled -= Stop;
        AsleepTrap.onEnemyTrapped -= Trap;
        MazeGenerator.onMazeGenerated -= SetMaze;
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        playerCollider = player.GetComponent<Collider>();
        enemyCollider = GetComponent<Collider>();
        meshRenderer = GetComponent<MeshRenderer>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();

        navMeshAgent.speed = roamSpeed;
    }

    private void FixedUpdate()
    {
        if (navMeshAgent.isStopped) return;

        Vector3 rayDirection = player.transform.position - transform.position;
        Vector3 rayOrigin = transform.position;

        rayDirection.Normalize();
        rayDirection *= viewDistance;

        rayOrigin.y = 1f;
        rayDirection.y = 0f;

        Debug.DrawRay(rayOrigin, rayDirection, Color.green);

        Physics.Raycast(rayOrigin, rayDirection, out RaycastHit lineOfSightRay, maxDistance: viewDistance);
        bool playerSeen = (lineOfSightRay.collider == playerCollider);

        if (playerSeen)
        {
            if (lineOfSightRay.distance <= killDistance)
            {
                AsleepPlayerControl.killPlayer();
            }

            onPlayerSeen?.Invoke(enemyID, audioSource);

            meshRenderer.material.color = Color.red;
            navMeshAgent.speed = chaseSpeed;
            navMeshAgent.SetDestination(player.transform.position);
        }
        else
        {
            meshRenderer.material.color = Color.blue;
            navMeshAgent.speed = roamSpeed;
        }

        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            int targetNodeIndex = allowedTargetNodes[Random.Range(0, allowedTargetNodes.Count)];

            int row = targetNodeIndex / maze.size;
            int col = targetNodeIndex % maze.size;

            navMeshAgent.SetDestination(new Vector3(col * maze.scale.x, 0, -row * maze.scale.z));
        }
    }

    public void SetAllowedTargetNodes(List<int> targetNodes)
    {
        allowedTargetNodes = targetNodes;
    }
}
