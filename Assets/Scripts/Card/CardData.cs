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
    public int cost;
    public bool visibleCost = true;

    public List<CardSingleFieldPropertyType> singleFieldProperties = new List<CardSingleFieldPropertyType>();

    public CardEffectCarrierData Damage;
    public CardEffectCarrierData Block;

    public List<CardEffectCarrierData>  effectsOnPlay = new List<CardEffectCarrierData>();
    public List<CardActivitySetting>    activitiesOnPlay = new List<CardActivitySetting>();

    public List<CardEffectCarrierData>  effectsOnDraw = new List<CardEffectCarrierData>();
    public List<CardActivitySetting>    activitiesOnDraw = new List<CardActivitySetting>();

    public Rarity rarity;

    public CardClassType cardClass;
    public GameObject animationPrefab;
    public AudioClip audio;

    
    public int goldValue;
}
