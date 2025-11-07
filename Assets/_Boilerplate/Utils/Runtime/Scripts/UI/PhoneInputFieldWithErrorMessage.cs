using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using U9.CountryDetails;

namespace U9.Utils
{
    public class PhoneInputFieldWithErrorMessage : InputFieldWithErrorMessage
    {
        [SerializeField] private TextMeshProUGUI _countryCodeLabel;


        public void SetCountryCode(CountryDetailsOption countryOption)
        {
            _countryCodeLabel.text = $"+{countryOption.PhoneCode}";
        }
    }
}