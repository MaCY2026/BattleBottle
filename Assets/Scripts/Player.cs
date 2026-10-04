using UnityEngine;

public class Player : MonoBehaviour
{
    private StateMachine stateMachine;

    //ステートの変数の宣言
    public Player_IdleState idleState { get; private set; }
    public Player_MoveState moveState { get; private set; }

    //初期化の前段階
    private void Awake()
    {
        stateMachine = new StateMachine();

        //ステートのアロケーション
        idleState = new Player_IdleState(this, stateMachine, "idle");
        moveState = new Player_MoveState(this, stateMachine, "move");
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
        //現在のステートを更新
        stateMachine.UpdateActiveState();
    }
}