using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class AsleepEnemy : MonoBehaviour
{
    public float roamSpeed;
    public float chaseSpeed;
    public float viewDistance;

    private Maze maze;

    private GameObject player;
    private NavMeshAgent navMeshAgent;
    // private bool chasingPlayer = false;
    private Collider playerCollider;
    private MeshRenderer meshRenderer;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        playerCollider = player.GetComponent<Collider>();
        meshRenderer = GetComponent<MeshRenderer>();
        navMeshAgent = GetComponent<NavMeshAgent>();

        maze = MazeGenerator.maze_2;
        navMeshAgent.speed = roamSpeed;
    }

    void FixedUpdate()
    {
        Vector3 rayDirection = player.transform.position - transform.position;
        rayDirection.y = 0;

        rayDirection.Normalize();
        rayDirection *= viewDistance;

        Debug.DrawRay(transform.position, rayDirection, Color.green);

        Physics.Raycast(transform.position, rayDirection, out RaycastHit lineOfSightRay, maxDistance: viewDistance);
        bool playerSeen = (lineOfSightRay.collider == playerCollider);

        if (playerSeen)
        {
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
            int row = Random.Range(0, maze.size);
            int col = Random.Range(0, maze.size);

            navMeshAgent.SetDestination(new Vector3(col * 2, 0, -row * 2));
        }
    }
}
