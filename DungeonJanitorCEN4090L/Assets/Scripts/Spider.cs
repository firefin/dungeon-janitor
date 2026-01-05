using UnityEngine; 

public class Spider : Enemy
{
    [Header("Spider Specific")]
    public float erraticMovementAmount = 0.3f;
    private Vector2 erraticOffset;
    private float offsetChangeTimer;
    
    protected override void Start()
    {
        base.Start();
        
        // Spider stats
        maxHealth = 60f;
        currentHealth = maxHealth;
        moveSpeed = 4f;
        damage = 8f;
        detectionRange = 5f;
        attackRange = 1.7f;
        attackCooldown = 0.7f;
        shouldPatrol = false; // Spiders wait in ambush
    }
    
    protected override void Update()
    {
        base.Update();  // ← This calls the Enemy.cs Update with all the debug and attack logic
        
        // Change erratic offset periodically
        offsetChangeTimer += Time.deltaTime;
        if (offsetChangeTimer >= 0.2f)
        {
            erraticOffset = Random.insideUnitCircle * erraticMovementAmount;
            offsetChangeTimer = 0f;
        }
    }
    
    protected override void ChasePlayer()
    {
        // Spider moves erratically
        Vector2 direction = (player.position - transform.position).normalized;
        Vector2 finalDirection = (direction + erraticOffset).normalized;
        rb.linearVelocity = finalDirection * moveSpeed;
    }
    
    protected override void PerformAttack()
    {
        base.PerformAttack();
        
        // Apply poison (could expand this with a poison system)
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            //playerHealth.ApplyPoison(5f, 3f);
        }
    }
}
