using UnityEngine;

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
        if (player.isJetUseWater)
        {
            player.waterVolume.Fill -= player.liquidLosRate * Time.deltaTime;
            if (player.waterVolume.Fill <= 0f)
            {
                player.waterVolume.Fill = 0f;
                stateMachine.ChangeState(player.hoverState);
                return;
            }
        }

        // 時間経過に応じて噴射チャージ量を増やす
        player.jetChargeLevel += Time.deltaTime * player.maxJetChargeLevel / player.timeToTopSpeed;
        player.jetChargeLevel = Mathf.Clamp(player.jetChargeLevel, 0f, player.maxJetChargeLevel);

        // 増加する速度：噴射の反対方向 × 噴射チャージのレベル × 変換率
        Vector2 acceleration = -(Vector2)player.transform.up * player.jetChargeLevel * player.jetConversionRate;

        player.AddVelocity(acceleration * Time.deltaTime);

        //回転を更新
        player.SetRotationSpeed(input.Rotate * player.rollAirSpeed);

        //もしZキーが押されたら
        if (!input.JetHeld)
        {
            if (player.groundDetected)
                stateMachine.ChangeState(player.idleState);
            else
                stateMachine.ChangeState(player.hoverState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
