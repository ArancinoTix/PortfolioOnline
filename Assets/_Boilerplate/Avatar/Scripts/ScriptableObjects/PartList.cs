using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace U9.Avatar
{
    [CreateAssetMenu(fileName = "partList", menuName = "UNIT9/Avatar/Configs/Part List", order = 0)]
    public class PartList : ScriptableObject
    {
        [SerializeField] private string displayNameID;
        [SerializeField] private string animationID;
        [SerializeField] private PartSlot slot;
        [SerializeField] private PartConfig[] parts;

        public string DisplayNameID { get => displayNameID; }
        public string AnimationID { get => animationID; }
        public PartSlot Slot { get => slot; }
        public PartConfig[] Parts { get => parts; }
    }
}
