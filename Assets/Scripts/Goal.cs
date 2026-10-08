using UnityEngine;

public class Goal : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Goal");
            GetComponent<SpriteRenderer>().color = Color.red;
        }
    }
}
