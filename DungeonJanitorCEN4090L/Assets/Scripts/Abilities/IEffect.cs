public interface IEffect
{
    /// <summary>
    /// Apply the effect at runtime.
    /// <para><paramref name="caster"/> - the AbilityUser who cast this ability.</para>
    /// <para><paramref name="target"/> - GameObject hit (may be null for area/self effects).</para>
    /// <para><paramref name="def"/> - the EffectDefinition data from the AbilityDefinition.</para>
    /// <para><paramref name="hitPoint"/> - the world point where effect is applied (optional).</para>
    /// </summary>
    /// <param name="caster">The AbilityUser who cast this ability.</param>
    /// <param name="target">GameObject hit (may be null for area/self effects).</param>
    /// <param name="def">The EffectDefinition data from the AbilityDefinition.</param>
    /// <param name="hitPoint">The world point where effect is applied (optional).</param>
    void Apply(AbilityUser caster, UnityEngine.GameObject target, EffectDefinition def, UnityEngine.Vector3 hitPoint);
}
