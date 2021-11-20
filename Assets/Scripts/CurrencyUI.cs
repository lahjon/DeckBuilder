using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class CurrencyUI : MonoBehaviour, IToolTipable, IEventSubscriber
{
    public CurrencyType currencyType;
    public Image image;
    public TMP_Text text;
    public Transform toolTipAnchor;
    public string tooltipDescription;

    void Start()
    {
        Subscribe();
        SetIcon();
        SetDescription();
        SetCurrency();
    }

    void OnDestroy()
    {
        Unsubscribe();
    }

    void SetIcon()
    {
        image.sprite = DatabaseSystem.instance.allCurrencyIcons.FirstOrDefault(x => x.name == currencyType.ToString());
    }

    void SetCurrency()
    {
        switch (currencyType)
        {
            case CurrencyType.Gold:
                text.text = WorldSystem.instance.characterManager.characterCurrency.gold.ToString();
                break;
            case CurrencyType.Ember:
                text.text = WorldSystem.instance.characterManager.characterCurrency.ember.ToString();
                break;
            case CurrencyType.FullEmber:
                text.text = WorldSystem.instance.characterManager.characterCurrency.fullEmber.ToString();
                break;
            case CurrencyType.ArmorShard:
                text.text = WorldSystem.instance.characterManager.characterCurrency.armorShard.ToString();
                break;
            case CurrencyType.Shard:
                text.text = WorldSystem.instance.characterManager.characterCurrency.shard.ToString();
                break;
            default:
                break;
        }
    }
    void SetDescription()
    {
        switch (currencyType)
        {
            case CurrencyType.Gold:
                tooltipDescription = "Gold is used to buy stuff in the overworld!";
                break;
            case CurrencyType.Ember:
                tooltipDescription = "Embers are used to upgrade you card temporarily!";
                break;
            case CurrencyType.FullEmber:
                tooltipDescription = "Full Ember is used to permanently upgrade your cards!";
                break;
            case CurrencyType.ArmorShard:
                tooltipDescription = "Armor shards are used to upgrade your armor";
                break;
            case CurrencyType.Shard:
                tooltipDescription = "Shards are used to buy stuff in town!";
                break;
            default:
                break;
        }
    }

    void UpdateCurrency(CurrencyType aCurrencyType, int currentAmount)
    {
        if (aCurrencyType == currencyType)
            text.text = currentAmount.ToString();
    }

    public (List<string> tips, Vector3 worldPosition) GetTipInfo()
    {
        Vector3 pos = WorldSystem.instance.cameraManager.currentCamera.WorldToScreenPoint(toolTipAnchor.position);
        return (new List<string>() {tooltipDescription}, pos);
    }

    public void Subscribe()
    {
        EventManager.OnCurrencyChanged += UpdateCurrency;
    }

    public void Unsubscribe()
    {
        EventManager.OnCurrencyChanged -= UpdateCurrency;
    }
}
