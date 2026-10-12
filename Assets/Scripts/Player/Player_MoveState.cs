using UnityEngine;

public class Player_MoveState : State
{
    public Player_MoveState(Player player, StateMachine stateMachine, string stateName) :
    base(player, stateMachine, stateName)
    {
    }

    public override void Update()
    {
        base.Update();

        // 位置を更新
        player.AddPosition(input.Direction * player.moveSpeed * Time.deltaTime);
        // 回転を更新
        player.AddRotation(
            new Vector3(0f, 0f, input.Rotate * player.rollAirSpeed * Time.deltaTime)
        );

        //移動していなればステートをIdleにする
        if (input.JetHeld)
        {
            stateMachine.ChangeState(player.jetState);
        }
        else if (input.Direction == Vector3.zero)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}