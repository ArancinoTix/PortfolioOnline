using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

#if UNITY_EDITOR
using System.Linq;
#endif

namespace U9.CountryDetails
{
    [CreateAssetMenu(fileName = "AvailableCountryDetails", menuName = "UNIT9/Localisation/CountryCode/Available Country Details")]
    public class AvailableCountryDetailsOptions : ScriptableObject
    {
        [SerializeField] private CountryDetailsOption _defaultOption;
        [SerializeField] private CountryDetailsOption[] _options;

        public CountryDetailsOption[] Options { get => _options; }
        public CountryDetailsOption DefaultOption { get => _defaultOption; }

        /// <summary>
        /// Try to get a country detail based on a locale
        /// </summary>
        /// <param name="locale"></param>
        /// <returns></returns>
        public CountryDetailsOption GetCountryCode(Locale locale)
        {
            foreach(var option in _options)
            {
                if (locale.Identifier.Code.Contains(option.CountryCode))
                    return option;
            }

            return _defaultOption;
        }

        /// <summary>
        /// Try to get a country code detail based on a localeCode, for example en-GB.
        /// In this case we will try to find an option that has GB.
        /// </summary>
        /// <param name="localeCode"></param>
        /// <returns></returns>
        public CountryDetailsOption GetCountryCode(string localeCode)
        {
            foreach (var option in _options)
            {
                if (localeCode.Contains(option.CountryCode))
                    return option;
            }

            return _defaultOption;
        }

        public CountryDetailsOption GetCountryCode(int id)
        {
            if(_options[id]!=null)
                return _options[id];

            return _defaultOption;
        }


#if UNITY_EDITOR
        public void AddOptions(CountryDetailsOption[] options)
        {
            List<CountryDetailsOption> allOptions = new List<CountryDetailsOption>(_options);

            foreach(var option in options)
            {
                if (!allOptions.Contains(option))
                    allOptions.Add(option);
            }

            allOptions.OrderBy(x => x.name);

            _options = allOptions.ToArray();
        }
#endif
    }
}
