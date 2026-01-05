using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Effects/HealEffectSO")]
public class HealEffectSO :  EffectSO
{
    public override IEffect CreateEffectInstance()
    {
        return new HealEffect();
    }
}
