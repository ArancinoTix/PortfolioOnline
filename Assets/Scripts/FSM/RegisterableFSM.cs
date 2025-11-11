using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace U9.FSM
{
    public class RegisterableFSM<T, TStateID> : FSM<TStateID>
        where T : FSM<TStateID>, new()
        where TStateID : System.IConvertible
    {        
        public static T Get()
        {
            if (FSMManager.Instance != null)
                return FSMManager.Instance.GetFSM<T, TStateID>();
            else
                return new T();
        }
    }
}
