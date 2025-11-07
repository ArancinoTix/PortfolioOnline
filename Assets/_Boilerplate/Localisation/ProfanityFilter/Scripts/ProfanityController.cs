using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Localization
{
    public class ProfanityController : MonoSingleton<ProfanityController>
    {
        [SerializeField] private ProfanityFilterSO english;
        [SerializeField] private ProfanityFilterSO french;
        [SerializeField] private ProfanityFilterSO spanish;

        private ProfanityFilterSO filter;

        private void Awake()
        {
            Instance = this;
        }

        public void SetFilter(ProfanityLanguage languageCode)
        {
            switch (languageCode)
            {
                case ProfanityLanguage.English:
                    Debug.Log("Filter set to eng");
                    filter = english;
                    break;
                case ProfanityLanguage.French:
                    Debug.Log("Filter set to fr");
                    filter = french;
                    break;
                case ProfanityLanguage.Spanish:
                    Debug.Log("Filter set to es");
                    filter = spanish;
                    break;
                default:
                    Debug.Log("Filter set to defualt es");
                    filter = spanish;
                    break;
            }
            
        }

        public bool ValidateComplex(string input, bool whiteList = false)
        {
            //Debug.Log("Validate: " + input + " : " + filter.ValidateComplex(input, whiteList));
            return filter.ValidateComplex(input, whiteList);
        }

        public string Replace(string input, bool useWhiteList = false, char replaceChar = '*')
        {
            return filter.Replace(input, useWhiteList, replaceChar);
        }
    }
}

public enum ProfanityLanguage
{
    English = 0,
    French = 1,
    Spanish = 2
}