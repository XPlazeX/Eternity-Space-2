using DamageSystem;
using UnityEngine;

/// <summary>
/// Создаёт пресет смерти (эффекта смерти) по DeathContext.
/// </summary>
public class DeathCaller : MonoBehaviour
{
    [SerializeField] private bool hasWreck = true;
    [SerializeField] private Wreck wreck;
    [SerializeField] private float wreckVelocityMultiplier = 0.66f;
    [Space()]
    [SerializeField] private bool hasExplosion;
    [SerializeField] private ExplosionObject explosionObject;
    [SerializeField] private float scale = 1f;
    [SerializeField] private bool overrideColor = false;
    [SerializeField] private Color color = Color.wheat;
    [Space()]
    [SerializeField] private bool hasShuttles = false;
    [SerializeField] private ShuttleMine[] shuttleMines;

    public void Call(DeathContext ctx)
    {
        if (hasWreck && wreck != null)
        {
            // Debug.Log("Call");
            Wreck w = Instantiate(wreck, transform.position, transform.rotation);

            w.SetFixedStepDelta(ctx.velocity * wreckVelocityMultiplier);
            w.Detonate();

            if (ctx.damageTag == DamageTag.Charged)
            {
                w.TryCharge();
            }   
        }

        if (hasExplosion && explosionObject != null)
        {
            Explode(transform.position);
        }

        if (hasShuttles && shuttleMines.Length > 0)
        {
            DeployShuttles();
        }
    }

    private void Explode(Vector3 position)
    {
        ExplosionObject explosion = Pool.Spawn(explosionObject, position, transform.rotation);
        explosion.SetScale(scale);
        if (overrideColor)
            explosion.SetColor(color);
    }

    private void DeployShuttles()
    {
        for (int i = 0; i < shuttleMines.Length; i++)
        {
            DamageBody shuttle = EnemySpawner.Spawn(shuttleMines[i].shuttle, shuttleMines[i].pivot.position);
            shuttle.transform.up = shuttleMines[i].pivot.up;
        }
    }

    [System.Serializable]
    private struct ShuttleMine
    {
        public DamageBody shuttle;
        public Transform pivot;
    }
}

public struct DeathContext
{
    public Vector3 velocity;
    public DamageTag damageTag;
    public int overdamage;
    public bool wasStunned;
}