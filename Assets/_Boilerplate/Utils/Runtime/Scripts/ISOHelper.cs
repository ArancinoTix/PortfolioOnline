using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_IOS
using System.Runtime.InteropServices;
#endif

public class ISOHelper
{
#if UNITY_IOS
    [DllImport("__Internal")]
    extern static public string _getPreferenceLanguageString(); 
    
#endif

    static string[] GetSupportedLanguageCodes()
    {
        string[] validCodes = new string[]
        {
           "af",                     // Afrikaans                        
           "sq",                     // Albanian                         
           "ar-dz",                  // Arabic (Algeria)                 
           "ar-bh",                  // Arabic (Bahrain)                 
           "ar-eg",                  // Arabic (Egypt)                   
           "ar-iq",                  // Arabic (Iraq)                    
           "ar-jo",                  // Arabic (Jordan)                  
           "ar-kw",                  // Arabic (Kuwait)                  
           "ar-lb",                  // Arabic (Lebanon)                 
           "ar-ly",                  // Arabic (Libya)                   
           "ar-ma",                  // Arabic (Morocco)                 
           "ar-om",                  // Arabic (Oman)                    
           "ar-qa",                  // Arabic (Qatar)                   
           "ar-sa",                  // Arabic (Saudi Arabia)            
           "ar-sy",                  // Arabic (Syria)                   
           "ar-tn",                  // Arabic (Tunisia)                 
           "ar-ae",                  // Arabic (U.A.E.)                  
           "ar-ye",                  // Arabic (Yemen)                   
           "eu",                     // Basque                           
           "be",                     // Belarusian                       
           "bg",                     // Bulgarian                        
           "ca",                     // Catalan                          
           "zh-hk",                  // Chinese (Hong Kong)              
           "zh-cn",                  // Chinese (PRC)                    
           "zh-sg",                  // Chinese (Singapore)              
           "zh-tw",                  // Chinese (Taiwan)                 
           "hr",                     // Croatian                         
           "cs",                     // Czech                            
           "da",                     // Danish                           
           "nl-be",                  // Dutch (Belgium)                  
           "nl",                     // Dutch (Standard)                 
           "en",                     // English                          
           "en-au",                  // English (Australia)              
           "en-bz",                  // English (Belize)                 
           "en-ca",                  // English (Canada)                 
           "en-ie",                  // English (Ireland)                
           "en-jm",                  // English (Jamaica)                
           "en-nz",                  // English (New Zealand)            
           "en-za",                  // English (South Africa)           
           "en-tt",                  // English (Trinidad)               
           "en-gb",                  // English (United Kingdom)         
           "en-us",                  // English (United States)          
           "et",                     // Estonian                         
           "fo",                     // Faeroese                         
           "fa",                     // Farsi                            
           "fi",                     // Finnish                          
           "fr-be",                  // French (Belgium)                 
           "fr-ca",                  // French (Canada)                  
           "fr-lu",                  // French (Luxembourg)              
           "fr",                     // French (Standard)                
           "fr-ch",                  // French (Switzerland)             
           "gd",                     // Gaelic (Scotland)                
           "de-at",                  // German (Austria)                 
           "de-li",                  // German (Liechtenstein)           
           "de-lu",                  // German (Luxembourg)              
           "de",                     // German (Standard)                
           "de-ch",                  // German (Switzerland)             
           "el",                     // Greek                            
           "he",                     // Hebrew                           
           "hi",                     // Hindi                            
           "hu",                     // Hungarian                        
           "is",                     // Icelandic                        
           "id",                     // Indonesian                       
           "ga",                     // Irish                            
           "it",                     // Italian (Standard)               
           "it-ch",                  // Italian (Switzerland)            
           "ja",                     // Japanese                         
           "ko",                     // Korean                           
           "ko",                     // Korean (Johab)                   
           "ku",                     // Kurdish                          
           "lv",                     // Latvian                          
           "lt",                     // Lithuanian                       
           "mk",                     // Macedonian (FYROM)               
           "ml",                     // Malayalam                        
           "ms",                     // Malaysian                        
           "mt",                     // Maltese                          
           "no",                     // Norwegian                        
           "nb",                     // Norwegian (Bokmål)               
           "nn",                     // Norwegian (Nynorsk)              
           "pl",                     // Polish                           
           "pt-br",                  // Portuguese (Brazil)              
           "pt",                     // Portuguese (Portugal)            
           "pa",                     // Punjabi                          
           "rm",                     // Rhaeto-Romanic                   
           "ro",                     // Romanian                         
           "ro-md",                  // Romanian (Republic of Moldova)   
           "ru",                     // Russian                          
           "ru-md",                  // Russian (Republic of Moldova)    
           "sr",                     // Serbian                          
           "sk",                     // Slovak                           
           "sl",                     // Slovenian                        
           "sb",                     // Sorbian                          
           "es-ar",                  // Spanish (Argentina)              
           "es-bo",                  // Spanish (Bolivia)                
           "es-cl",                  // Spanish (Chile)                  
           "es-co",                  // Spanish (Colombia)               
           "es-cr",                  // Spanish (Costa Rica)             
           "es-do",                  // Spanish (Dominican Republic)     
           "es-ec",                  // Spanish(Ecuador)     
           "es-sv",                  // Spanish(El Salvador)             
           "es-gt",                  // Spanish(Guatemala)               
           "es-hn",                  // Spanish(Honduras)                
           "es-mx",                  // Spanish(Mexico)                  
           "es-ni",                  // Spanish(Nicaragua)               
           "es-pa",                  // Spanish(Panama)                  
           "es-py",                  // Spanish(Paraguay)                
           "es-pe",                  // Spanish(Peru)                    
           "es-pr",                  // Spanish(Puerto Rico)             
           "es",                     // Spanish(Spain)                   
           "es-uy",                  // Spanish(Uruguay)                 
           "es-ve",                  // Spanish(Venezuela)               
           "sv",                     // Swedish                          
           "sv-fi",                  // Swedish(Finland)                 
           "th",                     // Thai                             
           "ts",                     // Tsonga                           
           "tn",                     // Tswana                           
           "tr",                     // Turkish                          
           "uk",                     // Ukrainian                        
           "ur",                     // Urdu                             
           "ve",                     // Venda                            
           "vi",                     // Vietnamese                       
           "cy",                     // Welsh                            
           "xh",                     // Xhosa                            
           "ji",                     // Yiddish                          
           "zu"                      // Zulu        
        };

        return validCodes;
    }

