using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace U9.SaveDataManagement
{
    /// <summary>
    /// Json Utility compatible
    /// </summary>
    [System.Serializable]
    public abstract class BaseSaveDataStructure
    {
        [SerializeField] private int _saveSlot = 0;

        public int SaveSlot { get => _saveSlot; set => _saveSlot = value; }

        /// <summary>
        /// Converts this class into a string.
        /// Override if you need a custom implementation
        /// </summary>
        /// <returns></returns>
        public virtual string Serialize()
        {
            return JsonUtility.ToJson(this);
        }

        public virtual void Deserialize(string serializedString)
        {
            JsonUtility.FromJsonOverwrite(serializedString, this);
        }

        public virtual string GetId()
        {
            return GetType().ToString();
        }
    }
}
