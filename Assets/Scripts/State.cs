using UnityEngine;

//ステートクラス
public abstract class State
{
    protected Player player;
    protected StateMachine stateMachine;
    protected string stateName;
    public State(Player player, StateMachine stateMachine, string stateName)
    {
        this.player = player;
        this.stateMachine = stateMachine;
        this.stateName = stateName;
    }

    public virtual void Enter()
    {
        Debug.Log(stateName + "状態に入った");
    }

    public virtual void Update()
    {
        Debug.Log(stateName + "状態を更新中");
    }

    public virtual void Exit()
    {
        Debug.Log(stateName + "状態を終了した");
    }
}