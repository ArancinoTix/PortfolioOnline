using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace U9.View
{
    public class ViewManager : MonoSingleton<ViewManager>
    {
        [SerializeField] private View[] _views;

        private Dictionary<Type, View> _viewsDictionary = new Dictionary<Type, View>();
        private bool _initted = false;

        void Awake()
        {
            Instance = this;
            Init();
        }

        public void Init()
        {
            if (_initted)
                return;

            _initted = true;

            foreach (var view in _views)
            {
                _viewsDictionary.Add(view.GetType(), view);
            }
        }
        public T GetView<T>() where T : View
        {
            Init();
            Type givenType = typeof(T);
            if (_viewsDictionary.ContainsKey(givenType))
            {
                return _viewsDictionary[givenType] as T;
            }
            return default(T);
        }
    }
}
