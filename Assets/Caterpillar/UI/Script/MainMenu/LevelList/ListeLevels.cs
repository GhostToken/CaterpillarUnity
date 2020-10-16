﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public enum EDay
{
    Lundi = 0,
    Mardi,
    Mercredi,
    Jeudi,
    Venredi,
    Samedi_Dimanche
}

public class ListeLevels : MonoBehaviour
{
    #region Properties

    public TextMeshProUGUI Mois;
    public Image Baniere;
    public TextMeshProUGUI Description;

    public Semaine[] Semaines;

    #endregion

    #region Unity Methods

    private void OnEnable()
    {
        // TO DO : Determine last reached world
        Open(0);
    }

    #endregion

    #region Methods

    public void Open(int MondeId)
    {
        Monde.CurrentMonde = Monde.GetMonde(MondeId);

        Mois.text = Monde.CurrentMonde.Nom;
        Baniere.sprite = Monde.CurrentMonde.Baniere;
        Description.text = Monde.CurrentMonde.Description;

        int NumSemaine = (Monde.CurrentMonde.AllLevels.Count / 6) + 1;
        int Offset = Monde.CurrentMonde.AllLevels.Count % 6;
        EDay PremierJour = (EDay)(6 - Offset);
        int date = 0;
        int levelOfMonth = 0;
        ConfigurePremiereSemaine(PremierJour, ref levelOfMonth, ref date);
        if (NumSemaine > 1)
        {
            for (int semaine = 1; semaine <= NumSemaine; ++semaine)
            {
                ConfigureSemaine(semaine, ref levelOfMonth, ref date);
            }
        }
    }

    void ConfigurePremiereSemaine(EDay PremierJour, ref int LevelOfMonth, ref int Date)
    {
        for (EDay day = EDay.Lundi; day < PremierJour; ++day)
        {
            Semaines[0].CacheJour(day);
        }
        for (EDay day = PremierJour; day <= EDay.Samedi_Dimanche; ++ day)
        {
            if (LevelOfMonth >= Monde.CurrentMonde.AllLevels.Count)
            {
                Semaines[0].CacheJour(day);
            }
            else
            {
                Semaines[0].ConfigueJour(day, LevelOfMonth++, Date++);
            }
        }
        Date++; // dimanche
    }

    void ConfigureSemaine(int semaine, ref int LevelOfMonth, ref int Date)
    {
        for (EDay day = EDay.Lundi; day <= EDay.Samedi_Dimanche; ++day)
        {
            if(LevelOfMonth >= Monde.CurrentMonde.AllLevels.Count )
            {
                Semaines[semaine].CacheJour(day);
            }
            else
            {
                Semaines[semaine].ConfigueJour(day, LevelOfMonth, Date++);
            }
            LevelOfMonth++;
        }
        Date++; // dimanche
    }

    public void Close()
    {
        Menu.Instance.CloseAllPopups();
    }

    #endregion
}
