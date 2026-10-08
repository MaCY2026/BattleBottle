using UnityEngine;

public class Player : MonoBehaviour
{
    private StateMachine stateMachine;
    public Rigidbody2D rb { get; private set; }
    private Collider2D col;
    public bool groundDetected { get; private set; }

    //ステートの変数の宣言
    public Player_IdleState idleState { get; private set; }
    public Player_MoveState moveState { get; private set; }
    public Player_JetState jetState { get; private set; }
    public Player_HoverState hoverState { get; private set; }
    public Player_JumpState jumpState { get; private set; }
    [SerializeField] public LayerMask groundLayer;
    [SerializeField] public float moveSpeed = 3f;
    [SerializeField] public float jumpForce = 5.0f;
    [SerializeField] public float jetChargeLevel = 0f;
    [SerializeField] public float maxJetChargeLevel = 100f;
    [SerializeField] public float timeToTopSpeed = 1.0f;
    [SerializeField] public float jetConversionRate = 0.01f;
    [SerializeField] public float jetChargeDecreaseRate = 0.3f;
    [SerializeField] public float rollSpeed = 28.0f;

    //初期化の前段階
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        stateMachine = new StateMachine();

        //ステートのアロケーション
        idleState = new Player_IdleState(this, stateMachine, "idle");
        moveState = new Player_MoveState(this, stateMachine, "move");
        jetState = new Player_JetState(this, stateMachine, "jet");
        hoverState = new Player_HoverState(this, stateMachine, "hover");
        jumpState = new Player_JumpState(this, stateMachine, "jump");
    }

    //初期化
    private void Start()
    {
        //ステートマシーン初期化
        stateMachine.Init(idleState);
    }

    //更新
    private void Update()
    {
        UpdateGrounded();
        //現在のステートを更新
        stateMachine.UpdateActiveState();
    }

    public void SetVelocity(float xVelocity, float yVelocity)
    {
        rb.linearVelocity = new Vector2(xVelocity, yVelocity);
    }

    public void SetVelocity(Vector2 velocity)
    {
        SetVelocity(velocity.x, velocity.y);
    }

    public void AddVelocity(Vector2 velocity)
    {
        rb.linearVelocity += velocity;
    }

    public void AddRotation(Vector3 rotate)
    {
        transform.eulerAngles += rotate;
    }

    public void UpdateGrounded()
    {
        // 地面接地判定（プレイヤーの足元に少し重なる四角い判定を作る）
        // 引数: (中心点, サイズ, 角度, 対象レイヤー)
        groundDetected = Physics2D.OverlapBox(
            new Vector2(col.bounds.center.x, col.bounds.min.y),
            new Vector2(col.bounds.size.x * 0.9f, 0.1f), 0.0f, groundLayer
            );
    }
}