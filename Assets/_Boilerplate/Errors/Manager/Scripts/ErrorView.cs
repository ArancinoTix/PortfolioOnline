using System.Collections;
using System.Collections.Generic;
using U9.View;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Components;
using U9.Utils;
using U9.Errors.Codes;
using System;
using TMPro;
using UnityEngine.Localization;

namespace U9.Errors
{
    public class ErrorView :View.View
    {
        [SerializeField] private Image _iconImage;

        [SerializeField] private LocalizedString _errorCopy;
        [SerializeField] private TextMeshProUGUI _errorCodeLabel;
        [SerializeField] private LocalizeStringEvent _headerString;
        [SerializeField] private LocalizeStringEvent _bodyString;
        [SerializeField] private LocalizedString _defaultChoiceCopy;
        [SerializeField] private ButtonWithLabel[] _choiceButtons;

        public Action<int> onChoiceSelected;

        private int _defaultButtonIndex;

        protected override void Awake()
        {
            base.Awake();
        }

        public void Configure(int code, ResponseCodeOption response)
        {
            AttemptInitView();
            _errorCodeLabel.text = _errorCopy.GetLocalizedString(code);
            _headerString.StringReference = response.Title;
            _bodyString.StringReference = response.Message;

            _iconImage.enabled = response.Icon != null;
            _iconImage.sprite = response.Icon;

            int maxChoiceCount = _choiceButtons.Length;

            //We can only display a set number of choices. 
            int numberOfCustomChoices = Mathf.Clamp(response.CustomChoices.Length,0, maxChoiceCount);

            //If we need to display the default, we need to make sure we have a button left over
            if(response.ShowDefaultCloseChoice && numberOfCustomChoices >= maxChoiceCount)
            {
                numberOfCustomChoices--;

                if (numberOfCustomChoices < 0)
                    numberOfCustomChoices = 0;
            }

            //Set the custom choices
            for(int i = 0; i<numberOfCustomChoices; i++)
            {
                var button = _choiceButtons[i];

                button.Label.StringReference = response.CustomChoices[i];
                button.gameObject.SetActive(true);
            }

            //Set the default if needed
            if (response.ShowDefaultCloseChoice)
            {
                _defaultButtonIndex = numberOfCustomChoices;

                var button = _choiceButtons[numberOfCustomChoices];

                button.Label.StringReference = _defaultChoiceCopy;
                button.gameObject.SetActive(true);

                //Increment so we do not take this into account in the next step
                numberOfCustomChoices++;
            }
            else
                _defaultButtonIndex = -1;

            //Now to disable the rest. 
            for(int i = numberOfCustomChoices; i < maxChoiceCount; i++)
            {
                _choiceButtons[numberOfCustomChoices].gameObject.SetActive(false);
            }
        }

        protected override void EnableInteractionInternal()
        {
            base.EnableInteractionInternal();

            for(int i = 0, ni = _choiceButtons.Length; i<ni; i++)
            {
                int buttonIndex = i; //Need to store a new struct for the lambda
                var button = _choiceButtons[i];
                button.onClick.AddListener(() => 
                {
                    //Invoke the event. We check if this was the default option, and if so, mark it as -1
                    onChoiceSelected?.Invoke(buttonIndex == _defaultButtonIndex ? -1 : buttonIndex);
                });
            }
        }

        protected override void DisableInteractionInternal()
        {
            base.DisableInteractionInternal();

            for (int i = 0, ni = _choiceButtons.Length; i < ni; i++)
            {
                var button = _choiceButtons[i];
                button.onClick.RemoveAllListeners();
            }
        }
    }
}
