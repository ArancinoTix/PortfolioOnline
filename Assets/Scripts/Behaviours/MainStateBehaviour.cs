using UnityEngine;
using U9.FSM;

public class MainStateBehaviour : FSMBehaviour<GameState>
{
    public MainStateBehaviour(FSMState<GameState> owner) : base(owner)
    {
        
    }

    public override void OnEnter(FSMState<GameState> previousState)
    {

    }

    public override void OnExit(FSMState<GameState> nextState)
    {

    }
}
