using UnityEngine;
using UnityEngine.InputSystem;

public class Player_HoverState : State
{
    public Player_HoverState(Player player, StateMachine stateMachine, string stateName) :
    base(player, stateMachine, stateName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
        // 空中では噴射チャージ量が徐々に減少する。
        player.jetChargeLevel = Mathf.Lerp(player.jetChargeLevel, 0, Time.deltaTime * player.jetChargeDecreaseRate);
        if (player.jetChargeLevel < 0.1f)
            player.jetChargeLevel = 0f;

        //キー入力を待つ変数
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;

        //回転をくわえる変数
        Vector3 deltaRotate = Vector3.zero;

        //回転
        if (keyboard.leftArrowKey.isPressed)
            deltaRotate.z += 5.0f;
        else if (keyboard.rightArrowKey.isPressed)
            deltaRotate.z -= 5.0f;

        //回転を更新
        player.AddRotation(deltaRotate * player.rollSpeed * Time.deltaTime);

        //もしZキーが押されたら
        if (keyboard.zKey.isPressed)
        {
            stateMachine.ChangeState(player.jetState);
        }
        else if (player.groundDetected)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
