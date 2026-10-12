using UnityEngine;

public class MentosItem : Item
{
    protected override bool TryApply(Player player)
    {
        Debug.Log("Apply");

        return true;
    }

    private void Awake()
    {
        transform.eulerAngles = new Vector3(0f, 0f, 20.0f);
    }

    protected override void Update()
    {
        base.Update();
    }
}