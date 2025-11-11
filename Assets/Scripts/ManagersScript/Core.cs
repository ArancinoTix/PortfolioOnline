using U9.FSM;
using UnityEngine;

public class Core : MonoBehaviour
{
    [SerializeField] private InputManager _inputManager;
    [SerializeField] private ShootingManager _shootingManager;
    private FSM<GameState> _fsm;

    private void Start()
    {
        InitIaliseFSM();
        _shootingManager.Init();
        _fsm.Start(GameState.MainState);
    }

    private void InitIaliseFSM()
    {
        _fsm = new FSM<GameState>();

        var mainState = _fsm.AddState(GameState.MainState);
        mainState.AddBehaviour(new MainStateBehaviour(_inputManager, _shootingManager, mainState));
        mainState.AddTransition(GameState.WorkExperience);
        mainState.AddTransition(GameState.AboutMe);

        var aboutMe = _fsm.AddState(GameState.AboutMe);
        aboutMe.AddBehaviour(new AboutMeBehaviour(aboutMe));
        aboutMe.AddTransition(GameState.MainState);

        var experience = _fsm.AddState(GameState.WorkExperience);
        experience.AddBehaviour(new ExperiencesBehaviour(experience));
        experience.AddTransition(GameState.MainState);
    }

}

public enum GameState
{
    MainState,
    AboutMe,
    WorkExperience
}
