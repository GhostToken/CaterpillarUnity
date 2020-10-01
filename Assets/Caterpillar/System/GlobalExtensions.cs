using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalExtensions
{
    public static string ToIdentifier(this ECurrency Currency)
    {
        switch(Currency)
        {
            case ECurrency.Energy:
                {
                    return "PW";
                }
            case ECurrency.GhostTokens:
                {
                    return "GT";
                }
            default:
                {
                    throw new Exception("Bad Currency Type");
                }
        }
    }

    public static ECurrency FromIdentifier(this string CurrencyIdentifier)
    {
        switch(CurrencyIdentifier)
        {
            case "PW":
                {
                    return ECurrency.Energy;
                }
            case "GT":
                {
                    return ECurrency.GhostTokens;
                }
            default:
                {
                    throw new Exception("Bad Currency Identifier : " + CurrencyIdentifier);
                }
        }
    }
}
