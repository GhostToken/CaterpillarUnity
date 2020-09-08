using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveGame : MonoBehaviour
{
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
            Stars = Mathf.Max(Stars, NewStars);
            Score = Mathf.Max(Score, NewScore);
            Debug.Log("Record Level " + Id + " Starts : " + Stars + " Score : " + Score);
        }

        public void Save()
        {
            PlayerPrefs.SetInt(StarsIdentifier, Stars);
            PlayerPrefs.SetInt(ScoreIdentifier, Score);
            Debug.Log("Save Level " + Id + " Starts : " + Stars + " Score : " + Score);
        }

        public void Load()
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

        #region Identifier

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

        #endregion
    }

    #region Properties

    public static List<LevelRecord> AllLevelRecords = new List<LevelRecord>();
    public static int MaxLevelReached;

    #endregion

    #region Unity Methods

    // Start is called before the first frame update
    void Start()
    {
        Load();
        DontDestroyOnLoad(this.gameObject);
    }

    private void OnApplicationQuit()
    {
        Save();
    }

    #endregion

    #region Methods

    private void Load()
    {
        if (PlayerPrefs.HasKey("MaxLevelReached"))
        {
            MaxLevelReached = PlayerPrefs.GetInt("MaxLevelReached");
            Debug.Log("load with max level " + MaxLevelReached);
        }
        for (int Level = 1; Level <= MaxLevelReached; ++Level)
        {
            LevelRecord Record = new LevelRecord(Level);
            Record.Load();
            AllLevelRecords.Add(Record);
        }
    }

    private static void Save()
    {
        Debug.Log("Save with max level " + MaxLevelReached);
        PlayerPrefs.SetInt("MaxLevelReached", MaxLevelReached);

        foreach(LevelRecord Record in AllLevelRecords)
        {
            Record.Save();
        }
    }

    public static void RecordCurrentGame()
    {
        int LevelId = Level.CurrentLevel.Id;
        MaxLevelReached = Mathf.Max(MaxLevelReached, LevelId);
        LevelRecord Record = AllLevelRecords.Find(T => T.Id == LevelId);
        if(Record == null)
        {
            Record = new LevelRecord(LevelId, Partie.Stars, Partie.Score);
            AllLevelRecords.Add(Record);
        }
        else
        {
            Record.Record(Partie.Stars, Partie.Score);
        }
        Record.Save();
        Save();
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
}
