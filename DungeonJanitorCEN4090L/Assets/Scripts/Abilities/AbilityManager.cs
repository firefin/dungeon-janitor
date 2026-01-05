using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class AbilityManager : MonoBehaviour
{
    [SerializeField] private Player playerData;
    [SerializeField] private ResourceManager manaManager;

    private PlayerInput playerInput;
    private InputAction useAbility1Action;
    private InputAction useAbility2Action;
    private InputAction useAbility3Action;

    private float[] abilityCooldowns;

    // Event that AbilityUser subscribes to for execution
    public static event Action<AbilityDefinition> OnAbilityRequested;

    // Events for UI/feedback systems
    public static event Action<int, float> OnCooldownStarted; // slotIndex, duration
    public static event Action<int, float> OnCooldownUpdated; // slotIndex, remainingTime
    public static event Action<AbilityDefinition, string> OnAbilityFailed; // ability, reason

    private void Awake()
    {
        Debug.Log("AbilityManager: Awake called");

        if (playerData == null)
        {
            Debug.LogError("Player SO not assigned to AbilityManager.");
            return;
        }

        if (manaManager == null)
        {
            Debug.LogError("Mana ResourceManager not assigned to AbilityManager.");
            return;
        }

        playerInput = GetComponent<PlayerInput>();

        if (playerInput == null)
        {
            Debug.LogError("PlayerInput component not found on AbilityManager GameObject.");
            return;
        }

        Debug.Log($"AbilityManager: PlayerInput found. Current Action Map: {playerInput.currentActionMap?.name ?? "NULL"}");

        InitializeInputActions();
        abilityCooldowns = new float[playerData.EquippedAbilities.Length];

        Debug.Log("AbilityManager: Awake completed successfully");
    }

    private void InitializeInputActions()
    {
        useAbility1Action = playerInput.actions["UseAbility1"];
        useAbility2Action = playerInput.actions["UseAbility2"];
        useAbility3Action = playerInput.actions["UseAbility3"];

        Debug.Log($"AbilityManager: Actions found - Ability1: {useAbility1Action != null}, Ability2: {useAbility2Action != null}, Ability3: {useAbility3Action != null}");
    }

    private void OnEnable()
    {
        Debug.Log("AbilityManager: OnEnable called");

        if (useAbility1Action != null)
        {
            useAbility1Action.performed += OnAbility1Performed;
            useAbility1Action.Enable();
            Debug.Log($"AbilityManager: Ability1 action enabled. IsEnabled: {useAbility1Action.enabled}");
        }
        else
        {
            Debug.LogWarning("AbilityManager: useAbility1Action is null in OnEnable");
        }

        if (useAbility2Action != null)
        {
            useAbility2Action.performed += OnAbility2Performed;
            useAbility2Action.Enable();
        }
        if (useAbility3Action != null)
        {
            useAbility3Action.performed += OnAbility3Performed;
            useAbility3Action.Enable();
        }
    }

    private void OnDisable()
    {
        Debug.Log("AbilityManager: OnDisable called");

        if (useAbility1Action != null)
        {
            useAbility1Action.performed -= OnAbility1Performed;
            useAbility1Action.Disable();
        }
        if (useAbility2Action != null)
        {
            useAbility2Action.performed -= OnAbility2Performed;
            useAbility2Action.Disable();
        }
        if (useAbility3Action != null)
        {
            useAbility3Action.performed -= OnAbility3Performed;
            useAbility3Action.Disable();
        }
    }

    private void OnAbility1Performed(InputAction.CallbackContext ctx)
    {
        Debug.Log("AbilityManager: OnAbility1Performed triggered!");
        UseAbility(0);
    }

    private void OnAbility2Performed(InputAction.CallbackContext ctx)
    {
        Debug.Log("AbilityManager: OnAbility2Performed triggered!");
        UseAbility(1);
    }

    private void OnAbility3Performed(InputAction.CallbackContext ctx)
    {
        Debug.Log("AbilityManager: OnAbility3Performed triggered!");
        UseAbility(2);
    }

    private void Update()
    {
        for (int i = 0; i < abilityCooldowns.Length; i++)
        {
            if (abilityCooldowns[i] > 0f)
            {
                abilityCooldowns[i] -= Time.deltaTime;
                OnCooldownUpdated?.Invoke(i, abilityCooldowns[i]);
            }
        }
    }

    private void UseAbility(int index)
    {
        Debug.Log($"AbilityManager: UseAbility called with index {index}");

        AbilityDefinition ability = playerData.GetAbility(index);

        if (ability == null)
        {
            Debug.Log($"No ability equipped in slot {index + 1}");
            return;
        }

        if (abilityCooldowns[index] > 0f)
        {
            string reason = $"{ability.Name} is on cooldown! {abilityCooldowns[index]:F1}s remaining";
            Debug.Log(reason);
            OnAbilityFailed?.Invoke(ability, reason);
            return;
        }

        if (!manaManager.HasSufficientResources(ability.ResourceCost))
        {
            string reason = $"Not enough mana to use {ability.Name}! Need: {ability.ResourceCost}, Have: {manaManager.CurrentAmount}";
            Debug.Log(reason);
            OnAbilityFailed?.Invoke(ability, reason);
            return;
        }

        manaManager.SpendResource(ability.ResourceCost);
        abilityCooldowns[index] = ability.Cooldown;
        OnCooldownStarted?.Invoke(index, ability.Cooldown);

        // Fire event for AbilityUser to execute
        OnAbilityRequested?.Invoke(ability);

        Debug.Log($"Used {ability.Name}. Cooldown: {ability.Cooldown}(s), Mana spent: {ability.ResourceCost}");
    }

    public float GetCooldownRemaining(int index)
    {
        if (index < 0 || index >= abilityCooldowns.Length)
            return 0f;
        return Mathf.Max(0f, abilityCooldowns[index]);
    }

    public bool IsAbilityReady(int index)
    {
        return GetCooldownRemaining(index) <= 0f;
    }
}
