using UnityEngine;
using System.Collections;

static public class Options
{
    #region Identifiers

    private static string MeshOccludingIdentifier
    {
        get
        {
            return "MeshOccluding";
        }
    }

    private static string MenuAffichéIdentifier
    {
        get
        {
            return "MenuAffiché";
        }
    }

    #endregion

    #region Properties

    static public bool MeshOccluding
    {
        get
        {
            if(PlayerPrefs.HasKey(MeshOccludingIdentifier) == false )
            {
                return false;
            }
            return (PlayerPrefs.GetInt(MeshOccludingIdentifier) == 1);
        }
        set
        {
            PlayerPrefs.SetInt(MeshOccludingIdentifier, (value ? 1 : 0));
        }
    }

    static public bool MenuAffiché
    {
        get
        {
            if (PlayerPrefs.HasKey(MenuAffichéIdentifier) == false)
            {
                return true;
            }
            return (PlayerPrefs.GetInt(MenuAffichéIdentifier) == 1);
        }
        set
        {
            PlayerPrefs.SetInt(MenuAffichéIdentifier, (value ? 1 : 0));
        }
    }

    #endregion
}