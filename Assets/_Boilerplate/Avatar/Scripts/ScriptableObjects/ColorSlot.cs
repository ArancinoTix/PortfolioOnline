using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace U9.Avatar
{
    [CreateAssetMenu(fileName = "PartConfig", menuName = "UNIT9/Avatar/Slots/Color Slot", order = 1)]
    public class ColorSlot : ScriptableObject
    {
        [SerializeField] string m_PartName;

        [SerializeField] string m_ShaderColorName = "_Color";

        public string ShaderColorName { get => m_ShaderColorName; }
        public string Name { get => m_PartName; }
    }
}
