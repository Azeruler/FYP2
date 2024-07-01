using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;

    private Transform player;

    public LayerMask whatIsGround, whatIsPlayer;

    // Patrol mode
    public Vector3 walkPoint;
    bool walkPointSet;

    [Header("Patrol Settings")]
    public float range; // radius of sphere
    public Transform centrePoint; // centre of the area the agent wants to move around in

    [Header("Attack Settings")]
    // Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    // Cooldown
    public float attackCooldown = 2f; // Cooldown duration in seconds
    private float cooldownTimer = 0f; // Remaining cooldown time

    // States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    Animator animator;

    [Header("Surface Settings")]
    public SurfaceManager surfaceManager;

    // Projectile Enemy
    [Header("Projectile Settings")]
    public bool canShootProjectiles = false;
    public GameObject projectilePrefab;
    public Transform shootingPoint;

    [Header("Field of View")]
    public FieldOfView fieldOfView; // Reference to the FieldOfView component

    [Header("Player Settings")]
    private PlayerHealth playerHealth; // Reference to the player's health script

    [Header("Audio Settings")]
    public AudioSource walkingAudioSource;
    public AudioSource attackingAudioSource;
    public AudioSource idleAudioSource;
    public AudioClip[] walkingClips;
    public AudioClip[] attackingClips;
    public AudioClip[] idleClips;
    public float hearingRange = 10f;  // The range within which the enemy can hear sounds
    public LayerMask whatIsSound;    // LayerMask to filter sound events

    private bool isDead; // New flag to check if the enemy is dead

    private void Start()
    {
        InvokeRepeating(nameof(CheckForSound), 0f, 0.2f);
    }

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        isDead = false; // Initialize as not dead

        // Get the FieldOfView component from the same GameObject
        fieldOfView = GetComponent<FieldOfView>();

        // Get the PlayerHealth component from the player
        playerHealth = player.GetComponent<PlayerHealth>();
    }

    // Check if player is in sight range or attack range
    private void Update()
    {
        if (isDead) return; // Stop processing if the enemy is dead

        // Update cooldown timer
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }

        // Use the FieldOfView component to check if the player is in sight range
        playerInSightRange = fieldOfView.canSeePlayer;

        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange)
        {
            Patrol();
        }
        if (playerInSightRange && !playerInAttackRange)
        {
            ChasePlayer();
        }
        if (playerInSightRange && playerInAttackRange)
        {
            AttackPlayer();
        }
    }

    private void CheckForSound()
    {
        if (isDead) return; // Stop processing if the enemy is dead

        Collider[] soundSources = Physics.OverlapSphere(transform.position, hearingRange, whatIsSound);

        foreach (Collider source in soundSources)
        {
            Vector3 soundDirection = source.transform.position - transform.position;
            float distanceToSound = soundDirection.magnitude;

            if (distanceToSound <= hearingRange)
            {
                Debug.Log("Sound detected at position: " + source.transform.position);
                MoveTowardsSound(source.transform.position);
                return;
            }
        }
    }

    private void MoveTowardsSound(Vector3 soundPosition)
    {
        if (!agent.isOnNavMesh || isDead)
        {
            return;
        }

        Debug.Log("Moving towards sound at position: " + soundPosition);
        agent.SetDestination(soundPosition);
        animator.SetBool("isWalking", true);
        animator.SetBool("isIdle", false);
        animator.SetBool("isAttacking", false);
        PlayWalkingSound();
        PlayIdleSound();
    }

    private bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        Vector3 randomPoint = center + Random.insideUnitSphere * range; // random point in a sphere 
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }

    private void Patrol()
    {
        if (!agent.isOnNavMesh || isDead) // Check if the agent is on the NavMesh or dead
        {
            return;
        }

        if (agent.remainingDistance <= agent.stoppingDistance) // done with path
        {
            Vector3 point;
            if (RandomPoint(centrePoint.position, range, out point)) // pass in our centre point and radius of area
            {
                Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f); // so you can see with gizmos
                agent.SetDestination(point);
            }
        }

        animator.SetBool("isWalking", true);
        animator.SetBool("isIdle", false);
        animator.SetBool("isAttacking", false);
        PlayWalkingSound();
        PlayIdleSound();
    }

    private void ChasePlayer()
    {
        if (!agent.isOnNavMesh || isDead) // Check if the agent is on the NavMesh or dead
        {
            return;
        }

        agent.SetDestination(player.position);
        animator.SetBool("isWalking", true);
        animator.SetBool("isAttacking", false);
        animator.SetBool("isIdle", false);
        PlayWalkingSound();
        PlayIdleSound();  // Play attacking sounds while chasing
    }

    private void AttackPlayer()
    {
        if (!agent.isOnNavMesh || cooldownTimer > 0 || isDead) // Check if the agent is on the NavMesh, on cooldown, or dead
        {
            return;
        }

        agent.SetDestination(transform.position);
        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            alreadyAttacked = true;
            animator.SetBool("isIdle", false);
            animator.SetBool("isWalking", false);
            animator.SetBool("isAttacking", true);
            PlayAttackingSound();

            // Perform attack actions here
            if (canShootProjectiles)
            {
                ShootProjectile();
            }
            else
            {
                // Reduce player's health directly
                playerHealth.TakeDamage(20); // Adjust the damage value as needed
            }

            cooldownTimer = attackCooldown; // Start cooldown timer
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    public void ShootProjectile()
    {
        if (projectilePrefab && shootingPoint)
        {
            Instantiate(projectilePrefab, shootingPoint.position, Quaternion.LookRotation(player.position - shootingPoint.position));
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;

        // If the player is still within attack range, attack again
        if (Physics.CheckSphere(transform.position, attackRange, whatIsPlayer) && cooldownTimer <= 0 && !isDead)
        {
            AttackPlayer();
        }
        else
        {
            animator.SetBool("isAttacking", false);

            // Decide next action based on player position
            if (Physics.CheckSphere(transform.position, attackRange, whatIsPlayer) && cooldownTimer <= 0 && !isDead)
            {
                AttackPlayer();
            }
            else if (Physics.CheckSphere(transform.position, sightRange, whatIsPlayer) && !isDead)
            {
                ChasePlayer();
            }
            else
            {
                animator.SetBool("isWalking", true);
                animator.SetBool("isIdle", false);
                animator.SetBool("isAttacking", false);
                Patrol();
            }
        }
    }

    private void PlayWalkingSound()
    {
        if (walkingAudioSource && !walkingAudioSource.isPlaying && !isDead)
        {
            int clipIndex = Random.Range(0, walkingClips.Length);
            walkingAudioSource.clip = walkingClips[clipIndex];
            walkingAudioSource.Play();
        }
    }

    private void PlayAttackingSound()
    {
        if (attackingAudioSource && !attackingAudioSource.isPlaying && !isDead)
        {
            int clipIndex = Random.Range(0, attackingClips.Length);
            attackingAudioSource.clip = attackingClips[clipIndex];
            attackingAudioSource.Play();
        }
    }

    private void PlayIdleSound()
    {
        if (idleAudioSource && !idleAudioSource.isPlaying && !isDead)
        {
            int clipIndex = Random.Range(0, idleClips.Length);
            idleAudioSource.clip = idleClips[clipIndex];
            idleAudioSource.Play();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, hearingRange);
    }

    // Method to set the enemy as dead, called from Health
    public void SetDead()
    {
        isDead = true;
        agent.isStopped = true; // Stop the NavMeshAgent
        animator.SetBool("isDead", true); // Optionally trigger a dead animation
        // Disable any audio sources or other components as needed
    }
}
