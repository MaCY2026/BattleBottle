using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJump : MonoBehaviour
{
    //SerializeFieldはインスペクターで変更できるようにするもの

    [SerializeField] private float jumpForce = 12.0f; 

    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    private Collider2D col;
    private bool isGrounded;

    void Start()
    {
        // コンポーネントの自動取得
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;

        // 地面接地判定（プレイヤーの足元に少し重なる四角い判定を作る）
        // 引数: (中心点, サイズ, 角度, 対象レイヤー)
        isGrounded = Physics2D.OverlapBox(
            new Vector2(col.bounds.center.x, col.bounds.min.y),
            new Vector2(col.bounds.size.x * 0.9f, 0.1f), 0.0f, groundLayer);

        // 地面にいて、かつスペースキー（またはWキーや上矢印）が押されたらジャンプ
        if (isGrounded && keyboard.spaceKey.wasReleasedThisFrame)
        {
            // Y軸の速度を直接上書きして上方向への力を加える
            rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce);
        }
    }
}