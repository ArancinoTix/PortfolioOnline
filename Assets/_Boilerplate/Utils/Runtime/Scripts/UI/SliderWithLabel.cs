using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization;

namespace U9.Utils
{
    public class SliderWithLabel : MonoBehaviour
    {
        [SerializeField] private Slider _slider;
        [SerializeField] private TextMeshProUGUI _valueDisplayLabel;
        [Tooltip("Most values will be 0 to 1, but if it's a percentage, we need to multiple by 100")]
        [SerializeField] private float _multiplier = 1;
        [SerializeField] [Range(0,10)] private int _numberOfDecimalPlacesToDisplay = 2;

        [Tooltip("Optional. For example for a percentage, this should be \"{0}%\"")]
        [SerializeField] private LocalizedString _extraFormating;

        private int _currentNumberOfDecimalPlacesToDisplay = -1;
        private string _displayFormat;

        public int NumberOfDecimalPlacesToDisplay { 
            get => _numberOfDecimalPlacesToDisplay; 
            set => _numberOfDecimalPlacesToDisplay = value; 
        }

        public void SetMinMaxValues(float min, float max)
        {
            _slider.minValue = min;
            _slider.maxValue = max;
        }

        public Slider.SliderEvent onValueChanged => _slider.onValueChanged;

        private void Awake()
        {
            PrepDisplayFormat();
            _slider.onValueChanged.AddListener(UpdateDisplayedNumber);
        }

        public void SetValue(float value, bool notifyEvents)
        {
            if (notifyEvents)
                _slider.value = value;
            else
            {
                _slider.SetValueWithoutNotify(value);
                UpdateDisplayedNumber(value);
            }
        }

        private void PrepDisplayFormat()
        {
            if(_numberOfDecimalPlacesToDisplay != _currentNumberOfDecimalPlacesToDisplay)
            {
                //Clamp it
                _numberOfDecimalPlacesToDisplay = Mathf.Clamp(_numberOfDecimalPlacesToDisplay, 0, 10);
                _currentNumberOfDecimalPlacesToDisplay = _numberOfDecimalPlacesToDisplay;

                _displayFormat = '{' + $"0:F{_numberOfDecimalPlacesToDisplay}" + '}';  // {0:F2}

                if(!_extraFormating.IsEmpty)
                {
                    _displayFormat = _extraFormating.GetLocalizedString(_displayFormat);
                }

                UpdateDisplayedNumber(_slider.value);
            }
        }

        private void UpdateDisplayedNumber(float value)
        {
            PrepDisplayFormat();
            _valueDisplayLabel.text = string.Format(_displayFormat, value*_multiplier);
        }
    }
}