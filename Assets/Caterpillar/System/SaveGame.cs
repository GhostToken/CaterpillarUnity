using JetBrains.Annotations;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveGame : MonoBehaviour
{
    #region Identifiers

    static public string AllTimesTotalScoreIdentifier
    {
        get
        {
            return "All Times Meal Points";
        }
    }

    static public string BiggestScoreIdentifier
    {
        get
        {
            return "Biggest Meal Score";
        }
    }

    static public string MaxLevelReachedIdentifier
    {
        get
        {
            return "Max Level Reached";
        }
    }

    #endregion

    #region Data Structures

    [Serializable]
    public class LevelRecord
    {
        #region Properties

        public int Id = 0;
        public int Stars = 0;
        public int Score = 0;

        public bool Finished
        {
            get
            {
                return (Score > 0);
            }
        }

        #endregion

        #region Constructors

        public LevelRecord(int LevelId)
        {
            Id = LevelId;
        }

        public LevelRecord(int LevelId, int NewStars, int NewScore)
        {
            Id = LevelId;
            Stars = NewStars;
            Score = NewScore;
        }

        #endregion

        #region Method

        public void Record(int NewStars, int NewScore)
        {
            Debug.Log("Record Level " + Id + " Starts : " + Stars + " Score : " + Score);
            if (NewStars > Stars)
            {
                Stars = NewStars;
                SendStars();
            }
            Stars = Mathf.Max(Stars, NewStars);
            Score = Mathf.Max(Score, NewScore);
            SendScore(NewScore);
        }

        public void Save_LocalStorage()
        {
            PlayerPrefs.SetInt(StarsIdentifier, Stars);
            PlayerPrefs.SetInt(ScoreIdentifier, Score);
            Debug.Log("Save Level " + Id + " Starts : " + Stars + " Score : " + Score);
        }

        public void Load_LocalStorage()
        {
            if(PlayerPrefs.HasKey(StarsIdentifier))
            {
                Stars = PlayerPrefs.GetInt(StarsIdentifier);
            }
            if (PlayerPrefs.HasKey(ScoreIdentifier))
            {
                Score = PlayerPrefs.GetInt(ScoreIdentifier);
            }
            Debug.Log("Load Level " + Id + " Starts : " + Stars + " Score : " + Score);
        }

        #endregion

        #region Identifiers

        public string StarsIdentifier
        {
            get
            {
                return "Stars_" + Id.ToString("0000");
            }
        }

        public string ScoreIdentifier
        {
            get
            {
                return "Score_" + Id.ToString("0000");
            }
        }

        public string LeaderboardIdentifier
        {
            get
            {
                return "Level " + Id.ToString() + " Best Score";
            }
        }

        #endregion

        #region Playfab Push Calls

        void SendStars()
        {
            PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
            {
                Data = new Dictionary<string, string>() 
                {
                    {StarsIdentifier, Stars.ToString()}
                }
            },
            result =>
            {
                Debug.Log("Successfully updated star count for level " + Id);
            },
            error => 
            {
                Debug.Log("Got error updating star count for level " + Id);
                Debug.Log(error.GenerateErrorReport());
            });
        }

        void SendScore(int NewScore)
        {
            PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
            {
                Statistics = new List<StatisticUpdate> 
                {
                    new StatisticUpdate 
                    {
                        StatisticName = LeaderboardIdentifier,
                        Value = Score
                    },
                    new StatisticUpdate
                    {
                        StatisticName = AllTimesTotalScoreIdentifier,
                        Value = NewScore
                    },
                    new StatisticUpdate
                    {
                        StatisticName = BiggestScoreIdentifier,
                        Value = NewScore
                    }
                }
            },
            result =>
            {
                Debug.Log("Successfully updated score stats for level " + Id);
            },
            error =>
            {
                Debug.Log("Got error updating score stats for level " + Id);
                Debug.Log(error.GenerateErrorReport());
            });
        }

        #endregion

        #region Playfab Pull Call

        public void OnGetLocalPlayerStatistics(GetPlayerStatisticsResult Result)
        {
            Score = Result.TryGetInStatistics(LeaderboardIdentifier, Score);
        }

        public void OnGetLocalUserData(GetUserDataResult Result)
        {
            Stars = Result.TryGetInUserDatas(StarsIdentifier, Stars);
        }

        #endregion
    }

    #endregion

    #region Properties

    public static int MaxLevelReached = 1;
    public static int AllTimesTotalScore = 0;
    public static int BiggestScore = 0;
    public static List<LevelRecord> AllLevelRecords = new List<LevelRecord>();

    #endregion

    #region Unity Methods

    // Start is called before the first frame update
    void Start()
    {
        Load_LocalStorage();
        DontDestroyOnLoad(this.gameObject);
    }

    private void OnApplicationQuit()
    {
        Save_LocalStorage();
    }

    #endregion

    #region Private Methods

    private void Load_LocalStorage()
    {
        MaxLevelReached = TryGetPlayerPrefs(MaxLevelReachedIdentifier, MaxLevelReached);
        AllTimesTotalScore = TryGetPlayerPrefs(AllTimesTotalScoreIdentifier, AllTimesTotalScore);
        BiggestScore = TryGetPlayerPrefs(BiggestScoreIdentifier, BiggestScore);

        for (int Level = 1; Level <= MaxLevelReached; ++Level)
        {
            LevelRecord Record = new LevelRecord(Level);
            Record.Load_LocalStorage();
            AllLevelRecords.Add(Record);
        }
    }

    static private void Save_LocalStorage()
    {
        PlayerPrefs.SetInt(MaxLevelReachedIdentifier, MaxLevelReached );
        PlayerPrefs.SetInt(AllTimesTotalScoreIdentifier, AllTimesTotalScore);
        PlayerPrefs.SetInt(BiggestScoreIdentifier, BiggestScore);

        foreach (LevelRecord Record in AllLevelRecords)
        {
            Record.Save_LocalStorage();
        }
    }

    static private LevelRecord GetRecord(int LevelId)
    {
        LevelRecord record = AllLevelRecords.Find(T => T.Id == LevelId);
        if (record == null)
        {
            record = new LevelRecord(LevelId);
            AllLevelRecords.Add(record);
        }
        return record;
    }

    #endregion

    #region Static Public Methods

    public static void RecordCurrentGame()
    {
        int LevelId = Level.CurrentLevel.Id;

        LevelRecord Record = GetRecord(LevelId);
        Record.Record(Partie.Stars, Partie.Score);

        MaxLevelReached = Mathf.Max(MaxLevelReached, LevelId);
        AllTimesTotalScore += Partie.Score;
        BiggestScore = Mathf.Max(Partie.Score, BiggestScore);

        // Saving
        Record.Save_LocalStorage();
        Save_LocalStorage();
    }

    public static int GetStars(int LevelId)
    {
        if( LevelId <= MaxLevelReached)
        {
            LevelRecord Record = AllLevelRecords.Find(T => T.Id == LevelId);
            if(Record != null)
            {
                return Record.Stars;
            }
        }
        return 0;
    }

    public static int GetScore(int LevelId)
    {
        if (LevelId <= MaxLevelReached)
        {
            LevelRecord Record = AllLevelRecords.Find(T => T.Id == LevelId);
            if (Record != null)
            {
                return Record.Score;
            }
        }
        return 0;
    }

    #endregion

    #region Playfab Pull Call

    public static void OnGetLocalPlayerStatistics(GetPlayerStatisticsResult Result)
    {
        AllTimesTotalScore = Result.TryGetInStatistics(AllTimesTotalScoreIdentifier, AllTimesTotalScore);
        BiggestScore = Result.TryGetInStatistics(BiggestScoreIdentifier, BiggestScore);
    }

    public static void OnGetLocalUserData(GetUserDataResult Result)
    {
        MaxLevelReached = Result.TryGetInUserDatas(MaxLevelReachedIdentifier, MaxLevelReached);

        for (int LevelId = 1; LevelId <= MaxLevelReached; ++LevelId)
        {
            LevelRecord Record = GetRecord(LevelId);
            Record.OnGetLocalUserData(Result);
        }
    }

    #endregion

    #region PlayerPrefs Helpers

    int TryGetPlayerPrefs(string Identifier, int PreviousValue)
    {
        if (PlayerPrefs.HasKey(Identifier))
        {
            return PlayerPrefs.GetInt(Identifier);
        }
        return PreviousValue;
    }

    float TryGetPlayerPrefs(string Identifier, float PreviousValue)
    {
        if (PlayerPrefs.HasKey(Identifier))
        {
            return PlayerPrefs.GetFloat(Identifier);
        }
        return PreviousValue;
    }

    string TryGetPlayerPrefs(string Identifier, string PreviousValue)
    {
        if (PlayerPrefs.HasKey(Identifier))
        {
            return PlayerPrefs.GetString(Identifier);
        }
        return PreviousValue;
    }

    #endregion
}