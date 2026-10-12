using UnityEngine;

public class MentosDeliver : MonoBehaviour
{
    [SerializeField] private float rotateSpeed = 25f;
    [SerializeField] private float minRotateRate = 0.3f;
    [SerializeField] private float fallSpeed = 25f;
    [SerializeField] private float lifeTimer = 0f;
    [SerializeField] private float lifeDuration = 25f;
    [SerializeField] private float dropCooldown = 0f;
    [SerializeField] private float firstDropTime = 1f;
    [SerializeField] private float dropDuration = 2f;

    [SerializeField] private Item item;


    void Awake()
    {
        dropCooldown = firstDropTime;
        lifeTimer = lifeDuration;
    }

    void Update()
    {
        lifeTimer -= Time.deltaTime;
        if (lifeTimer < 0f)
        {
            Destroy(gameObject);
            return;
        }

        UpdateTransform();

        dropCooldown -= Time.deltaTime;
        if (dropCooldown < 0f)
        {
            dropCooldown = dropDuration;
            DropItem();
        }
    }

    private void UpdateTransform()
    {
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;

        float angle = transform.eulerAngles.y;

        float rate = Mathf.Abs(
            Mathf.Sin(angle * Mathf.Deg2Rad)
        );

        float speed = rotateSpeed * Mathf.Lerp(minRotateRate, 1f, rate);

        transform.Rotate(
            0f, speed * Time.deltaTime, 0f,
            Space.World
        );
    }

    void DropItem()
    {
        if (item == null) return;

        Vector3 position = new Vector3(0f, 5f, 0f);

        Item spawnedItem = Instantiate(
            item,
            transform.position,
            Quaternion.identity
        );
    }
}
