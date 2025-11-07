using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Components;

namespace U9.Utils
{
    public class ButtonWithLabel : Button
    {
        [SerializeField] private LocalizeStringEvent _label;

        public LocalizeStringEvent Label { get => _label; }
    }
}
