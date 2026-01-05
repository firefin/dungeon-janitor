using UnityEngine;
using UnityEngine.Events;

public enum ResourceType
{
    Mana,
    Stamina,
    Ammo,
    Arrows,
    Custom
}

public class ResourceManager : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private ResourceType resourceType = ResourceType.Custom;
    [SerializeField, Tooltip("Assign Player SO to sync with player stats (only works for Mana type). Leave empty for standalone resources like ammo/arrows.")]
    private Player playerData;

    [Header("Standalone Resources (ignored if using Player SO)")]
    [SerializeField] private float maxResource = 100f;
    [SerializeField] private float currentResource = 100f;

    [Header("Regeneration")]
    [SerializeField] private float regenRate = 5f;
    [SerializeField] private bool regenerate = true;

    [Header("Events")]
    public UnityEvent<float> OnResourceChanged;
    public UnityEvent<float> OnResourceSpent;
    public UnityEvent<float> OnResourceGained;

    // Properties that handle both SO and standalone modes
    public float CurrentAmount
    {
        get => UsesPlayerSO ? GetSOCurrentResource() : currentResource;
        private set
        {
            if (UsesPlayerSO)
                SetSOCurrentResource(value);
            else
                currentResource = value;

            OnResourceChanged?.Invoke(CurrentAmount);
        }
    }

    public float Max => UsesPlayerSO ? GetSOMaxResource() : maxResource;
    public float RegenRate => regenRate;
    public bool Regenerate => regenerate;
    public float ResourcePercentage => Max > 0 ? CurrentAmount / Max : 0f;

    private bool UsesPlayerSO => playerData != null && resourceType == ResourceType.Mana;

    private void Awake()
    {
        // Initialize standalone resources
        if (!UsesPlayerSO)
        {
            currentResource = maxResource;
        }
    }

    private void Start()
    {
        OnResourceChanged?.Invoke(CurrentAmount);
    }

    private void Update()
    {
        if (!regenerate || CurrentAmount >= Max)
            return;

        float newAmount = CurrentAmount + regenRate * Time.deltaTime;
        CurrentAmount = Mathf.Clamp(newAmount, 0, Max);
    }

    public bool HasSufficientResources(float amount)
    {
        return CurrentAmount >= amount;
    }

    public bool SpendResource(float amount)
    {
        if (!HasSufficientResources(amount))
        {
            Debug.Log($"Insufficient {resourceType}. Need: {amount}, Have: {CurrentAmount}");
            return false;
        }

        CurrentAmount -= amount;
        OnResourceSpent?.Invoke(amount);
        Debug.Log($"Spent {amount} {resourceType}. Remaining: {CurrentAmount}/{Max}");
        return true;
    }

    public void AddResource(float amount)
    {
        if (amount <= 0f) return;

        float before = CurrentAmount;
        CurrentAmount = Mathf.Clamp(CurrentAmount + amount, 0, Max);
        float actualGain = CurrentAmount - before;

        if (actualGain > 0f)
        {
            OnResourceGained?.Invoke(actualGain);
            Debug.Log($"Gained {actualGain} {resourceType}. Current: {CurrentAmount}/{Max}");
        }
    }

    public void FullRestore()
    {
        AddResource(Max);
    }

    // SO Helper Methods
    private float GetSOCurrentResource()
    {
        return resourceType switch
        {
            ResourceType.Mana => playerData.CurrentMana,
            _ => 0f
        };
    }

    private float GetSOMaxResource()
    {
        return resourceType switch
        {
            ResourceType.Mana => playerData.MaxMana,
            _ => 0f
        };
    }

    private void SetSOCurrentResource(float value)
    {
        switch (resourceType)
        {
            case ResourceType.Mana:
                playerData.CurrentMana = (int)value;
                break;
        }
    }

    // Testing
    [ContextMenu("Test Spend 10 Resource")]
    private void TestSpend()
    {
        SpendResource(10f);
    }

    [ContextMenu("Test Add 20 Resource")]
    private void TestAdd()
    {
        AddResource(20f);
    }
}