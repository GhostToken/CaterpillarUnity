using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CurrencyWidget : MonoBehaviour
{
    public ECurrency Currency;

    public TextMeshProUGUI CountText;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return null;
        yield return null;
        UpdateCurrency();

        Inventaire.OnInventoryUpdate += UpdateCurrency;
    }

    private void OnDestroy()
    {
        Inventaire.OnInventoryUpdate -= UpdateCurrency;
    }

    private void UpdateCurrency()
    {
        CountText.text = Inventaire.GetCurrency(Currency).ToString();
    }
}