    public static void GetLanguageAndCountryCodes(out string language, out string country, string defaultLanguage, string defaultCountry)
    {
        language = defaultLanguage;
        country = defaultCountry;
#if UNITY_EDITOR

#elif UNITY_ANDROID

        using (AndroidJavaClass cls = new AndroidJavaClass("java.util.Locale"))
        {
            if (cls != null)
            {
                using (AndroidJavaObject locale = cls.CallStatic<AndroidJavaObject>("getDefault"))
                {
                    if (locale != null)
                    {
                        language = locale.Call<string>("getLanguage").ToLower();
                        country = locale.Call<string>("getCountry").ToLower();      
                    }
                }
            }
        }
#elif UNITY_IOS
        language = _getPreferenceLanguageString().ToLower();
        string[] split = language.Split('-');
        country = split[split.Length - 1];    
        
        language = split[0];    
#endif

        string combinedLanguageCode = string.Format("{0}-{1}", language, country);
        Debug.Log("Detected Language: " + combinedLanguageCode);
        
        bool isSingleValid = false;
        bool isCombinedValid = false;
        string[] validCodes = GetSupportedLanguageCodes();

        foreach(string s in validCodes)
        {
            if (s == language)
                isSingleValid = true;

            if (s == combinedLanguageCode)
                isCombinedValid = true;

            if (isSingleValid && isCombinedValid)
                break;
        }

        if (isCombinedValid)
        {
            Debug.Log("Combined is valid");
            language = combinedLanguageCode;
        }
        else if (!isSingleValid)
        {
            Debug.Log("Single is not valid");
            language = string.Format("{0}-{1}", defaultLanguage, defaultCountry);
        }



#if UNITY_EDITOR
        //language = "en-us";
        //language = "fr-fr";
        //language = "es-es";
        //language = "ca-ca";
#endif

        // L: en-gb, C: en
        Debug.Log("L: " + language + ", C: " + country);
    }

}
