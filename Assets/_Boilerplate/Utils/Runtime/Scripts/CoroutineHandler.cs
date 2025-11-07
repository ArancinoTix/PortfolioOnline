using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace U9.Utils
{
    public class CoroutineHandler : MonoBehaviour
    {
        static CoroutineHandler instance;

        public static CoroutineHandler GetInstance(bool createIfNull)
        {
            if (instance == null && createIfNull)
            {
                GameObject go = new GameObject("CoroutineHandler");
                instance = go.AddComponent<CoroutineHandler>();
            }

            return instance;
        }

        // Use this for initialization
        void Start()
        {
            instance = this;
        }

        private void OnApplicationQuit()
        {
            instance = null;
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            instance = null;
        }
    }
}
