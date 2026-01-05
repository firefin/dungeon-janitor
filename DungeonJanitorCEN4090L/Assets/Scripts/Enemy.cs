using UnityEngine;

public class Enemy : MonoBehaviour 
{
    [Header("Stats")]
    public float maxHealth = 100f;
    public float currentHealth;
    public float moveSpeed = 2f;
    public float damage = 10f;
    
    [Header("Detection")]
    public float detectionRange = 5f;
    public float attackRange = 1f;
    public LayerMask playerLayer;
    
    [Header("Patrol")]
    public bool shouldPatrol = true;
    public float patrolRadius = 3f;
    public float patrolWaitTime = 2f;
    
    [Header("Combat")]
    public float attackCooldown = 1.5f;
    public bool isRoomActive = false;
    protected float lastAttackTime;
    
    protected Rigidbody2D rb;
    protected SpriteRenderer spriteRenderer;
    protected Transform player;
    
    // States
    protected enum EnemyState { Idle, Patrol, Chase, Attack, Dead }
    protected EnemyState currentState = EnemyState.Idle;
    
    private Vector2 patrolTarget;
    private float patrolWaitTimer;
    private Vector2 spawnPoint;
    
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
        spawnPoint = transform.position;
        
        // Finds the player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            Debug.Log($"{gameObject.name} found player at {player.position}");
        }
        else
        {
            Debug.LogError($"{gameObject.name} could NOT find player! Make sure player has 'Player' tag.");
        }
        
        if (shouldPatrol)
        {
            SetNewPatrolTarget();
        }
    }
    
    protected virtual void Update()
    {
        if (currentState == EnemyState.Dead) return;
        
        if (player == null)
        {
            Debug.LogError($"{gameObject.name} has no player reference!");
            return;
        }
        
        // Only act if the room is active
        if (!isRoomActive)
        {
            currentState = EnemyState.Idle;
            rb.linearVelocity = Vector2.zero;
            return;
        }
        
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        // Room is active - chase player (with buffer for collision)
        if (distanceToPlayer <= attackRange + 0.5f)
        {
            currentState = EnemyState.Attack;
            AttackPlayer();
        }
        else
        {
            currentState = EnemyState.Chase;
            ChasePlayer();
        }
        
        // Flip sprite
        if (rb.linearVelocity.x != 0)
        {
            spriteRenderer.flipX = rb.linearVelocity.x < 0;
        }
    }
    
    protected virtual void Patrol()
    {
        float distanceToTarget = Vector2.Distance(transform.position, patrolTarget);
        
        if (distanceToTarget < 0.2f)
        {
            patrolWaitTimer += Time.deltaTime;
            rb.linearVelocity = Vector2.zero;
            
            if (patrolWaitTimer >= patrolWaitTime)
            {
                SetNewPatrolTarget();
                patrolWaitTimer = 0f;
            }
        }
        else
        {
            Vector2 direction = (patrolTarget - (Vector2)transform.position).normalized;
            rb.linearVelocity = direction * moveSpeed * 0.5f;
        }
    }
    
    protected virtual void ChasePlayer()
    {
        if (player == null) return;
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed;
    }
    
    protected virtual void AttackPlayer()
    {
        rb.linearVelocity = Vector2.zero;
        
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            PerformAttack();
            lastAttackTime = Time.time;
        }
    }
    
    protected virtual void PerformAttack()
    {
        if (player == null) return;
        
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }
    }
    
    public virtual void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        
        StartCoroutine(DamageFlash());
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    public void SetRoomActive(bool active)
    {
        isRoomActive = active;
        Debug.Log($"{gameObject.name} - isRoomActive set to: {active}");
    }
    
    protected virtual void Die()
    {
        currentState = EnemyState.Dead;
        
        EnemyRespawnManager respawnManager = FindAnyObjectByType<EnemyRespawnManager>();
        if (respawnManager != null)
        {
            respawnManager.OnEnemyDied(this);
        }
        
        Destroy(gameObject);
    }
    
    private void SetNewPatrolTarget()
    {
        Vector2 randomDirection = Random.insideUnitCircle * patrolRadius;
        patrolTarget = spawnPoint + randomDirection;
    }
    
    private System.Collections.IEnumerator DamageFlash()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
    }
    
    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        if (shouldPatrol)
        {
            Gizmos.color = Color.blue;
            Vector2 spawn = Application.isPlaying ? spawnPoint : (Vector2)transform.position;
            Gizmos.DrawWireSphere(spawn, patrolRadius);
        }
    }
}