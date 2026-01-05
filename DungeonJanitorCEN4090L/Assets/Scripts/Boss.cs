using UnityEngine;

public class Boss : Enemy
{
    [Header("Boss Specific")]
    public float phase2HealthThreshold = 0.5f;
    public float phase2SpeedMultiplier = 1.5f;
    public float phase2AttackMultiplier = 1.3f;
    private bool inPhase2 = false;
    
    [Header("Special Attack")]
    public float specialAttackCooldown = 5f;
    private float lastSpecialAttackTime;
    public GameObject projectilePrefab;
    
    protected override void Start()
    {
        base.Start();
        
        // Boss stats
        maxHealth = 500f;
        currentHealth = maxHealth;
        moveSpeed = 2f;
        damage = 30f;
        detectionRange = 10f;
        attackRange = 2f;
        attackCooldown = 2f;
        shouldPatrol = false;
    }
    
    protected override void Update()
    {
        base.Update();
        
        // Check for phase transition
        if (!inPhase2 && currentHealth <= maxHealth * phase2HealthThreshold)
        {
            EnterPhase2();
        }
        
        // Special attack timer
        if (currentState == EnemyState.Attack && Time.time >= lastSpecialAttackTime + specialAttackCooldown)
        {
            PerformSpecialAttack();
            lastSpecialAttackTime = Time.time;
        }
    }
    
    private void EnterPhase2()
    {
        inPhase2 = true;
        moveSpeed *= phase2SpeedMultiplier;
        damage *= phase2AttackMultiplier;
        attackCooldown *= 0.7f;
    }
    
    protected override void PerformAttack()
    {
        base.PerformAttack();
    }
    
    private void PerformSpecialAttack()
    {
        // Area damage around boss
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(transform.position, 3f, playerLayer);
        foreach (Collider2D col in hitPlayers)
        {
            PlayerHealth playerHealth = col.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage * 1.5f);
            }
        }
    }
    /*
    protected override void Die()
    {
        base.Die(); 
    }
    */
}