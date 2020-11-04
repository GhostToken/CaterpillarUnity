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

    private static string UseOrbitalCameraIdentifier
    {
        get
        {
            return "UseOrbitalCamera";
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
            PlayerPrefsHelpers.SetBool(MeshOccludingIdentifier, value);
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
            PlayerPrefsHelpers.SetBool(MenuAffichéIdentifier, value);
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
            PlayerPrefsHelpers.SetBool(MoveMarkerIdentifier, value);
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
            PlayerPrefsHelpers.SetBool(UseTapToMoveIdentifier, value);
        }
    }

    static public bool UseOrbitalCamera
    {
        get
        {
            if (PlayerPrefs.HasKey(UseOrbitalCameraIdentifier) == false)
            {
                return false;
            }
            return (PlayerPrefs.GetInt(UseOrbitalCameraIdentifier) == 1);
        }
        set
        {
            PlayerPrefsHelpers.SetBool(UseOrbitalCameraIdentifier, value);
        }
    }

    #endregion
}