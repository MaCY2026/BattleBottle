using UnityEngine;
using UnityEngine.InputSystem;

public class Player_JetState : State
{
    public Player_JetState(Player player, StateMachine stateMachine, string stateName) :
    base(player, stateMachine, stateName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        if (player.jetChargeLevel < player.maxJetChargeLevel * 0.8f)
        {
            player.jetChargeLevel = player.maxJetChargeLevel * 0.8f;
        }
    }

    public override void Update()
    {
        base.Update();
        // 液体を減らす
        player.waterVolume.Fill -= player.liquidLosRate * Time.deltaTime;
        if (player.waterVolume.Fill <= 0f)
        {
            player.waterVolume.Fill = 0f;
            stateMachine.ChangeState(player.hoverState);
        }

        // 時間経過に応じて噴射チャージ量を増やす
        player.jetChargeLevel += Time.deltaTime * player.maxJetChargeLevel / player.timeToTopSpeed;
        player.jetChargeLevel = Mathf.Clamp(player.jetChargeLevel, 0f, player.maxJetChargeLevel);

        // 増加する速度：噴射の反対方向 × 噴射チャージのレベル × 変換率
        player.AddVelocity(-player.transform.up * player.jetChargeLevel * player.jetConversionRate);

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
        if (!keyboard.zKey.isPressed)
        {
            stateMachine.ChangeState(player.hoverState);
        }
        else if (!keyboard.zKey.isPressed && player.groundDetected)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
