using UnityEngine;

public class DropDamage : MonoBehaviour
{
    [SerializeField] private PlayerResourceUI owner;
    [SerializeField] private float damagePerHit = 0.1f;

    private void Awake()
    {
        if (owner == null)
            owner = GetComponentInParent<PlayerResourceUI>();

        if (owner == null)
            Debug.LogError("DropDamage: missing firing player's PlayerResourceUI.", this);
    }

    private void OnParticleCollision(GameObject other)
    {
        if (owner == null) return;
        PlayerResourceUI target = other.GetComponentInParent<PlayerResourceUI>();
        if (target == null) return;

        target.TakeDamage(damagePerHit);
    }
}
