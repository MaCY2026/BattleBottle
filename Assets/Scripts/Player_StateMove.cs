using UnityEngine;
using UnityEngine.InputSystem;

public class Player_MoveState : State
{
    // 移動速度の宣言
    [SerializeField] public float speed = 3f;
    //回転速度の宣言
    [SerializeField] public float rollspeed = 28.0f;

    public Player_MoveState(Player player, StateMachine stateMachine, string stateName) :
    base(player, stateMachine, stateName)
    {
    }

    public override void Update()
    {
        base.Update();

        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;

        //位置を更新するための変数
        Vector3 direction = Vector3.zero;
        //回転をくわえる変数
        Vector3 rotate = Vector3.zero;

        //移動
        if (keyboard.aKey.isPressed)
            direction.x -= 1f;
        else if (keyboard.dKey.isPressed)
            direction.x += 1f;
        // 斜め移動の処理
        direction = direction.normalized;

        //回転
        if (keyboard.leftArrowKey.isPressed)
            rotate.z += 5.0f;
        else if (keyboard.rightArrowKey.isPressed)
            rotate.z -= 5.0f;


        // 位置を更新
        player.transform.position += direction * speed * Time.deltaTime;
        //回転を更新
        player.transform.eulerAngles += rotate *rollspeed * Time.deltaTime;

        //移動していなればステートをIdleにする
        if (direction.x ==0||rotate.z==0)
            stateMachine.ChangeState(player.idleState);
    }
}