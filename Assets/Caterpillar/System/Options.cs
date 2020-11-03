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

    private static string MoveMarkerIdentifier
    {
        get
        {
            return "MoveMarker";
        }
    }

    private static string UseTapToMoveIdentifier
    {
        get
        {
            return "UseTapToMove";
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

    static public bool MoveMarker
    {
        get
        {
            if (PlayerPrefs.HasKey(MoveMarkerIdentifier) == false)
            {
                return true;
            }
            return (PlayerPrefs.GetInt(MoveMarkerIdentifier) == 1);
        }
        set
        {
            PlayerPrefs.SetInt(MoveMarkerIdentifier, (value ? 1 : 0));
        }
    }

    static public bool UseTapToMove
    {
        get
        {
            if (PlayerPrefs.HasKey(UseTapToMoveIdentifier) == false)
            {
                return true;
            }
            return (PlayerPrefs.GetInt(UseTapToMoveIdentifier) == 1);
        }
        set
        {
            PlayerPrefs.SetInt(UseTapToMoveIdentifier, (value ? 1 : 0));
        }
    }

    #endregion
}