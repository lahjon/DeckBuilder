using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using TMPro;

public class Card : MonoBehaviour
{
    public Rarity rarity;
    public string cardName;
    public Sprite artwork;

    public int cost;
    public CardData cardData;

    public bool immediate;
    public bool exhaust;

    public CardEffectInfo Damage;
    public CardEffectInfo Block;

    public List<CardEffectInfo> effectsOnPlay = new List<CardEffectInfo>();
    public List<CardEffectInfo> effectsOnDraw = new List<CardEffectInfo>();
    public List<CardActivitySetting> activitiesOnPlay = new List<CardActivitySetting>();
    public List<CardActivitySetting> activitiesOnDraw = new List<CardActivitySetting>();

    public GameObject animationPrefab;
    public CombatActor owner;
    public Material material;

    public CardClassType classType = CardClassType.None;

    public bool visibleCost = true;
    public bool unplayable;
    public bool unstable;

    public void BindCardData()
    {
        name            = cardData.cardName;
        rarity          = cardData.rarity;
        cardName        = cardData.cardName;
        artwork         = cardData.artwork;
        cost            = cardData.cost;
        immediate       = cardData.immediate;
        exhaust         = cardData.exhaust;
        Damage          = cardData.Damage;
        Block           = cardData.Block;
        effectsOnPlay   = cardData.effectsOnPlay;
        effectsOnDraw   = cardData.effectsOnDraw;
        activitiesOnPlay= cardData.activitiesOnPlay;
        activitiesOnDraw= cardData.activitiesOnDraw;
        animationPrefab = cardData.animationPrefab;
        classType       = cardData.cardClass;
        visibleCost     = cardData.visibleCost;
        unplayable      = cardData.unplayable;
        unstable        = cardData.unstable;
    }

    public void Mimic(Card card)
    {
        name = card.cardName;
        rarity = card.rarity;
        cardName = card.cardName;
        artwork = card.artwork;
        cost = card.cost;
        immediate = card.immediate;
        exhaust = card.exhaust;
        Damage = card.Damage;
        Block = card.Block;
        effectsOnPlay = card.effectsOnPlay;
        effectsOnDraw = card.effectsOnDraw;
        activitiesOnPlay = card.activitiesOnPlay;
        activitiesOnDraw = card.activitiesOnDraw;
        animationPrefab = card.animationPrefab;
        classType = card.classType;
        visibleCost = card.visibleCost;
        unplayable = card.unplayable;
        unstable = card.unstable;
    }

    [HideInInspector]
    public bool targetRequired
    {
        get
        {
            if (effectsOnPlay.Count(x => x.Target == CardTargetType.EnemySingle) == 0 && (Damage.Value == 0 || Damage.Target != CardTargetType.EnemySingle))
                return false;
            else
                return true;
        }
    }

    void Start()
    {
        //AssignMaterials();
    } 

    void AssignMaterials()
    {
        // material = Instantiate(material);

        // for (int i = 0; i < transform.childCount; i++)
        // {
            
        // }
    }

    public List<CardEffectInfo> allEffects
    {
        get
        {
            List<CardEffectInfo> tempList = new List<CardEffectInfo>();
            tempList.Add(Damage);
            tempList.Add(Block);
            tempList.AddRange(effectsOnPlay);
            return tempList;
        }
    }

    public HashSet<CardTargetType> allTargetTypes
    {
        get
        {
            HashSet<CardTargetType> tempSet = new HashSet<CardTargetType>();
            if (Damage.Value != 0) tempSet.Add(Damage.Target);
            if (Block.Value != 0) tempSet.Add(Block.Target);
            effectsOnPlay.ForEach(x => tempSet.Add(x.Target));
            return tempSet;
        }
    }

    public List<CardEffectInfo> GetEffectsByType(EffectType type)
    {
        return effectsOnPlay.Where(x => x.Type == type).ToList();
    }

    public CardActivitySetting GetactivityByType(CardActivityType type)
    {
        return activitiesOnPlay.Where(x => x.type == type).FirstOrDefault();
    }

    public static void SpliceCards(Card Target, Card a, Card b)
    {
        HashSet<EffectType> effectTypes = new HashSet<EffectType>();
        HashSet<CardActivityType> activityTypes = new HashSet<CardActivityType>();

        Target.classType = a.classType;
        Target.rarity = a.rarity;

        Target.artwork = a.artwork;
        Target.animationPrefab = a.animationPrefab;
        Target.cost = Mathf.Max(a.cost, b.cost);

        Target.cardName = a.cardName + (a.cardName.Contains("Mod+") ? "+" : " Mod+");

        Target.Damage = a.Damage + b.Damage;
        Target.Block = a.Block + b.Block;

        a.effectsOnPlay.ForEach(e => effectTypes.Add(e.Type));
        b.effectsOnPlay.ForEach(e => effectTypes.Add(e.Type));

        foreach(EffectType type in effectTypes)
        {
            List<CardEffectInfo> aE = a.GetEffectsByType(type);
            List<CardEffectInfo> bE = b.GetEffectsByType(type);

            if (aE.Count == 0)
                Target.effectsOnPlay.AddRange(bE);
            else if(bE.Count == 0)
                Target.effectsOnPlay.AddRange(aE);
            else
            {
                Target.effectsOnPlay.Add(aE[0] + bE[0]);
            }
        }

        a.activitiesOnPlay.ForEach(x => activityTypes.Add(x.type));
        b.activitiesOnPlay.ForEach(x => activityTypes.Add(x.type));
        List<CardActivityType> activityTypesList = activityTypes.OrderBy(x => x).ToList();

        foreach(CardActivityType type in activityTypesList)
        {
            if(type != CardActivityType.Splice)
            {
                Target.activitiesOnPlay.AddRange(a.activitiesOnPlay.Where(x => x.type == type));
                Target.activitiesOnPlay.AddRange(b.activitiesOnPlay.Where(x => x.type == type));
            }
            else
            {
                int aParam = Int32.Parse(a.GetactivityByType(CardActivityType.Splice).parameter);
                if(aParam > 1)
                    Target.activitiesOnPlay.Add(new CardActivitySetting() { type = CardActivityType.Splice, parameter = (aParam-1).ToString()});
            }
        }
    }


}

