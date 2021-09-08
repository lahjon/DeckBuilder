using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "New Card", menuName = "CardGame/Card")]
public class CardData : ScriptableObject
{
    public string cardName;

    public CardType cardType;

    public Sprite artwork;
    public string cost;
    public bool visibleCost = true;

    public List<CardSingleFieldPropertyType> singleFieldProperties = new List<CardSingleFieldPropertyType>();

    public List<CardEffectCarrierData>  effects = new List<CardEffectCarrierData>();
    public List<CardActivitySetting>    activities = new List<CardActivitySetting>();

    public Rarity rarity;

    public CardClassType cardClass;
    public GameObject animationPrefab;
    public AudioClip audio;

    
    public int goldValue;
}
