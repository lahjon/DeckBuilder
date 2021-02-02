using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class CardData : ScriptableObject
{
    public new string name;
    public string description;

    public Sprite artwork;
    public int cost;
    public int goldValue;

    public CardEffect Damage;
    public CardEffect Block;

    public List<CardEffect> SelfEffects;
    public List<CardEffect> Effects;

    public CardRarity cardRarity;

    public CharacterClass characterClass;
    public GameObject animationPrefab;

    [HideInInspector]
    public CardTargetType OverallTargetType
    {
        get
        {
            if (Effects.Count == 0 && Effects.Count(x => x.Target == CardTargetType.Single) == 0 && Damage.Target != CardTargetType.Single)
                return CardTargetType.ALL;
            else
                return CardTargetType.Single;
        }
    }

    

}
