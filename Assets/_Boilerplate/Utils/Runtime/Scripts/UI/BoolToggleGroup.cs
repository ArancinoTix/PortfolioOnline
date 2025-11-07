using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace U9.Utils
{
    public class BoolToggleGroup : MonoBehaviour
    {
        [SerializeField] Toggle _onToggle;
        [SerializeField] Toggle _offToggle;

        public Toggle.ToggleEvent onValueChanged;

        private void Awake()
        {
            _onToggle.onValueChanged.AddListener(ToggleOnChanged);
            _offToggle.onValueChanged.AddListener(ToggleOffChanged);
        }

        private void ToggleOnChanged(bool isOn)
        {
            if(isOn)
                SetValue(isOn, true);
        }

        private void ToggleOffChanged(bool isOn)
        {
            if (isOn)
                SetValue(!isOn,true);
        }

        public void SetValue(bool isOn, bool notifyEvents)
        {
            _onToggle.SetIsOnWithoutNotify(isOn);
            _onToggle.interactable = !isOn;
            _offToggle.SetIsOnWithoutNotify(!isOn);
            _offToggle.interactable = isOn;

            if (notifyEvents)
            {
                onValueChanged?.Invoke(isOn);
            }
        }

        /// <summary>
        /// Use this if you want to have no option selected at all.
        /// </summary>
        public void ClearValue()
        {
            _onToggle.SetIsOnWithoutNotify(false);
            _offToggle.SetIsOnWithoutNotify(false);
        }

        /// <summary>
        /// Did the user pick anything? Only useful if ClearValue was used.
        /// </summary>
        /// <returns></returns>
        public bool IsAtLeastOneOptionChosen()
        {
            return _onToggle.isOn || _offToggle.isOn;
        }

        /// <summary>
        /// Is the on toggle set?
        /// </summary>
        /// <returns></returns>
        public bool IsOn()
        {
            return _onToggle.isOn && !_offToggle.isOn;
        }
    }
}