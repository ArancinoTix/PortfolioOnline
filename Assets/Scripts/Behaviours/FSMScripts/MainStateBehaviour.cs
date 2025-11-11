using UnityEngine;
using U9.FSM;

public class MainStateBehaviour : FSMBehaviour<GameState>
{
    private InputManager _inputManager;
    public MainStateBehaviour(InputManager inputManager, FSMState<GameState> owner) : base(owner)
    {
        _inputManager = inputManager;
    }

    public override void OnEnter(FSMState<GameState> previousState)
    {

    }

    public override void OnExit(FSMState<GameState> nextState)
    {

    }
}
