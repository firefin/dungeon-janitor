using UnityEngine;

public class SkeletonArcher : Enemy
{
    [Header("Archer Specific")]
    public GameObject arrowPrefab;
    public float shootingRange = 8f;
    public float minDistanceFromPlayer = 3f;
    public float arrowSpeed = 10f;
    public Transform shootPoint;
    
    protected override void Start()
    {
        base.Start();
        
        // Archer stats
        maxHealth = 60f;
        currentHealth = maxHealth;
        moveSpeed = 2f;
        damage = 12f;
        detectionRange = 10f;
        attackRange = 8f;
        attackCooldown = 2f;
        shouldPatrol = true;
        patrolRadius = 5f;
    }
    
    protected override void Update()
    {
        if (currentState == EnemyState.Dead) return;
        
        if (player == null)
        {
            return;
        }
        
        if (!isRoomActive)
        {
            currentState = EnemyState.Idle;
            rb.linearVelocity = Vector2.zero;
            return;
        }
        
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        // Archer behavior: Keep distance and shoot
        if (distanceToPlayer <= attackRange && distanceToPlayer >= minDistanceFromPlayer)
        {
            // In shooting range - stop and shoot
            currentState = EnemyState.Attack;
            AttackPlayer();
        }
        else if (distanceToPlayer < minDistanceFromPlayer)
        {
            // Too close - back away!
            currentState = EnemyState.Chase;
            BackAwayFromPlayer();
        }
        else
        {
            // Too far - move closer
            currentState = EnemyState.Chase;
            ChasePlayer();
        }
        
        // Flip sprite based on player direction
        if (player.position.x < transform.position.x)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
    }
    
    protected override void ChasePlayer()
    {
        if (player == null) return;
        
        Vector2 direction = ((Vector2)player.position - (Vector2)transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed * 0.7f;
    }
    
    private void BackAwayFromPlayer()
    {
        if (player == null) return;
        
        // Move away from player
        Vector2 direction = ((Vector2)transform.position - (Vector2)player.position).normalized;
        rb.linearVelocity = direction * moveSpeed;
    }
    
    protected override void AttackPlayer()
    {
        rb.linearVelocity = Vector2.zero;
        
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            PerformAttack();
            lastAttackTime = Time.time;
        }
    }
    
    protected override void PerformAttack()
    {
        if (player == null) return;
        
        if (arrowPrefab != null)
        {
            ShootArrow();
        }
        else
        {
            base.PerformAttack();
        }
    }
    
    private void ShootArrow()
    {
        // Determine shoot position
        Vector2 spawnPos = shootPoint != null ? (Vector2)shootPoint.position : (Vector2)transform.position;
        
        // Calculate direction to player
        Vector2 direction = ((Vector2)player.position - spawnPos).normalized;
        
        // Spawn arrow
        GameObject arrow = Instantiate(arrowPrefab, spawnPos, Quaternion.identity);
        
        // Rotate arrow to face direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        arrow.transform.rotation = Quaternion.Euler(0, 0, angle);
        
        // Set arrow velocity
        Rigidbody2D arrowRb = arrow.GetComponent<Rigidbody2D>();
        if (arrowRb != null)
        {
            arrowRb.linearVelocity = direction * arrowSpeed;
        }
        
        // Initialize arrow with shooter info and damage
        Arrow arrowScript = arrow.GetComponent<Arrow>();
        if (arrowScript != null)
        {
            arrowScript.Initialize(gameObject, damage);
        }
    }
    
    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        
        // Draw shooting range (green)
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, shootingRange);
        
        // Draw minimum distance (cyan)
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, minDistanceFromPlayer);
    }
}