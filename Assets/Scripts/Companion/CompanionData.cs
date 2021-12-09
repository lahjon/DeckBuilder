using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "NewCompanion", menuName = "CardGame/Companion")]
public class CompanionData : ScriptableObject
{
    public string companionId;
    public string companionName;
    public Sprite artwork;
    public Rarity rarity;
    public GameObject characterArt;
    public List<CardData> deck = new List<CardData>();

    public List<StatusEffectCarrier> startingEffects = new List<StatusEffectCarrier>();
    public List<CombatActivity> startingActivities = new List<CombatActivity>();

    public bool shuffleInit = true;
    public bool stochasticReshuffle = true;
}
