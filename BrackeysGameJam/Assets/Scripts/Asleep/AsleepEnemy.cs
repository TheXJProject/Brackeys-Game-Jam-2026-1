using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[Serializable]
public class EnemyInformation
{
    [SerializeField] private string name = "RENAME_ME";
    public Sprite sprite;
    public RuntimeAnimatorController animation;
}

public class AsleepEnemy : MonoBehaviour
{
    private static int enemyVisualChoice = 0;
    [SerializeField] private List<EnemyInformation> visualChoices;
    private EnemyInformation thisEnemyVisual;

    public float roamSpeed;
    public float chaseSpeed;
    public float straightViewDistance;
    public float diagonalViewDistance;
    public float killDistance;
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    public int enemyID;

    [SerializeField] private float diagonalAngle;

    public List<int> allowedTargetNodes;

    private Maze maze;
    private bool isTrapped;

    private GameObject player;
    private NavMeshAgent navMeshAgent;
    private Collider playerCollider;
    private Collider enemyCollider;
    private AudioSource audioSource;

    private Vector3 targetBeforeStopped;

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
        if (isTrapped) return;

        if (frozen)
        {
            targetBeforeStopped = navMeshAgent.destination;
            navMeshAgent.enabled = false;
        }
        else
        {
            navMeshAgent.enabled = true;
            navMeshAgent.destination = targetBeforeStopped;
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
        navMeshAgent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();

        navMeshAgent.speed = roamSpeed;

        enemyVisualChoice++;
        enemyVisualChoice = enemyVisualChoice % visualChoices.Count;
        thisEnemyVisual = visualChoices[enemyVisualChoice];

        spriteRenderer.sprite = thisEnemyVisual.sprite;
        animator.runtimeAnimatorController = thisEnemyVisual.animation;
    }

    private void FixedUpdate()
    {
        SpriteLookAtPlayer();

        if (!navMeshAgent.enabled) return;

        Vector3 rayDirection = player.transform.position - transform.position;
        Vector3 rayOrigin = transform.position;

        rayDirection.Normalize();

        rayOrigin.y = 1f;
        rayDirection.y = 0f;

        rayDirection *= straightViewDistance;

        // Vector3 rayDirectionAbs =
        //     new Vector3(Math.Abs(rayDirection.x), Math.Abs(rayDirection.y), Math.Abs(rayDirection.z));
        //
        // print(Vector3.Angle(rayDirectionAbs, transform.forward));
        //
        // if (Vector3.Angle(rayDirectionAbs, transform.forward) > diagonalAngle)
        // {
        //     rayDirection *= diagonalViewDistance;
        // }
        // else
        // {
        //     rayDirection *= straightViewDistance;
        // }

        Debug.DrawRay(rayOrigin, rayDirection, Color.green);

        Physics.Raycast(rayOrigin, rayDirection, out RaycastHit lineOfSightRay, maxDistance: straightViewDistance);
        bool playerSeen = (lineOfSightRay.collider == playerCollider);

        if (playerSeen)
        {
            // DOM I'VE PUT MY CODE TO TRIGGER SPRITE CHANGE HERE:
            animator.SetBool("spottedPlayer", true);

            if (lineOfSightRay.distance <= killDistance)
            {
                AsleepPlayerControl.killPlayer();
                return;
            }

            onPlayerSeen?.Invoke(enemyID, audioSource);

            navMeshAgent.speed = chaseSpeed;
            navMeshAgent.SetDestination(player.transform.position);
            targetBeforeStopped = navMeshAgent.destination;
        }
        else
        {
            // DOM I'VE PUT MY CODE TO TRIGGER SPRITE CHANGE HERE:
            animator.SetBool("spottedPlayer", false);
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

    private void SpriteLookAtPlayer()
    {
        spriteRenderer.transform.LookAt(player.transform);

        Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
        directionToPlayer.y = 0;

        spriteRenderer.transform.forward = directionToPlayer;
    }

    public void SetAllowedTargetNodes(List<int> targetNodes)
    {
        allowedTargetNodes = targetNodes;
    }
}
