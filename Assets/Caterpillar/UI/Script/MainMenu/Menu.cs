using HedgehogTeam.EasyTouch;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum EEcran
{
    Principal,
    Inventaire,
    Moi,
    Carte,
    Porte
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
    public Tutorial_Slide Tutoriel;


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
        return LevelPopup.gameObject.activeInHierarchy | AfterLevelPopup.gameObject.activeInHierarchy;
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

        if( SaveGame.TutorialVu == false)
        {
            OpenTutoriel();
        }
    }

    private void Update()
    {
        // Make sure user is on Android platform
        if (Application.platform == RuntimePlatform.Android)
        {
            // Check if Back was pressed this frame
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if(HasPopupOpen() == true )
                {
                    Menu.Instance.CloseAllPopups();
                }
                else
                {
                    TransitionToStart();
                }
            }
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
            case EEcran.Principal:
                {
                    StartTransitionTo(EEcran.Moi, false);
                    break;
                }
            case EEcran.Moi:
                {
                    StartTransitionTo(EEcran.Carte, false);
                    break;
                }
            case EEcran.Carte:
                {
                    StartTransitionTo(EEcran.Inventaire, false);
                    break;
                }
            case EEcran.Inventaire:
                {
                    StartTransitionTo(EEcran.Porte, false);
                    break;
                }
            case EEcran.Porte:
                {
                    StartTransitionTo(EEcran.Principal, false);
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
            case EEcran.Principal:
                {
                    StartTransitionTo(EEcran.Porte, true);
                    break;
                }
            case EEcran.Porte:
                {
                    StartTransitionTo(EEcran.Inventaire, true);
                    break;
                }
            case EEcran.Inventaire:
                {
                    StartTransitionTo(EEcran.Carte, true);
                    break;
                }
            case EEcran.Carte:
                {
                    StartTransitionTo(EEcran.Moi, true);
                    break;
                }
            case EEcran.Moi:
                {
                    StartTransitionTo(EEcran.Principal, true);
                    break;
                }
        }
    }

    #endregion

    #region Method

    private void TransitionToStart()
    {
        switch (Current)
        {
            case EEcran.Principal:
                {
                    break;
                }
            case EEcran.Moi:
                {
                    StartTransitionTo(EEcran.Principal, true);
                    break;
                }
            case EEcran.Carte:
                {
                    StartTransitionTo(EEcran.Principal, true);
                    break;
                }
            case EEcran.Inventaire:
                {
                    StartTransitionTo(EEcran.Principal, false);
                    break;
                }
            case EEcran.Porte:
                {
                    StartTransitionTo(EEcran.Principal, false);
                    break;
                }
        }
    }

    private void StartTransitionTo(EEcran NextScreen, bool ToLeft)
    {
        if(MoveInProgress == true)
        {
            return;
        }

        MoveInProgress = true;
        ConfigEcran Config = GetConfig(NextScreen);

        CloseAllPopups();

        if(Footer != null)
        {
            if (!Config.ShowFooter)
            {
                Footer.SetActive(false);
            }
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

        if (Footer != null)
        {
            if (Config.ShowFooter)
            {
                Footer.SetActive(true);
            }
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

    static public void Open()
    {
        ScreenFader.Launch_FadeIn(() =>
        {
            SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Single);
        });
    }

    public bool CloseAllPopups()
    {
        Tutoriel.gameObject.SetActive(false);
        if (LevelPopup.gameObject.activeSelf)
        {
            LevelPopup.gameObject.SetActive(false);
            return true;
        }
        if (AfterLevelPopup.gameObject.activeSelf)
        {
            AfterLevelPopup.gameObject.SetActive(false);
            return true;
        }
        return false;
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

    public void OpenTutoriel()
    {
        CloseAllPopups();
        Tutoriel.gameObject.SetActive(true);
    }

    #endregion

}
