using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Effects/DamageEffectSO")]
public class DamageEffectSO : EffectSO
{
    public override IEffect CreateEffectInstance()
    {
        return new DamageEffect();
    }
}
