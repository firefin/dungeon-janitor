using UnityEngine;

public abstract class EffectSO : ScriptableObject
{
    /// <summary>
    /// Create a runtime instance of the effect logic (IEffect).
    /// This lets us keep SOs as data and runtime objects as logic.
    /// </summary>
    public abstract IEffect CreateEffectInstance();
}
