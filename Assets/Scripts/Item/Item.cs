using UnityEngine;

public abstract class Item : MonoBehaviour
{
    [SerializeField] protected float rotateSpeed = 25f;
    [SerializeField] protected float minRotateRate = 0.3f;
    [SerializeField] protected float fallSpeed = 25f;
    protected bool collected;

    protected virtual void Update()
    {
        UpdateTransform();
    }

    void UpdateTransform()
    {
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;

        float angle = transform.eulerAngles.y;

        float rate = Mathf.Abs(
            Mathf.Sin(angle * Mathf.Deg2Rad)
        );

        float speed = rotateSpeed * Mathf.Lerp(minRotateRate, 1f, rate);

        transform.Rotate(
            speed * Time.deltaTime, 0f, 0f,
            Space.World
        );
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected) return;

        Player player = other.GetComponent<Player>();
        if (player == null) return;

        if (!TryApply(player)) return;

        collected = true;
        Destroy(gameObject);
    }

    protected abstract bool TryApply(Player player);
}