using UnityEngine;

public class BottleNozzle : MonoBehaviour
{
    public Player Owner { get; private set; }

    private void Awake()
    {
        Owner = GetComponentInParent<Player>();
    }
}