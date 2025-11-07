using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace U9.FSM
{
    public class FSM<TStateID> : IUpdatable where TStateID : System.IConvertible
    {
        public FSMState<TStateID> CurrentState
        {
            get; private set;
        }

        private readonly Dictionary<TStateID, FSMState<TStateID>> _stateMap;

        /// <summary>
        ///     Read state machine map
        /// </summary>
        public IReadOnlyDictionary<TStateID, FSMState<TStateID>> States => _stateMap;

        ///<summary>
        /// This is the constructor that will initialize the FSM and give it
        /// a unique name or id.
        ///</summary>
        public FSM()
        {
            _stateMap = new Dictionary<TStateID, FSMState<TStateID>>();
            CurrentState = null;
        }

        public bool AllowUnsafeTransitions = false;

        ///<summary>
        /// This initializes the FSM. We can indicate the starting State of
        /// the Object that has an FSM.
        ///</summary>
        public void Start(TStateID state)
        {
            if (!_stateMap.ContainsKey(state))
            {
                Debug.LogError("State not found " + state);
                return;
            }

            ChangeToState(_stateMap[state]);
        }

        /// <summary>
        /// Changes the state if the transition is allowed by the state machine or the surrent state.
        /// </summary>
        public void ChangeState(TStateID stateId)
        {
            if(AllowUnsafeTransitions || CurrentState.HasTransitionFor(stateId))
                ChangeToState(_stateMap[stateId]);
            else
                Debug.LogError("The current state has no transition for event " + stateId);
        }

        /// <summary>
        /// Creates a state with 'id' and returns a reference
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FSMState<TStateID> AddState(TStateID id)
        {
            if (_stateMap.ContainsKey(id))
            {
                Debug.LogWarning("The FSM already contains: " + id);
                return null;
            }

            FSMState<TStateID> newState = new FSMState<TStateID>(id, this);
            _stateMap[id] = newState;

            return newState;
        }

        ///<summary>
        /// Call this under a MonoBehaviour's Update.
        ///</summary>
        public void Update()
        {
            if (CurrentState == null)
                return;

            CurrentState.OnUpdate();
        }

        ///<summary>
        /// This changes the state of the Object. This also calls the exit
        /// state before doing the next state.
        ///</summary>
        private void ChangeToState(FSMState<TStateID> state)
        {
            if (CurrentState == state || state == null)
            {
                return;
            }

            var previousState = CurrentState;
            CurrentState = state;

            previousState?.OnExit(CurrentState);
            CurrentState.OnEnter(previousState);

            Debug.LogFormat("State Changed - From: {0} To: {1}", previousState == null? "None": previousState.GetID(), state.GetID());
        }

        protected void AddTransitionTo(FSMState<TStateID> state, TStateID stateType)
        {
            //We don't want to allow transition from one selve, i.e Welcome to Welcome
            if (!Compare(state.GetID(),stateType))
            {
                state.AddTransition(stateType);
            }
        }

        private bool Compare(TStateID x, TStateID y)
        {
            return EqualityComparer<TStateID>.Default.Equals(x, y);
        }
    }
}
