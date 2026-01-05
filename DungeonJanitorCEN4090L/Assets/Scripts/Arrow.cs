using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float damage = 12f;
    public float lifetime = 5f;
    private GameObject shooter; // The enemy who shot this arrow
    
    public void Initialize(GameObject shooter, float damage)
    {
        this.shooter = shooter;
        this.damage = damage;
    }
    
    private void Start()
    {
        Destroy(gameObject, lifetime);
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Ignore collision with the shooter
        if (shooter != null && collision.gameObject == shooter)
        {
            return;
        }
        
        // Ignore other enemies
        if (collision.GetComponent<Enemy>() != null)
        {
            return;
        }
        
        // Check if hit player
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
            
            Destroy(gameObject);
        }
        // Check if hit wall or obstacle (anything with a non-trigger collider)
        else if (!collision.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}