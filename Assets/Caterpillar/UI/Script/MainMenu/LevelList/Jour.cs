using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

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
        private Button m_Button = null;

        public float AnimationDuration = 0.15f;

        private Button Button
        {
            get
            {
                if(m_Button == null)
                {
                    m_Button = GetComponent<Button>();
                }
                return m_Button;
            }
        }

        #endregion

        #region Static Sethods

        bool EstDebloqué(int LevelId)
        {
            return true;
            return LevelId <= SaveGame.MaxLevelReached;
        }

        public Level GetLevel(int _LevelOfMonth)
        {
            return Monde.CurrentMonde.AllLevels[_LevelOfMonth];
        }

        public string GetDate(int Day)
        {
            return (IsWeekEnd ? Day.ToString() + "/" + (Day+1).ToString() : Day.ToString());
        }

        #endregion

        #region Unity Methods

        private void Start()
        {
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
                Etoile.transform.localScale = Vector3.zero;
                Sequence Sequence = DOTween.Sequence();
                Sequence.Append(Etoile.transform.DOScale(1.0f, AnimationDuration));
                Sequence.Append(Etoile.transform.DOPunchScale(Vector3.one * 0.25f, AnimationDuration / 2.0f));
                Sequence.PrependInterval(AnimationDuration * _LevelOfMonth);
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