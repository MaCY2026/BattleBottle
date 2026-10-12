using UnityEngine;

public class Player_RollState : State
{
    public Player_RollState(Player player, StateMachine stateMachine, string stateName)
        : base(player, stateMachine, stateName)
    {
    }

    public override void Enter()
    {
        base.Enter();
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

        // 空中
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

        // 移動
        if (input.Direction != Vector3.zero)
        {
            stateMachine.ChangeState(player.moveState);
            return;
        }

        // 回転入力がない場合は停止状態へ
        if (input.Rotate == 0f)
        {
            stateMachine.ChangeState(player.idleState);
            return;
        }

        float rotateInput = input.Rotate;
        rb.angularVelocity = rotateInput * player.rollGroundSpeed;
    }

    public override void Exit()
    {
        base.Exit();
    }
}
