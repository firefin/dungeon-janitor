using UnityEngine;

// TODO: Move to location that most makes sense
[CreateAssetMenu(menuName = "Scriptable Objects/AbilityDefinition")]
public class AbilityDefinition : ScriptableObject
{
    [Header("Basic info")]
    [SerializeField] private string abilityName;
    [SerializeField] private string description;
    [SerializeField] private Sprite icon;

    [Header("Cooldown & Resources")]
    [SerializeField] private float cooldown;
    [SerializeField] private float resourceCost;


    [Header("Targeting")]
    [SerializeField] private TargetingType targetingType;
    [SerializeField, Tooltip("Max distance for targeting. Set to 0 if not applicable.")] private float range; // Cannot be nullable, so set to 0 if not applicable
    [SerializeField] private LayerMask hitLayers;

    [Header("Effects")]
    [SerializeField] private EffectDefinition[] effects;

    [Header("Projectile (if applicable)")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed;

    // Basic Info
    public string Name => abilityName;
    public string Description => description;
    public Sprite Icon => icon;

    // Cooldown & Resources
    public float Cooldown => cooldown;
    public float ResourceCost => resourceCost;

    // Targeting
    public TargetingType Targeting => targetingType;
    public float? Range => range;
    public LayerMask HitLayers => hitLayers;

    // Effects
    public EffectDefinition[] Effects => effects;

    // Projectile (if applicable)
    public GameObject ProjectilePrefab => projectilePrefab;
    public float ProjectileSpeed => projectileSpeed;


    /// <summary>
    /// Execute the ability based on its targeting type
    /// </summary>
    public bool Use(GameObject caster, Transform castPoint)
    {
        switch (targetingType)
        {
            case TargetingType.Self:
                return UseSelf(caster);

            case TargetingType.RaycastSingle:
                return UseRaycast(caster, castPoint);

            case TargetingType.Projectile:
                return UseProjectile(caster, castPoint);

            case TargetingType.AreaOfEffect:
                return UseAOE(caster, castPoint);

            default:
                Debug.LogWarning($"Targeting type {targetingType} not implemented for {abilityName}");
                return false;
        }
    }

    private bool UseSelf(GameObject caster)
    {
        ApplyEffects(caster, caster, caster.transform.position);
        return true;
    }

    private bool UseRaycast(GameObject caster, Transform castPoint)
    {
        throw new System.NotImplementedException();
    }

    private bool UseProjectile(GameObject caster, Transform castPoint)
    {
        throw new System.NotImplementedException();
    }

    private bool UseAOE(GameObject caster, Transform castPoint)
    {
        throw new System.NotImplementedException();
    }

    private void ApplyEffects(GameObject caster, GameObject target, Vector3 hitPoint = default)
    {
        if (effects == null || effects.Length == 0) return;

        AbilityUser abilityUser = caster.GetComponent<AbilityUser>();
        if (abilityUser == null)
        {
            Debug.LogWarning($"ApplyEffects: No AbilityUser component found on caster {caster.name} for {abilityName}");
            return;
        }

        foreach (EffectDefinition effectDef in effects)
        {
            if (effectDef?.effect != null)
            {
                IEffect effectInstance = effectDef.effect.CreateEffectInstance();
                effectInstance.Apply(abilityUser, target, effectDef, hitPoint);
            }
        }
    }
}

public enum TargetingType
{
    Self,
    RaycastSingle,
    AreaOfEffect,
    Projectile
}