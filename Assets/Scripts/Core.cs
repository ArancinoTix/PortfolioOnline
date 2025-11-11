using U9.FSM;
using UnityEngine;

public class Core : MonoBehaviour
{
    private FSM<GameState> _fsm;

    private void Start()
    {
        InitIaliseFSM();
        _fsm.Start(GameState.MainState);
    }

    private void InitIaliseFSM()
    {
        _fsm = new FSM<GameState>();

        var mainState = _fsm.AddState(GameState.MainState);
        mainState.AddBehaviour(new MainStateBehaviour(mainState));
        mainState.AddTransition(GameState.WorkExperience);
        mainState.AddTransition(GameState.AboutMe);

        var aboutMe = _fsm.AddState(GameState.AboutMe);
        aboutMe.AddBehaviour(new AboutMeBehaviour(aboutMe));
        aboutMe.AddTransition(GameState.MainState);

        var experience = _fsm.AddState(GameState.WorkExperience);
        aboutMe.AddBehaviour(new ExperiencesBehaviour(experience));
        aboutMe.AddTransition(GameState.MainState);
    }

}

public enum GameState
{
    MainState,
    AboutMe,
    WorkExperience
}
