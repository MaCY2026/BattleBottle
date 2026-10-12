using UnityEngine;

public class Player_IdleState : State
{
    public Player_IdleState(Player player, StateMachine stateMachine, string stateName)
        : base(player, stateMachine, stateName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.jetChargeLevel = 0f;
    }

    public override void Update()
    {
        base.Update();

        // 噴射
        if (input.JetHeld)
        {
            stateMachine.ChangeState(player.jetState);
            return;
        }

        // 地面から離れた場合
        if (!player.groundDetected)
        {
            stateMachine.ChangeState(player.hoverState);
            return;
        }

        // ジャンプ
        if (input.JumpReleased)
        {
            stateMachine.ChangeState(player.jumpState);
            return;
        }

        // 転がる
        if (Mathf.Abs(input.Rotate) >= 0.1f)
        {
            stateMachine.ChangeState(player.rollState);
            return;
        }

        // 通常の移動
        if (input.Direction != Vector3.zero)
        {
            stateMachine.ChangeState(player.moveState);
            return;
        }
    }
}