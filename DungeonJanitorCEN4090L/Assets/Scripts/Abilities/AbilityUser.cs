using UnityEngine;

public class AbilityUser : MonoBehaviour
{
    [SerializeField] private Transform castPoint;
    [SerializeField] private float projectileLifetime = 5f;

    private void Awake()
    {
        if (castPoint == null)
            castPoint = transform;
    }

    private void OnEnable()
    {
        AbilityManager.OnAbilityRequested += ExecuteAbility;
    }

    private void OnDisable()
    {
        AbilityManager.OnAbilityRequested -= ExecuteAbility;
    }

    /// <summary>
    /// Executes the ability based on its targeting type.
    /// Called via event from AbilityManager after validation passes.
    /// </summary>
    public void ExecuteAbility(AbilityDefinition ability)
    {
        if (ability == null)
        {
            Debug.LogWarning("ExecuteAbility: Received null ability");
            return;
        }

        switch (ability.Targeting)
        {
            case TargetingType.Projectile:
                ExecuteProjectileAbility(ability);
                break;
            case TargetingType.RaycastSingle:
                ExecuteRaycastSingleAbility(ability);
                break;
            case TargetingType.Self:
                ExecuteSelfAbility(ability);
                break;
            case TargetingType.AreaOfEffect:
                ExecuteAreaOfEffectAbility(ability);
                break;
            default:
                Debug.LogWarning($"Unknown targeting type: {ability.Targeting}");
                break;
        }
    }

    private void ExecuteProjectileAbility(AbilityDefinition ability)
    {
        if (ability.ProjectilePrefab == null)
        {
            Debug.LogError($"Ability {ability.Name} has no projectile prefab assigned.");
            return;
        }

        GameObject projGO = Instantiate(ability.ProjectilePrefab, castPoint.position, castPoint.rotation);
        var proj = projGO.GetComponent<Projectile>();

        if (proj == null)
        {
            Debug.LogError("Projectile prefab requires a Projectile component.");
            Destroy(projGO);
            return;
        }

        proj.Caster = gameObject;
        proj.Speed = ability.ProjectileSpeed;
        proj.Lifetime = projectileLifetime;
        proj.Effects = ability.Effects;
        proj.HitLayers = ability.HitLayers;
    }

    private void ExecuteRaycastSingleAbility(AbilityDefinition ability)
    {
        float range = ability.Range ?? 100f;

        if (Physics.Raycast(castPoint.position, castPoint.forward, out RaycastHit hit, range, ability.HitLayers))
        {
            ApplyEffectsToTarget(hit.collider.gameObject, ability.Effects, hit.point);
        }
        else
        {
            Debug.Log($"{ability.Name}: No target hit");
        }
    }

    private void ExecuteSelfAbility(AbilityDefinition ability)
    {
        ApplyEffectsToTarget(gameObject, ability.Effects, transform.position);
    }

    private void ExecuteAreaOfEffectAbility(AbilityDefinition ability)
    {
        float range = ability.Range ?? 5f;
        Collider[] hits = Physics.OverlapSphere(castPoint.position, range, ability.HitLayers);

        foreach (Collider hit in hits)
        {
            ApplyEffectsToTarget(hit.gameObject, ability.Effects, hit.transform.position);
        }

        if (hits.Length == 0)
        {
            Debug.Log($"{ability.Name}: No targets in AOE range");
        }
    }

    private void ApplyEffectsToTarget(GameObject target, EffectDefinition[] effects, Vector3 hitPoint = default)
    {
        if (effects == null || target == null) return;

        foreach (var ed in effects)
        {
            if (ed == null || ed.effect == null) continue;
            var runtimeEffect = ed.effect.CreateEffectInstance();
            runtimeEffect.Apply(this, target, ed, hitPoint);
        }
    }
}
