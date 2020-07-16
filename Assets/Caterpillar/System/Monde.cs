using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Monde_", menuName = "Caterpillar/Monde")]
public class Monde : ScriptableObject
{
    #region Properties

    public string Nom;
    public string Description;

    public List<Level> AllLevels;

    #endregion

    #region Static Accessor

    private static List<Monde> AllMondes;

    public static Monde CurrentMonde;

    public static List<Monde> GetAllMondes()
    {
        if(AllMondes == null)
        {
            AllMondes = new List<Monde>(Resources.LoadAll<Monde>("Mondes"));
        }

        return AllMondes;
    }

    public static Monde GetLevel(string Monde)
    {
        return GetAllMondes().Find(T => T.Nom == Monde);
    }

    #endregion
}
