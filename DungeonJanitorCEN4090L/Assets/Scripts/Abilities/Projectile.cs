using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Projectile : MonoBehaviour
{

    public GameObject Caster { get; set; }
    public float Speed { get; set; }
    public float Lifetime { get; set; }
    public EffectDefinition[] Effects { get; set; }
    public LayerMask HitLayers { get; set; }

    private float spawnTime;

    private void Start()
    {
        spawnTime = Time.time;
        // ensure collider is trigger to use OnTriggerEnter
        var col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
    }

    private void Update()
    {
        transform.position += transform.forward * Speed * Time.deltaTime;

        if (Time.time - spawnTime > Lifetime)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        // ignore the caster (optional)
        if (other.gameObject == Caster) return;

        // layer check
        if (((1 << other.gameObject.layer) & HitLayers) == 0) return;

        // apply all effects to the hit object
        if (Effects != null)
        {
            foreach (var ed in Effects)
            {
                if (ed == null || ed.effect == null) continue;
                var runtimeEffect = ed.effect.CreateEffectInstance();
                runtimeEffect.Apply(Caster?.GetComponent<AbilityUser>(), other.gameObject, ed, transform.position);
            }
        }

        // Destroy on hit (could be modified for piercing projectiles)
        Destroy(gameObject);
    }
}
