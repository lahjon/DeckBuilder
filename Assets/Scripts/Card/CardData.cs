using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "New Card", menuName = "CardGame/Card")]
public class CardData : ScriptableObject
{
    public string cardName;

    public Sprite artwork;
    public int cost;
    public bool visibleCost = true;

    public bool exhaust = false;

    public CardEffectInfo Damage;
    public CardEffectInfo Block;

    public List<CardEffectInfo>         effectsOnPlay = new List<CardEffectInfo>();
    public List<CardActivitySetting>    activitiesOnPlay = new List<CardActivitySetting>();

    public List<CardEffectInfo>         effectsOnDraw = new List<CardEffectInfo>();
    public List<CardActivitySetting>    activitiesOnDraw = new List<CardActivitySetting>();

    public Rarity rarity;

    public CardClassType cardClass;
    public GameObject animationPrefab;
    public AudioClip audio;

    
    public int goldValue;

    public bool unplayable = false;
    public bool unstable = false;
}
