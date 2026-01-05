using Unity.Hierarchy;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.Events;

public interface IHealth
{
    int CurrentHealth { get; }
    int MaxHealth { get; }
    bool IsDead { get; }

    void Heal(int amount);
    void TakeDamage(int amount);
}

public class HealthComponent : MonoBehaviour, IHealth
{
    [SerializeField] private Player playerData; // Reference to the Player ScriptableObject


    [Header("Events")]
    public UnityEvent<float> OnHealthChanged;
    public UnityEvent<float> OnHealed;
    public UnityEvent<float> OnDamaged;
    public UnityEvent OnDeath;


    public int MaxHealth => playerData != null ? playerData.MaxHealth : 0;
    public int CurrentHealth => playerData != null ? playerData.CurrentHealth : 0;
    public bool IsDead => playerData != null && playerData.IsDead;
    public float HealthPercentage => playerData != null ? playerData.HealthPercentage : 0f;

    private void Awake()
    {
        if(playerData == null)
        {
            Debug.LogError("Player ScriptableObject not assigned in HealthComponent.");
            return;
        }
        playerData.Initialize();
        playerData.ApplyPassiveStats();
    }
    private void Start()
    {
        OnHealthChanged?.Invoke(CurrentHealth);
    }
    public void TakeDamage(int amount)
    {
        if (playerData == null || IsDead || amount <= 0f) return;

        playerData.CurrentHealth = Mathf.Max(0, playerData.CurrentHealth - amount);
        OnDamaged?.Invoke(amount);
        OnHealthChanged?.Invoke(CurrentHealth);
        Debug.Log($"{name} took {amount} damage. Current health: {CurrentHealth}/{MaxHealth}");
        if (IsDead) Die();
    }

    public void Heal(int amount)
    {
        if(IsDead || amount <= 0f) return;
        
        float healthBefore = CurrentHealth;
        playerData.CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, MaxHealth);

        //Added this var in case we ever want to add a stat tracker for total healing lol
        float actualHealing = CurrentHealth - healthBefore;

        if(actualHealing > 0f)
        {
            OnHealed?.Invoke(actualHealing);
            OnHealthChanged?.Invoke(CurrentHealth);
            Debug.Log($"{name} healed for {actualHealing} health. Current health: {CurrentHealth}/{MaxHealth}");
        }
    }
    private void Die()
    {
        OnDeath?.Invoke();
        Debug.Log($"{name} died.");
        // TODO: Add death logic (play animation, drop loot, etc.)

    }
    // Added this in case we want to fully heal the player at some point.
    public void FullHeal()
    {
        if(playerData == null || CurrentHealth >= MaxHealth) return;
        Heal(MaxHealth);
    }
    // For testing purposes only
    [ContextMenu("Test Take 5 Damage")]
    private void TestTakeDamage()
    {
        TakeDamage(5);
    }
    [ContextMenu("Test Heal 5 Health")]
    private void TestHeal()
    {
        Heal(5);
    }
}
