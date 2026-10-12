using UnityEngine;
using UnityEngine.UI;

public enum PlayerId
{
    Player1,
    Player2
}

public class PlayerResourceUI : MonoBehaviour
{
    [SerializeField] private Player owner;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    [SerializeField] private Slider healthBar;

    [SerializeField] private Slider waterBar;
    public PlayerId Id => owner.Id;

    private void Awake()
    {
        if (owner == null)
            owner = GetComponentInParent<Player>();

        currentHealth = maxHealth;
    }

    private void Start()
    {
        if (healthBar != null)
        {
            healthBar.minValue = 0f;
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }

        if (waterBar != null)
        {
            waterBar.minValue = 0f;
            waterBar.maxValue = 1f;
        }

        UpdateWaterBar();
    }

    public void Update()
    {
        UpdateWaterBar();
    }

    public void TakeDamage(float damage)
    {
        if (damage <= 0f || currentHealth <= 0f) return;

        currentHealth = Mathf.Max(0f, currentHealth - damage);

        if (healthBar != null)
            healthBar.value = currentHealth;

        if (currentHealth <= 0f)
        {
            Debug.Log("0HP");
        }
    }

    public void UpdateWaterBar()
    {
        if (waterBar == null || owner == null || owner.waterVolume == null)
            return;

        waterBar.value = owner.waterVolume.Fill;
    }

    public void BindBars(Slider health, Slider water)
    {
        healthBar = health;
        waterBar = water;

        if (healthBar != null)
        {
            healthBar.minValue = 0f;
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }

        if (waterBar != null)
        {
            waterBar.minValue = 0f;
            waterBar.maxValue = 1f;
        }

        UpdateWaterBar();
    }
}
