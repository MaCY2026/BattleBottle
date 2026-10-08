using UnityEngine.InputSystem;

public class Player_IdleState : State
{
    public Player_IdleState(Player player, StateMachine stateMachine, string stateName) :
    base(player, stateMachine, stateName)
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

        //キー入力を待つ変数
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;


        //ad左右を押したら、ステートをmoveStateにする
        if (keyboard.aKey.isPressed || keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed || keyboard.leftArrowKey.isPressed)
        {
            stateMachine.ChangeState(player.moveState);
        }
        else if (player.groundDetected && keyboard.spaceKey.isPressed)
        {
            stateMachine.ChangeState(player.jumpState);
        }
        //もしZキーが押されたら
        else if (keyboard.zKey.isPressed)
        {
            stateMachine.ChangeState(player.jetState);
        }
    }
}
