using MK.Toon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial_Slide : MonoBehaviour
{
    #region Properties

    public List<Texture> Slides;
    public Button BoutonPasser;
    private RawImage SlideShow;
    private int Index = 0;

    #endregion

    #region Unity Methods

    private void OnEnable()
    {
        SlideShow = GetComponent<RawImage>();
        Reset();
    }

    private void OnDisable()
    {
        SaveGame.TutorialVu = true;
    }

    private void Reset()
    {
        Index = 0;
        SlideShow.texture = Slides[Index];
        ManageBoutonPasser();
    }

    #endregion

    #region Public Methods

    public void Passer()
    {
        if(Index == 0)
        {
            Menu.Instance.CloseAllPopups();
        }
    }

    public void Next()
    {
        if( Index < Slides.Count)
        {
            Index++;
            SlideShow.texture = Slides[Index];
        }
        else
        {
            Menu.Instance.CloseAllPopups();
        }
        ManageBoutonPasser();
    }

    public void Last()
    {
        if(Index > 0)
        {
            Index--;
            SlideShow.texture = Slides[Index];
        }
        ManageBoutonPasser();
    }

    #endregion

    #region Private Methods

    private void ManageBoutonPasser()
    {
        BoutonPasser.gameObject.SetActive(Index == 0);
    }

    #endregion
}
