using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace U9.Avatar
{
    [CreateAssetMenu(fileName = "colorList", menuName = "UNIT9/Avatar/Configs/Color List", order = 0)]
    public class ColorList : ScriptableObject
    {
        [SerializeField] private string displayNameID;
        [SerializeField] private ColorSlot slot;
        [SerializeField] private ColorConfig[] colors;

        public string DisplayNameID { get => displayNameID; }
        public ColorSlot Slot { get => slot; }
        public ColorConfig[] Colors { get => colors; }
    }
}
