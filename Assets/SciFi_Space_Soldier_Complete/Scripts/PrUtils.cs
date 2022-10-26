using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PrUtils{



    public static string floatToTimerString(float timer)
    {
        string minString = Mathf.Floor(timer / 60).ToString("00");
        string secString = Mathf.Floor(timer % 60).ToString("00");

        string finalTimer = minString + ":" + secString;
        return finalTimer; 
    }
}
