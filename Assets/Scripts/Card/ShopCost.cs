using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopCost : MonoBehaviour
{
    public TMP_Text costText;
    public Image costImage; 
    public static string upgradedText = "Fully Upgraded";
    public static string outOfStock = "Out of Stock";

    public void SetCost(CurrencyType currencyType, CostType costType, int amount)
    {
        string extension;
        switch (costType)
        {
            case CostType.FullyUpgraded:
                extension = upgradedText;
                break;
            case CostType.OutOfStock:
                extension = outOfStock;
                break;
            default:
                extension = "";
                break;
        }
        switch (currencyType)
        {
            case CurrencyType.Gold:
                costText.text = amount > 0 ? string.Format("{0} gold", amount) : extension;
                break;
            case CurrencyType.Shard:
                costText.text = amount > 0 ? string.Format("{0} shards", amount) : extension;
                break;
            case CurrencyType.FullEmber:
                costText.text = amount > 0 ? string.Format("{0} full ember", amount) : extension;
                break;
            default:
                break;
        }
    }
}