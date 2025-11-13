using UnityEngine;
using U9.FSM;

public class MainStateBehaviour : FSMBehaviour<GameState>
{
    private InputManager _inputManager;
    private ShootingManager _shootingManager;
    public MainStateBehaviour(InputManager inputManager, ShootingManager shootingManager, FSMState<GameState> owner) : base(owner)
    {
        _inputManager = inputManager;
        _shootingManager = shootingManager;
    }

    public override void OnEnter(FSMState<GameState> previousState)
    {
        _inputManager.onPositionUpdate += Aim; 
    }

    public override void OnExit(FSMState<GameState> nextState)
    {
        _inputManager.onPositionUpdate -= Aim;
    }

    private void Aim(Vector2 dir)
    {
        _shootingManager.Aim(dir);
    }
}
