using HedgehogTeam.EasyTouch;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EEcran
{
    Principal,
    Inventaire,
    Carte
}

[Serializable]
public struct ConfigEcran
{
    public EEcran Type;
    public float ToLeftZ;
    public float ToRightZ;
    public bool ShowHeader;
    public bool ShowFooter;
    public GameObject UIScreen;
}

public class Menu : MonoBehaviour
{
    #region Properties

    public List<ConfigEcran> Config;
    public Camera Camera;
    public GameObject Header;
    public GameObject Footer;
    public float DureeTransition = 0.35f;

    public LevelPopup LevelPopup;
    public AfterLevelPopup AfterLevelPopup;


    EEcran Current = EEcran.Principal;
    bool MoveInProgress = false;

    static private Menu MenuInstance;

    #endregion

    #region Accessors

    public ConfigEcran GetConfig(EEcran Ecran)
    {
        return Config.Find(T => T.Type == Ecran);
    }

    public bool HasPopupOpen()
    {
        return LevelPopup.gameObject.activeInHierarchy;
    }

    static public Menu Instance
    {
        get
        {
            return MenuInstance;
        }
    }

    #endregion

    #region Unity Methods

    private void Start()
    {
        MenuInstance = this;
        FinishTransitionTo(EEcran.Principal, true);

        if (Partie.JustTerminated == true)
        {
            Partie.JustTerminated = false;
            OpenAfterLevelPopup();
        }
    }

    private void OnEnable()
    {
        EasyTouch.On_SwipeEnd += OnSwipeEnd;
    }

    private void OnDisable()
    {
        EasyTouch.On_SwipeEnd -= OnSwipeEnd;
    }

    #endregion

    #region Handlers

    private void OnSwipeEnd(Gesture Gesture)
    {
        switch(Gesture.swipe)
        {
            case EasyTouch.SwipeDirection.Left:
                {
                    SwipeLeft();
                    break;
                }
            case EasyTouch.SwipeDirection.Right:
                {
                    SwipeRight();
                    break;
                }
            default:
                {
                    break;
                }
        }
    }

    private void SwipeLeft()
    {
        if(HasPopupOpen())
        {
            return;
        }

        switch (Current)
        {
            case EEcran.Carte:
                {
                    StartTransitionTo(EEcran.Principal, false);
                    break;
                }
            case EEcran.Principal:
                {
                    StartTransitionTo(EEcran.Inventaire, false);
                    break;
                }
            case EEcran.Inventaire:
                {
                    StartTransitionTo(EEcran.Carte, false);
                    break;
                }
        }
    }

    private void SwipeRight()
    {
        if (HasPopupOpen())
        {
            return;
        }

        switch (Current)
        {
            case EEcran.Carte:
                {
                    StartTransitionTo(EEcran.Inventaire, true);
                    break;
                }
            case EEcran.Inventaire:
                {
                    StartTransitionTo(EEcran.Principal, true);
                    break;
                }
            case EEcran.Principal:
                {
                    StartTransitionTo(EEcran.Carte, true);
                    break;
                }
        }
    }

    #endregion

    #region Method

    private void StartTransitionTo(EEcran NextScreen, bool ToLeft)
    {
        if(MoveInProgress == true)
        {
            return;
        }

        MoveInProgress = true;
        ConfigEcran Config = GetConfig(NextScreen);

        CloseAllPopups();

        if (!Config.ShowFooter)
        {
            Footer.SetActive(false);
        }

        if (!Config.ShowHeader)
        {
            Header.SetActive(false);
        }
        
        Config = GetConfig(Current);
        if (Config.UIScreen != null)
        {
            Config.UIScreen.SetActive(false);
        }

        StartCoroutine(MoveTo(NextScreen, ToLeft));
    }

    private IEnumerator MoveTo(EEcran NextScreen, bool ToLeft)
    {
        ConfigEcran Config = GetConfig(Current);
        Vector3 StartPosition = Camera.transform.position;
        if (Config.ToLeftZ != Config.ToRightZ)
        {
            StartPosition.z = (ToLeft ? Config.ToRightZ : Config.ToLeftZ);
        }

        Config = GetConfig(NextScreen);
        Vector3 EndPosition = StartPosition;
        EndPosition.z = (ToLeft ? Config.ToLeftZ : Config.ToRightZ);

        float StartTime = Time.time;
        while ( Time.time < StartTime + DureeTransition)
        {
            float Progress = (Time.time - StartTime) / DureeTransition;
            Camera.transform.position = Vector3.Lerp(StartPosition, EndPosition, Progress);

            yield return null;
        }

        FinishTransitionTo(NextScreen, ToLeft);
    }

    private void FinishTransitionTo(EEcran NextScreen, bool ToLeft)
    {
        Current = NextScreen;
        ConfigEcran Config = GetConfig(Current);

        Vector3 EndPosition = Camera.transform.position;
        EndPosition.z = (ToLeft ? Config.ToLeftZ : Config.ToRightZ);
        Camera.transform.position = EndPosition;

        if (Config.ShowFooter)
        {
            Footer.SetActive(true);
        }

        if (Config.ShowHeader)
        {
            Header.SetActive(true);
        }

        if (Config.UIScreen != null)
        {
            Config.UIScreen.SetActive(true);
        }

        MoveInProgress = false;
    }

    #endregion

    #region Events

    public void CloseAllPopups()
    {
        LevelPopup.gameObject.SetActive(false);
        AfterLevelPopup.gameObject.SetActive(false);
    }

    public void OpenNextLevel()
    {
        OpenLevelPopup(SaveGame.MaxLevelReached);
    }

    public void OpenLevelPopup(int LevelId)
    {
        LevelPopup.gameObject.SetActive(true);
        LevelPopup.Open(LevelId);
    }

    public void OpenAfterLevelPopup()
    {
        AfterLevelPopup.gameObject.SetActive(true);
        AfterLevelPopup.Open(Level.CurrentLevel.Id);
    }

    #endregion

}
