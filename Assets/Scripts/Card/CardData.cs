using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class CardData : ScriptableObject
{
    public string cardName;
    public string description;

    public Sprite artwork;
    public int cost;
    public int goldValue;

    public CardEffect Damage;
    public CardEffect Block;

    public List<CardEffect> SelfEffects = new List<CardEffect>();
    public List<CardEffect> Effects = new List<CardEffect>();

    public List<CardActivitySetting> activities = new List<CardActivitySetting>();

    public CardRarity cardRarity;

    public CharacterClassType characterClass;
    public GameObject animationPrefab;
    public AudioClip audio;

    [HideInInspector]
    public bool targetRequired
    {
        get
        {
            if (Effects.Count(x => x.Target == CardTargetType.Single) == 0 && (Damage.Value == 0 || Damage.Target != CardTargetType.Single))
                return false;
            else
                return true;
        }
    }

    public List<CardEffect> allEffects
    {
        get
        {
            List<CardEffect> tempList = new List<CardEffect>();
            tempList.Add(Damage);
            tempList.Add(Block);
            tempList.AddRange(SelfEffects);
            tempList.AddRange(Effects);
            return tempList;
        }
    }
    

}
