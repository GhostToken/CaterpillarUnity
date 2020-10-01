using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerPrefsHelpers
{
    public static int TryGet(string Identifier, int PreviousValue)
    {
        if (PlayerPrefs.HasKey(Identifier))
        {
            return PlayerPrefs.GetInt(Identifier);
        }
        return PreviousValue;
    }

    public static float TryGet(string Identifier, float PreviousValue)
    {
        if (PlayerPrefs.HasKey(Identifier))
        {
            return PlayerPrefs.GetFloat(Identifier);
        }
        return PreviousValue;
    }

    public static string TryGet(string Identifier, string PreviousValue)
    {
        if (PlayerPrefs.HasKey(Identifier))
        {
            return PlayerPrefs.GetString(Identifier);
        }
        return PreviousValue;
    }
}
