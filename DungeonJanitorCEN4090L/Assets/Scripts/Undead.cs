using UnityEngine; 

public class Undead : Enemy
{
    [Header("Undead Specific")]
    public float persistentChaseMultiplier = 2f;
    
    protected override void Start()
    {
        base.Start();
        
        // Undead stats
        maxHealth = 150f;
        currentHealth = maxHealth;
        moveSpeed = 2.0f;
        damage = 20f;
        detectionRange = 7f; // Notices player from farther
        attackRange = 1.8f;
        attackCooldown = 2f;
        shouldPatrol = true;
        patrolRadius = 2f;
        patrolWaitTime = 3f;
    }
    
   protected override void Update()
    {
        if (currentState == EnemyState.Dead) return;
        
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
        
    protected override void PerformAttack()
    {
        base.PerformAttack();
        // Grab/bite attack - could slow player temporarily
    }
}
