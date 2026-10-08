using UnityEngine;

public class GravityCenter : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private float gravityStrength = 80f;
    [SerializeField] private float minDistance = 1f;

    private void FixedUpdate()
    {
        if (player == null) return;

        Vector2 toCenter =
            (Vector2)transform.position - player.rb.position;

        float distanceSquared = Mathf.Max(
            toCenter.sqrMagnitude,
            minDistance * minDistance
        );

        Vector2 acceleration =
            toCenter.normalized * gravityStrength / distanceSquared;

        player.AddVelocity(acceleration * Time.fixedDeltaTime);
    }
}