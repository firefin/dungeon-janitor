using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float baseMaxHealth = 100f;
    public float maxHealth;
    public float currentHealth;

    [Header("UI (Optional)")]
    public UnityEngine.UI.Slider healthBar;

    private void Start()
    {
        // Apply Iron Body passive
        maxHealth = PassiveStats.ModifyMaxHealth((int)baseMaxHealth);
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        if (healthBar != null)
        {
            healthBar.value = currentHealth / maxHealth;
        }
    }

    private void Die()
    {
        // Handle player death (restart, game over screen, etc.)
        // For now, just respawn
        currentHealth = maxHealth;
        UpdateHealthUI();
    }
}

