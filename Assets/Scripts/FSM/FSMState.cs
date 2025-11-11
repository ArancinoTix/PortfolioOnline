namespace U9.FSM
{
    using System.Collections.Generic;
    using UnityEngine;
    

    public class FSMState<TStateID> where TStateID: System.IConvertible 
    {
        private List<FSMBehaviour<TStateID>> _behaviours;
        private List<IUpdatable> _updatableBehaviours;
        private TStateID _id;
        private List<TStateID> _transitionList;
        private FSM<TStateID> _owner;

        public TStateID GetID() => _id;

        /// <summary>
        /// Initializes a new instance of the <see cref="Common.FSM.FSMState"/> class.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="owner">Owner.</param>
        public FSMState(TStateID name, FSM<TStateID> owner)
        {
            _id = name;
            _owner = owner;
            _transitionList = new List<TStateID>();
            _behaviours = new List<FSMBehaviour<TStateID>>();
            _updatableBehaviours = new List<IUpdatable>();
        }

        /// <summary>
        /// Adds the transition.
        /// </summary>
        public void AddTransition(TStateID destinationState)
        {
            if (_transitionList.Contains(destinationState))
            {
                Debug.LogError(string.Format("state {0} already contains transition for {1}", this._id, destinationState));
                return;
            }

            _transitionList.Add(destinationState);
        }
        
        /// <summary>
        ///     Get all transitions
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TStateID> GetTransitions()
        {
            return _transitionList;
        }

        /// <summary>
        /// Checks if transition exists.
        /// </summary>
        public bool HasTransitionFor(TStateID stateId) {
            return _transitionList.Contains(stateId);
        }

        /// <summary>
        /// Adds the behaviour.
        /// </summary>
        public void AddBehaviour(FSMBehaviour<TStateID> behaviour)
        {
            if (_behaviours.Contains(behaviour))
            {
                Debug.LogError("This state already contains " + behaviour);
                return;
            }

            if (behaviour.GetOwner() != this)
            {
                Debug.LogError("This state doesn't own " + behaviour);
            }

            _behaviours.Add(behaviour);

            if (behaviour is IUpdatable)
                _updatableBehaviours.Add((IUpdatable)behaviour);
        }

        public void ChangeState(TStateID stateId)
        {
            this._owner.ChangeState(stateId);
        }

        public void OnEnter(FSMState<TStateID> previousState)
        {
            //Debug.Log("OnEnter " + this.m_id);
            foreach (var a in _behaviours)
            {
                a.OnEnter(previousState);
            }
        }

        public void OnExit(FSMState<TStateID> nextState)
        {
            //Debug.Log("OnExit " + this.m_id);
            foreach (var a in _behaviours)
            {                
                a.OnExit(nextState);
            }
        }

        public void OnUpdate()
        {
            foreach(var a in _updatableBehaviours)
            {
                a.Update();
            }
        }
    }
}