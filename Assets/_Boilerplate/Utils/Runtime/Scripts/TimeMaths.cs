using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace U9.Utils
{
    public class TimeMaths : MonoBehaviour
    {
        const int k_SecondsInAnHours = 3600;
        const int k_SecondsInAMinute = 60;
        const int k_MillisecondsInASecond = 1000;

        public static void SecondsToHMSMS(float totalSeconds, out int hours, out int minutes, out int seconds, out int milliseconds)
        {
            hours = Mathf.FloorToInt(totalSeconds / k_SecondsInAnHours); 
            minutes = Mathf.FloorToInt((totalSeconds % k_SecondsInAnHours) /k_SecondsInAMinute);
            seconds = Mathf.FloorToInt(totalSeconds) % k_SecondsInAMinute;
            milliseconds = Mathf.FloorToInt((totalSeconds * k_MillisecondsInASecond) % k_MillisecondsInASecond);
        }
    }
}
