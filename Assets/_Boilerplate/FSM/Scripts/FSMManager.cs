using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace U9.FSM
{
    public class FSMManager : MonoSingleton<FSMManager>
    {
        private Dictionary<Type, IUpdatable> _stateMachinesDictionary = new Dictionary<Type, IUpdatable>();

        private void Awake()
        {
            Instance = this;
        }
        public T GetFSM<T, TStateID>()
          where T : FSM<TStateID>, new()
          where TStateID : System.IConvertible
        {
            Type givenType = typeof(T);
            if (_stateMachinesDictionary.ContainsKey(givenType))
            {
                return _stateMachinesDictionary[givenType] as T;
            }
            else
            {
                Debug.Log($"### FSMManager: Registering new FSM {givenType}");

                var newFSM = new T();
                _stateMachinesDictionary.Add(givenType, newFSM);

                return newFSM;
            }
        }
    }
}