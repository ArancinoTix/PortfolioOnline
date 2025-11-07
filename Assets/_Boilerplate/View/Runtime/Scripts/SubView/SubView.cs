using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using VisualInspector;
using HeaderAttribute = VisualInspector.HeaderAttribute;

namespace U9.View
{
    public class SubView : View
    {
        [Header("Name")]
        [Order(-90)]
        [SerializeField] [Separator] private LocalizedString _localisedDisplayName;

        public LocalizedString LocalisedDisplayName { get => _localisedDisplayName; }
    }
}
