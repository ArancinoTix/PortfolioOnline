namespace U9.FSM
{
    public abstract class FSMMainBehaviour : FSMBehaviour<MainFSMState>
    {
        protected FSMMainBehaviour(FSMState<MainFSMState> owner) : base(owner)
        {
        }
    }
}
