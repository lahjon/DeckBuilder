using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Card : MonoBehaviour
{
    public int cost;
    public CardData cardData;

    public CardEffect Damage;
    public CardEffect Block;

    public List<CardEffect> Effects = new List<CardEffect>();
    public List<CardActivitySetting> activities = new List<CardActivitySetting>();


    public virtual void BindCardData()
    {
        cost        = cardData.cost;
        Damage      = cardData.Damage;
        Block       = cardData.Block;
        Effects     = cardData.inEffects;
        activities  = cardData.inActivities;
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


}

