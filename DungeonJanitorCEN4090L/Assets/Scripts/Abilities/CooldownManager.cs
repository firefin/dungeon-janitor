using System.Collections.Generic;
using UnityEngine;

public class CooldownManager
{
    private Dictionary<AbilityDefinition, float> cooldowns = new();

    public bool IsOnCooldown(AbilityDefinition ability)
    {
        if (!cooldowns.TryGetValue(ability, out float endsAt))
            return false;

        return Time.time < endsAt;
    }

    public void StartCooldown(AbilityDefinition ability)
    {
        cooldowns[ability] = Time.time + ability.Cooldown;
    }
}
