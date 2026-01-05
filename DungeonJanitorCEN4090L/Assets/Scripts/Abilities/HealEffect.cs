using UnityEngine;
using System.Collections;

public class HealEffect : IEffect
{
    public void Apply(AbilityUser caster, GameObject target, EffectDefinition def, Vector3 hitpoint)
    {
        //Debug.Log($"[HealEffect] Apply called! Target: {target?.name ?? "NULL"}");
        if (target == null || def == null)
        {
            Debug.LogWarning("HealEffect: Target or EffectDefinition is null.");
            return;
        }

        // Get the health component from the target
        HealthComponent healthComponent = target.GetComponent<HealthComponent>();
        //Debug.Log($"[HealEffect] HealthComponent found: {healthComponent != null}");
        if (healthComponent == null)
        {
            Debug.LogWarning($"HealEffect: {target.name} does not have a HealthComponent.");
            return;
        }

        float healAmount = def.GetScaledMagnitude();
        //Debug.Log($"[HealEffect] Healing {target.name} for {healAmount} HP");

        // Instant heal
        if (def.duration <= 0f)
        {
            Debug.Log("[HealEffect] Instant heal");
            healthComponent.Heal((int)healAmount);
        }
        // Heal over time

        // TODO: Implement Heal over Time
        else
        {
            // Start a coroutine for HoT (Heal over Time)
            if (target.TryGetComponent<MonoBehaviour>(out var behaviour))
            {
                behaviour.StartCoroutine(HealOverTime(healthComponent, healAmount, def.duration));
            }
            else
            {
                Debug.Log($"[HealEffect] Heal over time: {healAmount} over {def.duration} seconds");
                Debug.LogWarning("HealEffect: Cannot start HoT coroutine - target has no MonoBehaviour.");
                // Fallback to instant heal
                healthComponent.Heal((int)healAmount);
            }
        }
    }

    private IEnumerator HealOverTime(HealthComponent healthComponent, float totalHealAmount, float duration)
    {
        float elapsed = 0f;
        float tickRate = 0.5f; // Heal every 0.5 seconds
        float healPerTick = (totalHealAmount / duration) * tickRate;

        while (elapsed < duration)
        {
            if (healthComponent == null || healthComponent.IsDead)
                yield break;

            healthComponent.Heal((int)healPerTick); // TODO: Might be a problem with rounding here

            yield return new WaitForSeconds(tickRate);
            elapsed += tickRate;
        }

        // Apply any remaining healing due to rounding
        int remainingHeal = (int)(totalHealAmount - (healPerTick * Mathf.Floor(duration / tickRate)));
        if (remainingHeal > 0f && healthComponent != null && !healthComponent.IsDead)
        {
            healthComponent.Heal(remainingHeal);
        }
    }
}