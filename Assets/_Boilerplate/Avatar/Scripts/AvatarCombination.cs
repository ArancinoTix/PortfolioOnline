using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace U9.Avatar
{
    [System.Serializable]
    public class AvatarCombination
    {
        [SerializeField] private PartList partList;
        [SerializeField] private ColorList colorList;

        public PartList PartList { get => partList; }
        public ColorList ColorList { get => colorList; }

        public bool HasColor()
        {
            return ColorList != null;
        }

        public bool HasParts()
        {
            return PartList != null;
        }

        public int NoOfParts()
        {
            return HasParts() ? partList.Parts.Length : 0;
        }
        public int NoOfColors()
        {
            return HasColor() ? colorList.Colors.Length : 0;
        }

        public string GetCategoryDisplayNameId()
        {
            if (HasParts())
                return partList.DisplayNameID;
            else if (HasColor())
                return colorList.DisplayNameID;
            else
                return null;
        }

        public bool IsCombo()
        {
            return HasColor() && HasParts();
        }

        public string AnimationID()
        {
            return partList.AnimationID;
        }
    }
}
