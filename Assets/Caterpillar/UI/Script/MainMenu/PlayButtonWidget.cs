using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Assets.Caterpillar.UI.Script.MainMenu
{
    public class PlayButtonWidget : MonoBehaviour
    {
        Button Button;

        // Start is called before the first frame update
        IEnumerator Start()
        {
            Button = GetComponent<Button>();

            yield return null;
            yield return null;
            UpdateStatus();
            Inventaire.OnInventoryUpdate += UpdateStatus;
        }

        private void OnDestroy()
        {
            Inventaire.OnInventoryUpdate -= UpdateStatus;
        }

        private void UpdateStatus()
        {
            Button.interactable = Inventaire.CanStartGame();
        }
    }
}