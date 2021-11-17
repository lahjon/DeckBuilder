using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "New Card", menuName = "CardGame/Card")]
public class CardData : CardFunctionalityData
{
    public string cardName;

    public CardType cardType;

    public Sprite artwork;

    [SerializeField]
    public List<EnergyData> costDatas = new List<EnergyData>();
    public List<EnergyData> costOptionalDatas = new List<EnergyData>();
    public bool visibleCost = true;
    
    public string upgradeCostFullEmber;

    public int maxUpgrades;
    public List<CardFunctionalityData> upgrades = new List<CardFunctionalityData>();

    public Rarity rarity;

    public CardClassType cardClass;
    public GameObject animationPrefab;
    public AudioClip audio;

    
    public int goldValue;
}

[System.Serializable]
public class EnergyData
{
    public EnergyType type;
    public CardIntData data;
}
