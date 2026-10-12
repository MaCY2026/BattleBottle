using UnityEngine;

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

        // 回転を更新
        player.SetRotationSpeed(input.Rotate * player.rollAirSpeed);

        // 噴射
        if (input.JetHeld)
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
