using U9.View;

namespace U9.FSM
{
    public class ViewIntergrationBehaviour : FSMMainBehaviour
    {
        private ViewManager viewManager;
        private ViewIntergration view;

        public ViewIntergrationBehaviour(ViewManager viewManager, 
            FSMState<MainFSMState> owner) : base(owner)
        {
            this.viewManager = viewManager;
        }

        public override void OnEnter(FSMState<MainFSMState> previousState)
        {
            view = viewManager.GetView<ViewIntergration>();
            view.Display();
        }

        public override void OnExit(FSMState<MainFSMState> nextState)
        {
            view.Hide();
        }
    }
}
