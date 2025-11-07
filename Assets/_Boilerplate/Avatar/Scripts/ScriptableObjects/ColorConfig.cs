using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace U9.Avatar
{

    [CreateAssetMenu(fileName = "ColorConfig", menuName = "UNIT9/Avatar/Configs/Color Config", order = 1)]
    public class ColorConfig : ScriptableObject
    {
        [SerializeField] ColorSlot m_Slot;
        [SerializeField] Color m_Color;
        [SerializeField] private Sprite icon;
        public ColorSlot Slot { get => m_Slot; }
        public Color Color { get => m_Color; }
        public Sprite Icon => icon;
        public string ID
        {
            get
            {
                string slotName = m_Slot != null ? m_Slot.name : string.Empty;
                return string.Format("{0}_{1}", slotName, name);
            }
        }
    }
}
