using UnityEngine;

//ステートクラス
public abstract class State
{
    protected Player player;
    protected StateMachine stateMachine;
    protected string stateName;

    protected Rigidbody2D rb;

    public State(Player player, StateMachine stateMachine, string stateName)
    {
        this.player = player;
        this.stateMachine = stateMachine;
        this.stateName = stateName;

        rb = player.rb;
    }

    public virtual void Enter()
    {
        Debug.Log(stateName + "状態に入った");
    }

    public virtual void Update()
    {

    }

    public virtual void Exit()
    {
        Debug.Log(stateName + "状態を終了した");
    }
}