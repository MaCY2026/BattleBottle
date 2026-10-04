using UnityEngine.InputSystem;

public class Player_IdleState : State
{
    public Player_IdleState(Player player, StateMachine stateMachine, string stateName) :
    base(player, stateMachine, stateName)
    {
    }

    public override void Update()
    {
        base.Update();

    
        //キー入力を待つ変数
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;

        //もしZキーが押されたら
        if (keyboard != null)
        {
            //ステートをmoveStateにする
            if (keyboard.aKey.isPressed || keyboard.dKey.isPressed|| keyboard.rightArrowKey.isPressed || keyboard.leftArrowKey.isPressed)
            {
                stateMachine.ChangeState(player.moveState);
            }
        }   
    }
}
