using UnityEngine; 

public class Skeleton : Enemy
{
    [Header("Skeleton Specific")]
    public float chargeSpeedMultiplier = 1.5f;
    
    protected override void Start()
    {
        base.Start();
        
        // Skeleton stats
        maxHealth = 100f;
        currentHealth = maxHealth;
        moveSpeed = 2.5f;
        damage = 15f;
        detectionRange = 6f;
        attackRange = 1.2f;
        attackCooldown = 1.4f;
        shouldPatrol = true;
        patrolRadius = 4f;
    }
    
    protected override void ChasePlayer()
    {
        // Skeleton charges faster when chasing
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed * chargeSpeedMultiplier;
    }
    
    protected override void PerformAttack()
    {
        base.PerformAttack();
        // Could add sword slash animation here
    }
}
