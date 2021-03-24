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
    public int goldValue;

    public bool exhaust = false;

    public CardEffect Damage;
    public CardEffect Block;

    public List<CardEffect> inEffects = new List<CardEffect>();

    public List<CardActivitySetting> inActivities = new List<CardActivitySetting>();

    public Rarity cardRarity;

    public CharacterClassType characterClass;
    public GameObject animationPrefab;
    public AudioClip audio;


}
