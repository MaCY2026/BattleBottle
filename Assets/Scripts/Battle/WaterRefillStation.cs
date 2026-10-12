using UnityEngine;

public class WaterRefillStation : MonoBehaviour
{
    [SerializeField] private float refillRate = 0.2f;

    void OnTriggerStay2D(Collider2D other)
    {
        var mouth = other.GetComponent<BottleNozzle>();

        if (mouth == null || mouth.Owner == null)
            return;

        mouth.Owner.AddWater(refillRate * Time.deltaTime);

        GetComponent<SpriteRenderer>().color = Color.red;

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        GetComponent<SpriteRenderer>().color = Color.white;
        var player = other.GetComponent<Player>();
        if (player)
        {
            player.StopRefillEffect();
        }
    }
}
