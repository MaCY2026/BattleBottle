using UnityEngine;
public class TriggerArea : MonoBehaviour
{
    // リンゴの当たり判定を指定する
    public BoxCollider2D apple;
    void OnTriggerEnter2D(Collider2D other)
    {
        // リンゴ以外なら何もしない
        if (other != apple)
            return;
        Debug.Log("長方形に触れた!");
        // 長方形を赤くする
        GetComponent<SpriteRenderer>().color = Color.red;
    }
}