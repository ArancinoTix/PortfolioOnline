using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

namespace U9.CountryDetails
{
    [CreateAssetMenu(fileName = "CountryDetails", menuName = "UNIT9/Localisation/CountryCode/Country Details")]
    public class CountryDetailsOption : ScriptableObject
    {
        [SerializeField] private string _countryCode;
        [SerializeField] private string _phoneCode;
        [SerializeField] private Sprite _smallFlagIcon;
        [SerializeField] private Sprite _largeFlagIcon;

        [Tooltip("If not set, this will default to the SO file name")]
        [SerializeField] private LocalizedString _localizedDisplayName;

        public string CountryCode { get => _countryCode; }
        public string PhoneCode { get => _phoneCode;  }
        public string DisplayName { get => _localizedDisplayName.IsEmpty ? name : _localizedDisplayName.GetLocalizedString();  }
        public Sprite SmallFlagIcon { get => _smallFlagIcon; }
        public Sprite LargeFlagIcon { get => _largeFlagIcon;  }

#if UNITY_EDITOR

        public void SetDetails(string countryCode, string phoneCode)
        {
            _countryCode = countryCode;
            _phoneCode = phoneCode;
        }

#endif
    }
}
