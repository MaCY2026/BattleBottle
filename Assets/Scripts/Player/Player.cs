using UnityEngine;

[RequireComponent(typeof(PlayerInputManager))]
public class Player : MonoBehaviour
{
    [SerializeField] private PlayerId playerId;
    public PlayerId Id => playerId;
    private StateMachine stateMachine;
    public Rigidbody2D rb { get; private set; }
    private Collider2D col;
    public ParticleSystem drops;
    public LiquidVolume waterVolume;
    public PlayerInputManager input { get; private set; }
    [SerializeField] private ParticleSystem refillPlus;
    public bool groundDetected { get; private set; }

    //ステートの変数の宣言
    public Player_IdleState idleState { get; private set; }
    public Player_MoveState moveState { get; private set; }
    public Player_RollState rollState { get; private set; }
    public Player_JetState jetState { get; private set; }
    public Player_HoverState hoverState { get; private set; }
    public Player_JumpState jumpState { get; private set; }
    [SerializeField] public LayerMask groundLayer;
    [SerializeField] public float moveSpeed = 3f;
    [SerializeField] public float maxSpeed = 3f;
    [SerializeField] public float jumpForce = 5.0f;
    [SerializeField] public bool isJetUseWater = false;
    [SerializeField] public float jetChargeLevel = 0f;
    [SerializeField] public float maxJetChargeLevel = 100f;
    [SerializeField] public float timeToTopSpeed = 1.0f;
    [SerializeField] public float jetConversionRate = 0.01f;
    [SerializeField] public float jetChargeDecreaseRate = 0.3f;
    [SerializeField] public float rollAirSpeed = 28.0f;
    [SerializeField] public float rollGroundSpeed = 40.0f;
    [SerializeField] public float maxDropRate = 120f;
    [SerializeField] public float liquidLosRate = 0.1f;
    [SerializeField] private float groundCheckDistance = 0.05f;
    [SerializeField] private float groundLeaveDelay = 0.1f;
    private float airTime;
    private readonly RaycastHit2D[] groundHits = new RaycastHit2D[8];

    //初期化の前段階
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        input = GetComponent<PlayerInputManager>();

        if (waterVolume == null)
            waterVolume = GetComponentInChildren<LiquidVolume>(true);
        if (refillPlus == null)
        {
            foreach (var particles in GetComponentsInChildren<ParticleSystem>(true))
            {
                if (particles.name != "RefillPlus") continue;
                refillPlus = particles;
                break;
            }
        }

        stateMachine = new StateMachine();

        //ステートのアロケーション
        idleState = new Player_IdleState(this, stateMachine, "idle");
        moveState = new Player_MoveState(this, stateMachine, "move");
        rollState = new Player_RollState(this, stateMachine, "roll");
        jetState = new Player_JetState(this, stateMachine, "jet");
        hoverState = new Player_HoverState(this, stateMachine, "hover");
        jumpState = new Player_JumpState(this, stateMachine, "jump");

        StopRefillEffect();
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

        // チャージ量を0～1に収める
        float chargeRate = jetChargeLevel / maxJetChargeLevel;

        // チャージ量に合わせて、水滴の発生数を変える
        var emission = drops.emission;
        emission.rateOverTime = maxDropRate * chargeRate;

        ClampSpeed();
    }

    public void AddPosition(Vector3 deltaPosition)
    {
        transform.position += deltaPosition;
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

    public void SetRotationSpeed(float degreesPerSecond)
    {
        rb.angularVelocity = degreesPerSecond;
    }

    public void UpdateGrounded()
    {
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(groundLayer);
        filter.useTriggers = false;

        int count = col.Cast(
            Vector2.down,
            filter,
            groundHits,
            groundCheckDistance
        );

        bool groundBelow = false;

        for (int i = 0; i < count; i++)
        {
            if (groundHits[i].normal.y > 0.5f)
            {
                groundBelow = true;
                break;
            }
        }

        if (groundBelow)
        {
            airTime = 0f;
            groundDetected = true;
        }
        else
        {
            airTime += Time.deltaTime;

            groundDetected = groundDetected && airTime < groundLeaveDelay;
        }
    }

    private void ClampSpeed()
    {
        rb.linearVelocity = Vector2.ClampMagnitude(rb.linearVelocity, maxSpeed);
    }

    public void AddWater(float amount)
    {
        waterVolume.Fill = Mathf.Clamp01(
            waterVolume.Fill + amount
        );

        if (refillPlus != null &&
            !refillPlus.isPlaying)
        {
            refillPlus.Play();
        }
    }

    public void StopRefillEffect()
    {
        if (refillPlus != null)
        {
            refillPlus.Stop(
                true,
                ParticleSystemStopBehavior.StopEmitting
            );
        }
    }
}
