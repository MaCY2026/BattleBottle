using UnityEngine.InputSystem;

public class Player_JumpState : State
{
    public Player_JumpState(Player player, StateMachine stateMachine, string stateName) :
    base(player, stateMachine, stateName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.SetVelocity(rb.linearVelocityX, player.jumpForce);
    }

    public override void Update()
    {
        base.Update();

        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (!player.groundDetected)
        {
            stateMachine.ChangeState(player.hoverState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}