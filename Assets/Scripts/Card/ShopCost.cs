using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopCost : MonoBehaviour
{
    public TMP_Text costText;
    public Image costImage; 

    public void SetCost(CurrencyType currencyType, int amount)
    {
        // if (amount == 0) gameObject.SetActive(false);
        // else gameObject.SetActive(true);
        switch (currencyType)
        {
            case CurrencyType.Gold:
                costText.text = amount > 0 ? string.Format("{0} gold", amount) : "Fully Upgraded";
                break;
            case CurrencyType.Shard:
                costText.text = amount > 0 ? string.Format("{0} shards", amount) : "Fully Upgraded";
                break;
            default:
                break;
        }
    }
}
