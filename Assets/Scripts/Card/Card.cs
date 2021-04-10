using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Card : MonoBehaviour
{
    public string cardName;
    public Sprite artwork;

    public int cost;
    public CardData cardData;

    public bool exhaust;

    public CardEffect Damage;
    public CardEffect Block;

    public List<CardEffect> Effects = new List<CardEffect>();
    public List<CardActivitySetting> activities = new List<CardActivitySetting>();

    public GameObject animationPrefab;
    public CombatActor owner;

    public void BindCardData()
    {
        name            = cardData.name;
        cardName        = cardData.cardName;
        artwork         = cardData.artwork;
        cost            = cardData.cost;
        exhaust         = cardData.exhaust;
        Damage          = cardData.Damage;
        Block           = cardData.Block;
        Effects         = cardData.inEffects;
        activities      = cardData.inActivities;
        animationPrefab = cardData.animationPrefab;
    }


    [HideInInspector]
    public bool targetRequired
    {
            get
        {
                if (Effects.Count(x => x.Target == CardTargetType.EnemySingle) == 0 && (Damage.Value == 0 || Damage.Target != CardTargetType.EnemySingle))
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
            tempList.AddRange(Effects);
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
            Effects.ForEach(x => tempSet.Add(x.Target));
            return tempSet;
        }
    }

    public List<CardEffect> GetEffectsByType(EffectType type)
    {
        return Effects.Where(x => x.Type == type).ToList();
    }

    public CardActivitySetting GetactivityByType(CardActivityType type)
    {
        return activities.Where(x => x.type == type).FirstOrDefault();
    }

    public static void SpliceCards(Card Target, Card a, Card b)
    {
        HashSet<EffectType> effectTypes = new HashSet<EffectType>();
        HashSet<CardActivityType> activityTypes = new HashSet<CardActivityType>();

        Target.artwork = a.artwork;
        Target.animationPrefab = a.animationPrefab;
        Target.cost = Mathf.Max(a.cost, b.cost);
        for (int i = 0; i < Mathf.Max(a.cardName.Length, b.cardName.Length); i++)
        {
            if (i == a.cardName.Length)
            {
                Target.cardName += b.cardName.Substring(i);
                break;
            }
            else if (i == b.cardName.Length)
            {
                Target.cardName += a.cardName.Substring(i);
                break;
            }

            Target.cardName += i % 2 == 0 ? a.cardName[i] : b.cardName[i];
        }

        Target.Damage = a.Damage + b.Damage;
        Target.Block = a.Block + b.Block;

        a.Effects.ForEach(e => effectTypes.Add(e.Type));
        b.Effects.ForEach(e => effectTypes.Add(e.Type));

        foreach(EffectType type in effectTypes)
        {
            List<CardEffect> aE = a.GetEffectsByType(type);
            List<CardEffect> bE = b.GetEffectsByType(type);

            if (aE.Count == 0)
                Target.Effects.AddRange(bE);
            else if(bE.Count == 0)
                Target.Effects.AddRange(aE);
            else
            {
                HashSet<CardTargetType> targetTypes = new HashSet<CardTargetType>();
                bE.ForEach(x => targetTypes.Add(x.Target));
                aE.ForEach(x => targetTypes.Add(x.Target));

                foreach(CardTargetType targetType in targetTypes)
                    Target.Effects.Add(bE.Where(x => x.Target == targetType).FirstOrDefault() + aE.Where(x => x.Target == targetType).FirstOrDefault());
            }
        }

        a.activities.ForEach(x => activityTypes.Add(x.type));
        b.activities.ForEach(x => activityTypes.Add(x.type));
        List<CardActivityType> activityTypesList = activityTypes.OrderBy(x => x).ToList();

        foreach(CardActivityType type in activityTypesList)
        {
            if(type != CardActivityType.Splice)
            {
                Target.activities.AddRange(a.activities.Where(x => x.type == type));
                Target.activities.AddRange(b.activities.Where(x => x.type == type));
            }
            else
            {
                int aParam = Int32.Parse(a.GetactivityByType(CardActivityType.Splice).parameter);
                int bParam = Int32.Parse(a.GetactivityByType(CardActivityType.Splice).parameter);

                Target.activities.Add(new CardActivitySetting() { type = CardActivityType.Splice, parameter = Mathf.Max(aParam, bParam).ToString() });
            }
        }
    }


}

