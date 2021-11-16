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
    public CardIntData cost;
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
