using UnityEngine;
using System.Collections;
using PlayFab.ClientModels;

public static class PlayfabHelpers
{
    public static int TryGetInStatistics(this GetPlayerStatisticsResult Result, string Identifier, int PreviousValue)
    {
        StatisticValue statistic = Result.Statistics.Find(T => T.StatisticName == Identifier);
        if (statistic != null)
        {
            return statistic.Value;
        }
        return PreviousValue;
    }

    public static float TryGetInStatistics(this GetPlayerStatisticsResult Result, string Identifier, float PreviousValue)
    {
        StatisticValue statistic = Result.Statistics.Find(T => T.StatisticName == Identifier);
        if (statistic != null)
        {
            return statistic.Value;
        }
        return PreviousValue;
    }

    public static string TryGetInUserDatas(this GetUserDataResult Result, string Identifier, string PreviousValue)
    {
        if (Result.Data.ContainsKey(Identifier) == true)
        {
            return Result.Data[Identifier].Value;
        }
        return PreviousValue;
    }

    public static int TryGetInUserDatas(this GetUserDataResult Result, string Identifier, int PreviousValue)
    {
        if (Result.Data.ContainsKey(Identifier) == true)
        {
            return int.Parse(Result.Data[Identifier].Value);
        }
        return PreviousValue;
    }

    public static float TryGetInUserDatas(this GetUserDataResult Result, string Identifier, float PreviousValue)
    {
        if (Result.Data.ContainsKey(Identifier) == true)
        {
            return float.Parse(Result.Data[Identifier].Value);
        }
        return PreviousValue;
    }
}