public class StateMachine
{
    public State currentState { get; private set; }

    //初期化
    public void Init(State startState)
    {
        //ステートの初期化
        currentState = startState;
        currentState.Enter();
    }

    //ステートの変更
    public void ChangeState(State newState)
    {
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void UpdateActiveState()
    {
        currentState.Update();
    }
}