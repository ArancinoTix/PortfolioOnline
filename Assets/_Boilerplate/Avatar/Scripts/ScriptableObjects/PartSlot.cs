using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace U9.Avatar
{
    [CreateAssetMenu(fileName = "PartSlot", menuName = "UNIT9/Avatar/Slots/Part Slot", order = 0)]
    public class PartSlot : ScriptableObject
    {
        [SerializeField] string m_PartName;

        public string Name { get => m_PartName; }
        //Serves as an enum style comparitor
    }
}