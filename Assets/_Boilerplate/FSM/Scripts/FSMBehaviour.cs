namespace U9.FSM
{
    public abstract class FSMBehaviour<TStateID> where TStateID: System.IConvertible
    {
        private readonly FSMState<TStateID> m_owner;

        protected readonly string _stateIdString;

        public FSMBehaviour(FSMState<TStateID> owner)
        {
            this.m_owner = owner;
            _stateIdString = m_owner.GetID().ToString();
        }

        public FSMState<TStateID> GetOwner()
        {
            return m_owner;
        }

        ///<summary>
        /// Starts the action.
        ///</summary>
        public abstract void OnEnter(FSMState<TStateID> previousState);

        ///<summary>
        /// Finishes the action.
        ///</summary>
        public abstract void OnExit(FSMState<TStateID> nextState);
    }
}