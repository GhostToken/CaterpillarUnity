using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

namespace Assets.Caterpillar.UI.Script.MainMenu.LevelList
{
    public class Jour : MonoBehaviour
    {
        #region Properties

        public GameObject Blanks;
        public Image Etoile;
        public Sprite Zero_Etoile;
        public Sprite Une_Etoile;
        public Sprite Deux_Etoile;
        public Sprite Trois_Etoile;
        public TextMeshProUGUI Date;
        public bool IsWeekEnd = false;

        private int LevelOfMonth = -1;
        private Level Level = null;
        private Button Button = null;

        #endregion

        #region Static Sethods

        bool EstDebloqué(int LevelId)
        {
            return LevelId <= SaveGame.MaxLevelReached;
        }

        public Level GetLevel(int Day)
        {
            return Monde.CurrentMonde.AllLevels[Day];
        }

        public string GetDate(int Day)
        {
            return (IsWeekEnd ? Day.ToString() + "/" + (Day+1).ToString() : Day.ToString());
        }

        #endregion

        #region Unity Methods

        private void Start()
        {
            Button = GetComponent<Button>();
            Cache();
        }

        #endregion

        #region Methods

        public void Cache()
        {
            LevelOfMonth = -1;
            Level = null;

            Blanks.SetActive(true);
            Etoile.gameObject.SetActive(false);
            Date.gameObject.SetActive(false);
            Button.enabled = false;
        }

        public void Configure(int _LevelOfMonth, int Day)
        {
            LevelOfMonth = _LevelOfMonth;
            Level = GetLevel(_LevelOfMonth);

            Blanks.SetActive(false);
            Button.enabled = true;
            if (EstDebloqué(Level.Id) == true )
            {
                Etoile.gameObject.SetActive(true);
                Date.gameObject.SetActive(false);
                switch (SaveGame.GetStars(Level.Id))
                {
                    case 0:
                    {
                        Etoile.sprite = Zero_Etoile;
                        break;
                    }
                    case 1:
                    {
                        Etoile.sprite = Une_Etoile;
                        break;
                    }
                    case 2:
                    {
                        Etoile.sprite = Deux_Etoile;
                        break;
                    }
                    case 3:
                    {
                        Etoile.sprite = Trois_Etoile;
                        break;
                    }
                }
            }
            else
            {
                Etoile.gameObject.SetActive(false);
                Date.gameObject.SetActive(true);
                Date.text = GetDate(Day);
            }
        }

        public void Selectionne()
        {
            Menu.Instance.OpenLevelPopup(Level.Id);
        }

        #endregion
    }
}